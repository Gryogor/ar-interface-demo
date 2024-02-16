#!/usr/bin/env python
from gripperControl import Node
import rospy

if __name__ == '__main__':
    rospy.init_node('/gripper_controller')
    node = Node("169.254.247.173") #placeholder ip
    rospy.spin()