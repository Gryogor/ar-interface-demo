using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoolSubscriber : RosSharp.RosBridgeClient.UnitySubscriber<RosSharp.RosBridgeClient.MessageTypes.Std.Bool>
{
    private Queue<Action> actions = new Queue<Action>();
    private BoolPublisher _BoolPublisherHandle;

    private void Awake()
    {
        _BoolPublisherHandle = GetComponent<BoolPublisher>();
    }

    private void Update()
    {
        while (actions.Count > 0)
        {
            actions.Dequeue().Invoke();
        }
    }

    protected override void ReceiveMessage(Bool message)
    {
        Debug.Log("Received! " + message.data);
        if (message.data == true)
        {
            Debug.Log("Clearing!");
            actions.Enqueue(() =>
            {
                transform.GetComponent<EefPositionSubscriber>().ClearList();

            });
            Debug.Log("Cleared!");
        }
        _BoolPublisherHandle.UpdateMsg(true);
    }
}
