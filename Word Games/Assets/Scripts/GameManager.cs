using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    public string gameUrl = "192.168.0.69:1818";

    [Header("Common Components")]
    public Players players;

    

    protected ServerCommunicator server;
    


    public void StartServer()
    {
        server = new ServerCommunicator(gameUrl);
    }


    public void OnApplicationQuit()
    {
        server.Close();
    }
}