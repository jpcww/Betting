using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI input_roomName;
    public GameObject lobbyPanel;
    public GameObject roomPanel;
    public TextMeshProUGUI text_roomName;

    #region Room List
    public RoomItem pref_roomItem;
    List<RoomItem> roomItems = new List<RoomItem>();
    public Transform content;
    public float updateInterval = 1.5f;
    float nextUpdateTime;
    #endregion

    private void Start()
    {
        PhotonNetwork.JoinLobby(); // Connect to Lobby as connected to server
    }

    public void OnClickCreate()
    {
        if(input_roomName.text.Length >= 1) // TODO: add warning when there is no name
        {
            PhotonNetwork.CreateRoom(input_roomName.text, new RoomOptions() { MaxPlayers = 3});  // Create a room
        }
    }

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);  // Open Room panel
        text_roomName.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomInfos)
    {
        if (Time.time >= nextUpdateTime)
        {
            UpdateRoomList(roomInfos);
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    private void UpdateRoomList(List<RoomInfo> roomInfos)
    {
        // Clear the RoomTimes
        foreach(RoomItem roomItem in roomItems)
        {
            Destroy(roomItem.gameObject);
        }
        roomItems.Clear();

        // Fill up the list
        foreach(RoomInfo roomInfo in roomInfos)
        {
            RoomItem room = Instantiate(pref_roomItem, content);
            room.SetRoomName(roomInfo.Name);
            roomItems.Add(room);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
}
