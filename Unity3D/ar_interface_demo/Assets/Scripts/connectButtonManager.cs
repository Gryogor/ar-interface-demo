using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class connectButtonManager : MonoBehaviour
{
    public GameObject textBoxTMP;
    public GameObject buttonTMP;

    public GameObject IPtext;
    private string _ipText = "192.168.0.1";
    private string _portText = "9090";

    public void onButtonClick()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
