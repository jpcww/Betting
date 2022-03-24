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
    List<Color> colors = new List<Color>();
    bool allBet = false;
    int currentNumberOfUsers = 0;
    List<ValueTuple<int, Color, int>> playerInfos = new List<ValueTuple<int, Color, int>>(); // UserID, Color, BetAmount
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
            ValueTuple<int, Color, int> playerInfo = new ValueTuple<int, Color, int>((int)photonEvent.CustomData, Color.gray, 0);
            playerInfos.Add(playerInfo);
            currentNumberOfUsers = playerInfos.Count;
        }

        // Event : Bet
        else if (eventCode == BetEventCode)
        {
            ValueTuple<int, Color, int> receivedPlayerInfo = (ValueTuple<int, Color, int>)photonEvent.CustomData; 
            OnReceiveBet(receivedPlayerInfo);
        }
    }

    private void OnReceiveBet(ValueTuple<int, Color, int> receivedInfo)
    {
        // Check all bets from players
        UpdatePlayerInfo(receivedInfo);

        CheckIfAllBet();

        // TODO : UPDATE UI THAT SHOW EVERY ONE's BET AND COLOR
    }

    private void UpdatePlayerInfo(ValueTuple<int, Color, int> receivedInfo)
    {
        for (int i = 0; i < playerInfos.Count; i++)
        {
            if (playerInfos[i].Item1 == receivedInfo.Item1)
            {
                ValueTuple<int, Color, int> matchedInfo = playerInfos[i];
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
            foreach(ValueTuple<int, Color, int> playerInfo in playerInfos)
            {
                colors.Add(playerInfo.Item2);
            }

            if (!colors.Contains(Color.gray))
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
