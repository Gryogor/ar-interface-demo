using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.RosBridgeClient
{
    public class RobotPositionSubscriber : UnitySubscriber<MessageTypes.Geometry.Pose>
    {
        [SerializeField]
        private GameObject test_sphere;

        private Queue<Action> actions = new Queue<Action>();
        



        EefPositionSubscriber _previewPositionManager;

        // Update is called once per frame
        void Update()
        {
            while (actions.Count > 0)
            {
                actions.Dequeue().Invoke();
            }
        }

        private void Awake()
        {
            _previewPositionManager = GetComponent<EefPositionSubscriber>();
            

        }

        protected override void ReceiveMessage(MessageTypes.Geometry.Pose message)
        {
            //Debug.Log("Recieved!");
            actions.Enqueue(() =>
            {
            //Debug.Log(message.position.x);
                Vector3 RobotPosition = new Vector3(-(float)message.position.y, (float)message.position.z, (float)message.position.x);
                //Debug.Log($"RobotPosition: {RobotPosition}");
                Test(RobotPosition);
                //List<GameObject> _prefabPositionList = _previewPositionManager.EefPositionList;
                //var delta = _prefabPositionList[0].transform.localPosition - RobotPosition;
                //Debug.Log(delta.magnitude);
                //if(delta.magnitude < 0.08f && delta.magnitude > -0.08f)
                //{
                //    GameObject _prefabToDestroy = _prefabPositionList[0];
                //    _prefabPositionList.RemoveAt(0);
                //    Destroy(_prefabToDestroy);
                //}
            });
        }

        private void Test(Vector3 coord)
        {
            test_sphere.transform.position = coord;
        }
    }
}
