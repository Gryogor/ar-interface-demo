#include <moveit/move_group_interface/move_group_interface.h>
#include <moveit/planning_scene_interface/planning_scene_interface.h>

#include <moveit_msgs/DisplayRobotState.h>
#include <moveit_msgs/DisplayTrajectory.h>

#include <moveit_msgs/AttachedCollisionObject.h>
#include <moveit_msgs/CollisionObject.h>
#include <moveit_msgs/GetPositionFK.h>

#include <moveit_visual_tools/moveit_visual_tools.h>
#include <tf2/LinearMath/Transform.h>

#include "ros/ros.h"
#include "ski_assembly/cobot_positions.h"
#include "geometry_msgs/PoseArray.h"
#include "std_msgs/Bool.h"

std::vector<geometry_msgs::Pose> get_trajectory_poses(moveit::planning_interface::MoveGroupInterface::Plan, ros::NodeHandle&);
void movement_task
  (ros::NodeHandle&,
  moveit::planning_interface::MoveGroupInterface&,
  std::vector<std::vector<double>>,
  std::vector<std::vector<double>>,
  ros::Publisher&,
  ros::Publisher&);

void is_recieved_callback(const std_msgs::Bool::ConstPtr&);
bool isRecieved = false;


std::string EEF_TOPIC = "/end_effector_poses";
std::string CLEAR_LIST_TOPIC = "/clear_list";

std::vector<float> testing_coords = {-0.265, -0.224, 0.195};
std::vector<float> testing_orientation_radian = {-0.637, 0.642, 0.295, 0.307};


int main(int argc, char** argv)
{
  ros::init(argc, argv, "ski_assembly_demo");
  ros::NodeHandle _nh;

  ros::AsyncSpinner spinner(1);
  spinner.start();

  ros::Publisher eef_poses_pub = _nh.advertise<geometry_msgs::PoseArray>(EEF_TOPIC, 1);

  ros::Publisher to_clear_pub = _nh.advertise<std_msgs::Bool>(CLEAR_LIST_TOPIC, 1);
  std_msgs::Bool to_clear;

  ros::Subscriber is_recieved = _nh.subscribe("/unity/recieved", 1, &is_recieved_callback);

  static const std::string PLANNING_GROUP_ARM = "manipulator";
  // static const std::string PLANNING_GROUP_GRIPPER = "hande";

  //setting up interfaces
  static moveit::planning_interface::MoveGroupInterface move_group_arm_interface(PLANNING_GROUP_ARM);
  // static moveit::planning_interface::MoveGroupInterface move_group_gripper_interface(PLANNING_GROUP_GRIPPER);

  //setting up planning scene interface
  moveit::planning_interface::PlanningSceneInterface planning_scene_interface;

  const robot_state::JointModelGroup* joint_model_group_arm =
    move_group_arm_interface.getCurrentState()->getJointModelGroup(PLANNING_GROUP_ARM);
  

  std::vector<double> starting_position = demo_joint_states[0];
  std::vector<double> starting_pose = demo_tcp_states[0];

  std::vector<std::vector<double>> code_joint_states;
  std::vector<std::vector<double>> code_tcp_states;
  code_joint_states.push_back(starting_position);
  code_tcp_states.push_back(starting_pose);
  movement_task(_nh, move_group_arm_interface, code_joint_states, code_tcp_states, eef_poses_pub, to_clear_pub);
  for (int i = 1; i < demo_joint_states.size(); i+=2)
  {
    to_clear.data = true;
    to_clear_pub.publish(to_clear);
    to_clear.data = false;
    ROS_INFO("i: %d", i);
    std::vector<std::vector<double>> code_joint_states;
    code_joint_states.push_back(demo_joint_states[i]);
    std::vector<std::vector<double>> code_tcp_states;
    code_tcp_states.push_back(demo_tcp_states[i]);
    if (i+1 >= demo_joint_states.size())
    {
      to_clear_pub.publish(to_clear);
      movement_task(_nh, move_group_arm_interface, code_joint_states, code_tcp_states, eef_poses_pub, to_clear_pub);
      break;
    }
    to_clear_pub.publish(to_clear);
    code_joint_states.push_back(demo_joint_states[i+1]);
    code_tcp_states.push_back(demo_tcp_states[i+1]);
    movement_task(_nh, move_group_arm_interface, code_joint_states, code_tcp_states, eef_poses_pub, to_clear_pub);
  }
  return 0;
}

