using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RosSharp.RosBridgeClient
{
    public class EefPositionSubscriber : UnitySubscriber<MessageTypes.Geometry.PoseArray>
    {
        [SerializeField]
        private GameObject EefPositionPrefab;
        private GameObject parentFolder;
        [SerializeField]
        private GameObject robotObject;

        private Queue<Action> actions = new Queue<Action>();

        private BoolPublisher _BoolPublisherHandle;


        [HideInInspector]
        public List<GameObject> EefPositionList = new List<GameObject>();

        //protected override void Start()
        //{
        //    parentFolder = new GameObject("EefPositions");
        //    parentFolder.transform.SetParent(robotObject.transform);
        //    parentFolder.transform.localPosition = new Vector3(0f, 0f, 0f);
        //    parentFolder.transform.localRotation = Quaternion.identity;


        //}

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

        protected override void ReceiveMessage(MessageTypes.Geometry.PoseArray message)
        {

            foreach (var Pose in message.poses)
            {
                //Debug.Log("EefPositionSubscriber ReceiveMessage");
                //Debug.Log(Pose.position.x);
                //Debug.Log(Pose.position.y);
                //Debug.Log(Pose.position.z);
                //Debug.Log(Pose.orientation.x);
                //Debug.Log(Pose.orientation.y);
                //Debug.Log(Pose.orientation.z);
                //Debug.Log(Pose.orientation.w);
                actions.Enqueue(() =>
                {
                    GameObject EefPosition = Instantiate(EefPositionPrefab, new Vector3(-(float)Pose.position.y, (float)Pose.position.z, (float)Pose.position.x), Quaternion.identity);
                    EefPosition.transform.SetParent(robotObject.transform);
                    EefPosition.transform.localPosition = new Vector3((float)Pose.position.y, (float)Pose.position.z, -(float)Pose.position.x);
                    EefPositionList.Add(EefPosition);
                });
            }
            _BoolPublisherHandle.UpdateMsg(true);
        }

            

        //later to swap to deletetig based on the moving arm
        //for other subscriber to later delete
        public void ClearList()
        {
            // Debug.Log("EefPositionSubscriber ClearList");
            foreach (GameObject EefPosition in EefPositionList)
            {
                Destroy(EefPosition);
            }
            EefPositionList = new List<GameObject>();
        }

        
    }
}
