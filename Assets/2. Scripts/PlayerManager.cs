using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    #region UI
    PlayerUI playerUI;
    #endregion

    #region Network
    private ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    public const byte PlayerInitiatedEventCode = 1;
    #endregion

    #region Bet
    public Dealer dealer;
    public int selectedColor;
    private bool hasSelectedColor = false;
    public int currentChipAmount = 100;
    private int defaultBetAmount = 10;
    public int betAmount = 0;
    public bool hasBet = false;
    #endregion

    #region Input
    PlayerInput playerInput;
    bool selectGreenInput = false;
    bool selectRedInput = false;
    bool decideBetAmountInput = false;
    bool betInput = false;
    #endregion

    #region Coin
    public Transform transForm_availableChips;
    public Transform transForm_betChips;
    public List<GameObject> ownedChips = new List<GameObject>();
    public List<GameObject> betChips = new List<GameObject>();
    public List<GameObject> removedOwnedChips = new List<GameObject>();
    public List<GameObject> removedBetChips = new List<GameObject>();
    #endregion

    private void Start()
    {
        InitiatePlayer();
    }

    public override void OnEnable()
    {
        if (photonView.IsMine)
        {
            if (playerInput == null)
            {
                playerInput = new PlayerInput();

                playerInput.Bet.SelectGreen.performed += i => selectGreenInput = true;
                playerInput.Bet.SelectRed.performed += i => selectRedInput = true; ;
                playerInput.Bet.DecideBetAmount.performed += i => decideBetAmountInput = true;
                playerInput.Bet.Bet.started += i => betInput = true;
            }

            playerInput.Enable();
        }
    }

    public override void OnDisable()
    {
        if(photonView.IsMine)
            playerInput.Disable();
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            // Input
            SelectGreen();
            SelectRed();
            DecideBetAmout();
            Bet();
        }
    }

    private void InitiatePlayer()
    {
        // PLAYER
        if(photonView.IsMine)
        {
            // Dealer
            dealer = PhotonView.Find(1).gameObject.GetComponent<Dealer>();

            // UI
            playerUI = GetComponentInChildren<PlayerUI>();
            playerUI.SetUserName(photonView.Owner.NickName);
            playerUI.SetColor(Color.gray);

            // Network Communication
            playerProperties.Add("selectedColor", -1);
            playerProperties.Add("betAmount", betAmount);
            playerProperties.Add("hasBet", hasBet);
            playerProperties.Add("hasReceivedColor", false);
            playerProperties.Add("hasProcessedWinLose", false);
            StartCoroutine(WaitForRevealedColor()); // REPLACE WITH EVENT

            PlayerInstantiatedEvent();
        }
    }

    private void PlayerInstantiatedEvent()
    {
        int photonViewID = photonView.ViewID;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(PlayerInitiatedEventCode, photonViewID, raiseEventOptions, ExitGames.Client.Photon.SendOptions.SendReliable);
        Debug.Log("event raised");
    }

    public IEnumerator WaitForRevealedColor()
    {
        yield return new WaitUntil(() => playerProperties.ContainsKey("hasReceivedColor") && (bool)playerProperties["hasReceivedColor"] == false && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("hasRevealedColor") && (bool)PhotonNetwork.CurrentRoom.CustomProperties["hasRevealedColor"] == true);

        playerProperties["hasReceivedColor"] = true;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);

        ProcessWinLose((int)PhotonNetwork.CurrentRoom.CustomProperties["revealedColor"]);
    }

    // SELECT COLORS
    private void SelectGreen()
    {
        if (!selectGreenInput)
            return;

        if (!hasBet)
        {
            selectedColor = (int)Colors.green;
            playerUI.SetColor(Color.green);

            // Network communication
            playerProperties["selectedColor"] = (int)Colors.green;

            hasSelectedColor = true;
            selectGreenInput = false;
        }
    }
    private void SelectRed()
    {
        if (!selectRedInput)
            return;

        if (!hasBet)
        {
            selectedColor = (int)Colors.red;
            playerUI.SetColor(Color.red);

            // Network communication
            playerProperties["selectedColor"] = (int)Colors.red;

            hasSelectedColor = true;
            selectRedInput = false;
        }
    }

    // DECIDE AMOUNT OF BETTING
    public void DecideBetAmout()
    {
        if (!decideBetAmountInput)
            return;

        if(!hasSelectedColor)
        {
            playerUI.DisplayWarningMessage("Please Select Color");
            decideBetAmountInput = false;
            return;
        }

        if (!hasBet && betAmount < currentChipAmount)
        {
            betAmount += defaultBetAmount;
            playerUI.DisplayBetAmount(string.Format("Bet Amount : ${0}", betAmount));

            // Network communication
            playerProperties["betAmount"] = betAmount;

            decideBetAmountInput = false;
            return;
        }

        else if(!hasBet && betAmount >= currentChipAmount)
        {
            playerUI.DisplayWarningMessage("Not Enough Chips");
            decideBetAmountInput = false;
            return;
        }

        else if(hasBet)
        {
            playerUI.DisplayWarningMessage("Already Bet");
            decideBetAmountInput = false;
            return;
        }
    }

    // BET
    public void Bet()
    {
        if (!betInput|| hasBet)
            return;

        if (!hasSelectedColor)
        {
            playerUI.DisplayWarningMessage("Please Select Color");
            betInput = false;
            return;
        }

        if (hasSelectedColor)
        {
            // UI
            currentChipAmount -= betAmount;
            playerUI.DisplayerCurrentChipAmount(string.Format("Current Chip : ${0}", currentChipAmount));

            // Move Chips
            BetChips(betAmount);

            // Initialize UI
            hasBet = true;
            betInput = false;

            // Network communication
            playerProperties["hasBet"] = hasBet;
            PhotonNetwork.SetPlayerCustomProperties(playerProperties);
        }
    }

    // PROCESS WIN/LOSE
    private void ProcessWinLose(int revealedColor)
    {
        Debug.Log("ProcessWinLose");
        if(revealedColor == selectedColor)
        {
            EarnChips();
        }

        else
        {
            LoseChips();
        }

        hasSelectedColor = false;
        hasBet = false;
        playerProperties["hasBet"] = hasBet;
        playerProperties["hasReceivedColor"] = false;
        PhotonNetwork.SetPlayerCustomProperties(playerProperties);  // It seems to take time to update through the server

        // Reinitialize Coroutine in Dealer
        StartCoroutine(dealer.WaitUntilAllBet());
        StartCoroutine(WaitForRevealedColor());
    }

    // EARN CHIPS
    public void EarnChips()
    {
        // UI
        currentChipAmount += betAmount;
        playerUI.DisplayWinLose(true);
        playerUI.DisplayerCurrentChipAmount(string.Format("Current Chip : ${0}", currentChipAmount));

        // CHIPS
        GetChipsBack(betAmount);

        // Reinitialize Flags
        betAmount = 0;
        hasBet = false;
        hasSelectedColor = false;
        selectedColor = -1;
    }
    // LOSE CHIPS
    public void LoseChips()
    {
        playerUI.DisplayWinLose(false);
        playerUI.DisplayerCurrentChipAmount(string.Format("Current Chip : ${0}", currentChipAmount));

        // CHIPS
        RemoveChips(betAmount);

        if (currentChipAmount <= 0)
        {
            currentChipAmount += 100;

            RefillChips();
        }

        // Reinitialize Flags
        betAmount = 0;
        hasBet = false;
        hasSelectedColor = false;
        selectedColor = -1;
    }

    private void BetChips(int betAmount)
    {
        for (int i = 0; i < betAmount / 10; i++)
        {
            ownedChips[i].SetActive(false);
            betChips[i].SetActive(true);
        }
    }

    private void GetChipsBack(int betAmount)
    {
        for (int i = 0; i < betAmount / 10; i++)
        {
            ownedChips[i].SetActive(true);
            betChips[i].SetActive(false);
        }
    }

    private void RemoveChips(int betAmount)
    {
        int range = betAmount / 10;
        removedOwnedChips.AddRange(ownedChips.GetRange(0, range));
        for (int i = 0; i < range; i++)
            removedOwnedChips[i].SetActive(false);
        ownedChips.RemoveRange(0, range);

        removedBetChips.AddRange(betChips.GetRange(0, range));
        for (int i = 0; i < range; i++)
            removedBetChips[i].SetActive(false);
        betChips.RemoveRange(0, range);
    }

    private void RefillChips()
    {
        List<GameObject> temp = ownedChips;
        ownedChips.Clear();
        ownedChips.AddRange(removedOwnedChips);
        ownedChips.AddRange(temp);
        foreach (GameObject ownedChip in ownedChips)
            ownedChip.SetActive(true);

        temp.Clear();

        temp = betChips;
        betChips.Clear();
        betChips.AddRange(removedBetChips);
        betChips.AddRange(temp);
        foreach (GameObject betChip in betChips)
            betChip.SetActive(false);
    }
}
