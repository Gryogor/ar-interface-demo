#!/usr/bin/env python
import asyncio
from gripper import *
from std_msgs.msg import String

class Node:
    def __init__(self, ur_ip, port=63352, auto_calib=True):
        self.ur_ip=str(ur_ip)
        self.port = port
        self.gripper=Gripper(self.ur_ip)
        self.pub_sub()
        asyncio.run(self.init, auto_calib)
        self.state_msg = String()
        self.pos_msg = String()
    
    def pub_sub(self):
        self.state_pub = rospy.Publisher('/gripper_state', String, 10)
        self.pos_pub = rospy.Publisher('/gripper_pos', String, 10)
        self.command_sub = rospy.Subscriber('/gripper_command', String, self.cmd_callback)

    def cmd_callback(self, msg):
        cmds = msg.data.split(":")
        if len(cmds)==3:
            finalpos, status = asyncio.run(self.move(int(cmds[0]), int(cmds[1]), int(cmds[2])))
            self.pos_msg.data = str(finalpos)
            self.state_msg.data = str(status)
            self.pos_pub.publish(self.pos_msg)
            self.state_pub.publish(self.state_msg)

        else:
            rospy.logerr("No suitable command string received")

    async def init(self, auto_calib):
        await self.gripper.connect()
        await self.gripper.activate()
        if auto_calib:
            await self.gripper.auto_calibrate()

    async def move(self, pos:int, speed:int, force:int)-> Tuple[int, Gripper.ObjectStatus]:
        return await self.gripper.move_and_wait_for_pos(pos, speed, force)