std::vector<geometry_msgs::Pose> get_trajectory_poses(
  moveit::planning_interface::MoveGroupInterface::Plan moveit_plan,
  ros::NodeHandle &nh)
{
  //to return later
  std::vector<geometry_msgs::Pose> eef_poses;

  //get the trajectory from function input
  moveit_msgs::RobotTrajectory trajectory = moveit_plan.trajectory_;
  //for(std::vector<trajectory_msgs::JointTrajectoryPoint> joint_point : trajectory.joint_trajectory.points)

  //setup rosservice call
  ros::ServiceClient client = nh.serviceClient<moveit_msgs::GetPositionFK>("/compute_fk");
  moveit_msgs::GetPositionFK srv;
  srv.request.header.frame_id = "base";
  srv.request.fk_link_names.push_back("phantom_gripper");
  srv.request.robot_state.joint_state.name = trajectory.joint_trajectory.joint_names;
  for (auto &&point : trajectory.joint_trajectory.points)
  {
    srv.request.robot_state.joint_state.position = point.positions;
    if (client.call(srv))
    {
      geometry_msgs::Pose pose = srv.response.pose_stamped[0].pose;
      eef_poses.push_back(pose);
    }
    else
    {
      ROS_ERROR("Failed to call service");
    }
  }

  return eef_poses;
}

void movement_task (ros::NodeHandle &nh,
  moveit::planning_interface::MoveGroupInterface &interface,
  std::vector<std::vector<double>> joint_states,
  std::vector<std::vector<double>> end_goals,
  ros::Publisher &eef_poses_pub,
  ros::Publisher &to_clear_pub)
{
  int plan_n = 0;
  std::vector<moveit::planning_interface::MoveGroupInterface::Plan> plans;
  moveit::planning_interface::MoveGroupInterface::Plan plan;
  std::vector<std::vector<geometry_msgs::Pose>> all_eef_poses;
  robot_state::RobotState start_state(*interface.getCurrentState());
  interface.setStartState(start_state);

  for (auto joint_state : joint_states) //for each joint state
  {
    //plan for a joint space
    interface.setJointValueTarget(joint_state);

    bool success = (interface.plan(plan) == moveit::planning_interface::MoveItErrorCode::SUCCESS);
    //save the plan
    plans.push_back(plan);
    //update starting position for next plan
    start_state.setJointGroupPositions(interface.getName(), plan.trajectory_.joint_trajectory.points.back().positions);
    interface.setStartState(start_state);
    //compute eef
    std::vector<geometry_msgs::Pose> eef_poses = get_trajectory_poses(plan, nh);
    //save eef
    all_eef_poses.push_back(eef_poses);
    //iterate plan_n
    plan_n++;
  }

  //display all eef (publish to topic)
  geometry_msgs::PoseArray all_eef_poses_msg;
  all_eef_poses_msg.header.frame_id = "base_link";
  for (auto eef_poses : all_eef_poses)
  {
    for (auto eef_pose : eef_poses)
    {
      all_eef_poses_msg.poses.push_back(eef_pose);
    }
  }

  while (!isRecieved)
  {
    eef_poses_pub.publish(all_eef_poses_msg);
    ros::Duration(0.1).sleep();
  }
  for (int i = 0; i < 10; i++)
  {
    isRecieved = false;
    ros::Duration(0.1).sleep();
  }

  //execute all saved plans
  for (auto plan : plans)
  {
    interface.execute(plan);
    ros::Duration(0.2).sleep();
    ROS_INFO("Plan executed");
  }
  std_msgs::Bool to_clear;
  while (!isRecieved)
  {
    to_clear.data = true;
    to_clear_pub.publish(to_clear);
    ros::Duration(0.05).sleep();
  }
  to_clear.data = false;

  for (int i = 0; i < 2; i++)
  {
    isRecieved = false;
    ros::Duration(0.05).sleep();
  }

}

std::vector<double> coord_converter(std::vector<float> coords)
{
  std::vector<double> converted_coords;
  geometry_msgs::Pose pose;
  pose.position.x = coords[0];
  pose.position.y = coords[2];
  pose.position.z = coords[2];
  pose.orientation.w = coords[3];
  pose.orientation.x = coords[4];
  pose.orientation.y = coords[5];
  pose.orientation.z = coords[6];


    
  return converted_coords;
}