using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace RosSharp.RosBridgeClient
{
    public class RoundTripReceive : MonoBehaviour
    {

        public string Topic;
        public float TimeStep;

        private RosConnector rosConnector;
        private readonly int SecondsTimeout = 1;

        protected virtual void Start()
        {
            rosConnector = GetComponent<RosConnector>();
            new Thread(Subscribe).Start();
        }

        private void Subscribe()
        {

            if (!rosConnector.IsConnected.WaitOne(SecondsTimeout * 1000))
                Debug.LogWarning("Failed to subscribe: RosConnector not connected");

            rosConnector.RosSocket.Subscribe<MessageTypes.DelayTest.ByteStamped>(Topic, ReceiveMessage, (int)(TimeStep * 1000)); // the rate(in ms in between messages) at which to throttle the topics
        }

        RoundTripSend roundTripSendHandle;
        private void Awake()
        {
            roundTripSendHandle = transform.GetComponent<RoundTripSend>();
        }

        protected void ReceiveMessage(MessageTypes.DelayTest.ByteStamped message)
        {
            //Debug.Log("Received: " + message);
            roundTripSendHandle.SendBack(message);
        }
    }
}