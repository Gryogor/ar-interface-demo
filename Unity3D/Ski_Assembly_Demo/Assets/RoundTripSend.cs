using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RosSharp.RosBridgeClient
{
    public class RoundTripSend : UnityPublisher<MessageTypes.Geometry.PoseStamped>
    {

        MessageTypes.Geometry.PoseStamped msgToSend = new MessageTypes.Geometry.PoseStamped();
        private float timer = 0.0f;
        int id = 0;

        System.DateTime epochStart = new System.DateTime(1970, 1, 1);

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            
            if (timer >= 1.0f)
            {
                timer = 0.0f;
                msgToSend.header.seq = (uint)id;
                id++;
                msgToSend.pose.position.x = 0f;
                msgToSend.pose.position.y = 0f;
                msgToSend.pose.position.z = 0f;
                msgToSend.pose.orientation.x = 0f;
                msgToSend.pose.orientation.y = 0f;
                msgToSend.pose.orientation.z = 0f;
                msgToSend.pose.orientation.w = 1f;
                long elapsedTicks = DateTime.UtcNow.Ticks - epochStart.Ticks;
                long elapsedSecs = elapsedTicks / 10000000;
                long elapsedNsecs = (elapsedTicks * 100) - (elapsedSecs * 1000000000);
                msgToSend.header.stamp.secs = (uint)elapsedSecs;
                msgToSend.header.stamp.nsecs = (uint)elapsedNsecs;
                SendBack(msgToSend);
            }

        }

        public void SendBack(MessageTypes.Geometry.PoseStamped message)
        {
            //Debug.Log("sending message");
            Publish(message);
        }
    }
}