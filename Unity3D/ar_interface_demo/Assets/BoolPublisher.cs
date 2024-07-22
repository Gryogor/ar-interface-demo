using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolPublisher : RosSharp.RosBridgeClient.UnityPublisher<RosSharp.RosBridgeClient.MessageTypes.Std.Bool>
{
    private RosSharp.RosBridgeClient.MessageTypes.Std.Bool message;

    protected override void Start()
    {
        base.Start();
        InitializeMessage();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (message.data == true)
        {
            for (int i = 0; i < 50; i++)
            {
                Publish(message);
                Debug.Log("Sending bool!");
            }
            message.data = false;
        }
        Publish(message);
        Debug.Log("Sending bool!");

    }

    public void UpdateMsg(bool msg)
    {
        message.data = msg;
    }

    private void InitializeMessage()
    {
        message = new RosSharp.RosBridgeClient.MessageTypes.Std.Bool
        {
            data = false
        };
    }
}
