using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialAnchor : MonoBehaviour
{
    [SerializeField]
    private Vector3 _offset = new Vector3(0f,0f,0f);

    private GameObject _robotScene = null;
    private GameObject _robotRotation = null;

    // Start is called before the first frame update
    void Start()
    {
        _robotScene = GameObject.Find("ur_position");
        _robotRotation = GameObject.Find("ur_rotation");
        _robotScene.transform.SetParent(transform);
    }

    private void Awake()
    {
        if (_robotScene != null)
        {
            return;
        }
        //_robotScene = GameObject.Find("ur_robot");
        _robotScene = GameObject.Find("ur_position");
        _robotRotation = GameObject.Find("ur_rotation");
        _robotScene.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (_robotScene == null)
        {
            _robotScene = GameObject.Find("ur_position");
            _robotRotation = GameObject.Find("ur_rotation");
            _robotScene.transform.SetParent(transform);
        }
        _robotScene.transform.localPosition = _offset;

        _robotRotation.transform.localRotation = Quaternion.identity;
        _robotRotation.transform.localRotation = Quaternion.Euler(-90f, -90f, -90f);
    }
}
