using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomItem : MonoBehaviour
{
    public TextMeshProUGUI roomName;
    LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    public void SetRoomName(string roomName)
    {
        this.roomName.text = roomName;
    }

    public void OnClickRoomItem()
    {
        lobbyManager.JoinRoom(roomName.text);
    }
}
