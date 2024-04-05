using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RosSharp.RosBridgeClient
{
    public class RoundTripReceive : UnitySubscriber<MessageTypes.Geometry.PoseStamped>
    {

        RoundTripSend roundTripSendHandle;
        private void Awake()
        {
            roundTripSendHandle = transform.GetComponent<RoundTripSend>();
        }

        protected override void ReceiveMessage(PoseStamped message)
        {
            //Debug.Log("Received: " + message);
            roundTripSendHandle.SendBack(message);
        }
    }
}