using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI input_userName;
    public TextMeshProUGUI buttonText;

    // CONNECT TO SERVER
    public void OnClickConnect()
    {
        if(input_userName.text.Length >= 1) //TODO: add warning message
        {
            PhotonNetwork.NickName = input_userName.text; // Set User Name
            Debug.Log("username : " + input_userName.text);
            buttonText.text = "CONNECTING.......";
            PhotonNetwork.ConnectUsingSettings();   // Connect to server
        }
    }

    // LOAD LOBBY
    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
