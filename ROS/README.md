# ROS packages for rybalskii-incom-2024-replication-package

1. Install [Ubuntu 20.04](https://releases.ubuntu.com/focal/)

2. Install ROS1 Noetic following this [page](https://wiki.ros.org/noetic/Installation/Ubuntu). Make sure to also install rosdep.

3. Create catkin workspace folder and src folder in it. It should be somethig like 'catkin_ws/src'. Put all the content of 'packages' folder of this repository into src.

4. While in 'catkin_ws', run:

```bash
$ rosdep update
$ rosdep install --from-paths src --ignore-src -r -y
```

This will install all the necessary dependencies for downloaded packages

5. Extract calibration file for the robot:

```
$ roslaunch ur_calibration calibration_correction.launch \
robot_ip:=<robot_ip> target_filename:="${HOME}/my_robot_calibration.yaml"
```

Repalce <robot_ip> with the ip, which the robot got assigned in the network.

Calibration file can be put in any location on computer, but [Universal Robots recommends](https://github.com/UniversalRobots/Universal_Robots_ROS_Driver?tab=readme-ov-file#extract-calibration-information), to put them in a common package, especially if there are sever URs.

6. Install [externalcontrol URCap](https://github.com/UniversalRobots/Universal_Robots_ExternalControl_URCap) onto UR robot and create a program with it.

7. Launch robot bringrup and urcap and after that URCap program:

```
$ roslaunch ur_robot_driver <robot_type>_bringup.launch robot_ip:=<robot_ip> \
kinematics_config:=<path_to_calibration_file>
```

8. Start moveit:

```
$ roslaunch ur5e_moveit_config moveit_planning_execution.launch
```

9. Launch robridge:

```
$ roslaunch rosbridge_server rosbridge_websocket.launch
```

10. After launching the AR interface, start the demo:

```
$ rosrun ski_assembly ski_assembly_demo
```