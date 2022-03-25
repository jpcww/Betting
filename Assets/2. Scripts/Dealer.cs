using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class Dealer : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region Network
    public Hashtable dealerProperties = new Hashtable();

    public const byte PlayerSpawnedEventCode = 1;
    public const byte BetEventCode = 2;
    public const byte ColorAnnounceEventCode = 3;
    #endregion

    #region Color
    public GameObject colorBox;
    public Material material;
    private int randomForColor;
    private Color revealedColor;
    #endregion

    #region Player
    List<int> colors = new List<int>();
    bool allBet = false;
    int currentNumberOfUsers = 0;
    List<ValueTuple<string, int, int>> playerInfos = new List<ValueTuple<string, int, int>>(); // UserID, ColorNumber, BetAmount
    #endregion

    #region Coin

    #endregion
    private void Start()
    {
        // Check Players

        // Color Reveal
        material = colorBox.GetComponent<MeshRenderer>().material;
        material.color = Color.grey;

        // Start Coroutine
        StartCoroutine(WaitUntilAllBet());
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        // Event : Player Spawn
        if(eventCode == PlayerSpawnedEventCode)
        {
            ValueTuple<string, int, int> playerInfo = new ValueTuple<string, int, int>((string)photonEvent.CustomData, -1, 0);
            playerInfos.Add(playerInfo);
            currentNumberOfUsers = playerInfos.Count;
        }

        // Event : Bet
        else if (eventCode == BetEventCode)
        {
            ValueTuple<string, int, int> receivedPlayerInfo = new ValueTuple<string, int, int>();
            object[] receivedData = (object[])photonEvent.CustomData;
            receivedPlayerInfo.Item1 = (string)receivedData[0];
            receivedPlayerInfo.Item2 = (int)receivedData[1];
            receivedPlayerInfo.Item3 = (int)receivedData[2];

            OnReceiveBet(receivedPlayerInfo);
        }
    }

    private void OnReceiveBet(ValueTuple<string, int, int> receivedInfo)
    {
        // Check all bets from players
        UpdatePlayerInfo(receivedInfo);

        CheckIfAllBet();

        // TODO : UPDATE UI THAT SHOW EVERY ONE's BET AND COLOR
    }

    private void UpdatePlayerInfo(ValueTuple<string, int, int> receivedInfo)
    {
        for (int i = 0; i < playerInfos.Count; i++)
        {
            if (playerInfos[i].Item1 == receivedInfo.Item1)
            {
                ValueTuple<string, int, int> matchedInfo = playerInfos[i];
                matchedInfo.Item2 = receivedInfo.Item2;
                matchedInfo.Item3 = receivedInfo.Item3;
                playerInfos[i] = matchedInfo;
                return;
            }

            else
                continue;
        }
    }

    private void CheckIfAllBet()
    {
        // Check if there are players that just joined/left while other players are betting
        if(currentNumberOfUsers == playerInfos.Count)
        {
            foreach(ValueTuple<string, int, int> playerInfo in playerInfos)
            {
                colors.Add(playerInfo.Item2);
            }

            if (!colors.Contains(-1))
            {
                allBet = true;
                colors.Clear();
            }

            else
            {
                colors.Clear();
            }
        }
    }

    public IEnumerator WaitUntilAllBet()
    {
        yield return new WaitUntil(() => allBet == true);

        allBet = false;

        RevealColor();

        photonView.RPC("AnnounceColor", RpcTarget.All);
    }

    // REAVEAL THE COLOR
    private void RevealColor()
    {
        randomForColor = UnityEngine.Random.Range(0, 2);
        if (randomForColor == 0)
        {
            revealedColor = Color.green;
        }
        else if (randomForColor == 1)
        {
            revealedColor = Color.red;
        }

        StartCoroutine(WaitUntilAllBet());
    }

    [PunRPC]
    private void AnnounceColor()
    {
        Debug.Log("AnnounceColor : " + revealedColor);
        material.color = revealedColor;

        ColorAnnouncedEvent();
    }

    private void ColorAnnouncedEvent()
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(ColorAnnounceEventCode, revealedColor, raiseEventOptions, SendOptions.SendReliable);
        Debug.Log(string.Format($"Color Revealed : {revealedColor}"));
    }
}
