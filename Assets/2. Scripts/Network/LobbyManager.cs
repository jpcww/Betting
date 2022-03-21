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

    #region Player List
    public PlayerItem pref_playerItem;
    List<PlayerItem> playerItems = new List<PlayerItem>();
    public Transform playerList;
    #endregion

    #region Buttons
    public GameObject button_play;
    #endregion

    private void Awake()
    {
        lobbyPanel.SetActive(true);
        roomPanel.SetActive(false);

        button_play.SetActive(false);
    }

    private void Start()
    {
        PhotonNetwork.JoinLobby(); // Connect to Lobby as connected to server
    }

    private void Update()
    {
        CheckPlayButton();
    }

    #region Lobby
    public void OnClickCreate()
    {
        if (input_roomName.text.Length >= 1) // TODO: add warning when there is no name
        {
            PhotonNetwork.CreateRoom(input_roomName.text, new RoomOptions() { MaxPlayers = 3, BroadcastPropsChangeToAll = true });  // Create a room
        }
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
    #endregion

    #region Room

    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);  // Open Room panel
        text_roomName.text = "Room Name : " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        // Clear PlayerItem List
        foreach(PlayerItem playerItem in playerItems)
        {
            Destroy(playerItem.gameObject);
        }

        playerItems.Clear();

        // Fill up PlayerItem List
        if (PhotonNetwork.CurrentRoom == null)
            return;

        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem playerItem = Instantiate(pref_playerItem, playerList);
            playerItem.SetPlayerInfo(player.Value);

            // Indicate the local player
            if(player.Value == PhotonNetwork.LocalPlayer)
            {
                playerItem.ApplyLocalChanges();
            }

            playerItems.Add(playerItem);
        }
    }

    private void CheckPlayButton()
    {
        // Only the master client is allowed to hit the play button
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            button_play.SetActive(true);
        }
        else
        {
            button_play.SetActive(false);
        }
    }

    public void OnClickPlayButton()
    {
        PhotonNetwork.LoadLevel("Game");    // Load the game scene
    }
    #endregion
}
