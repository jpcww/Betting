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

    #region Event
    private Dealer dealer;
    #endregion

    #region Network
    private ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();

    #endregion

    #region Bet
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

    private void Start()
    {
        // Dealer
        StartCoroutine(WaitForDealer());
        StartCoroutine(WaitForRevealedColor());

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
        
        foreach (PlayerUI playerUI in FindObjectsOfType<PlayerUI>())
        {
            if (!playerUI.GetComponentInParent<PhotonView>().IsMine)
                playerUI.GetComponentInParent<Canvas>().enabled = false;
        }
    }

    public override void OnEnable()
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

    public override void OnDisable()
    {
        playerInput.Disable();
    }

    private void Update()
    {
        // Input
        SelectGreen();
        SelectRed();
        DecideBetAmout();
        Bet();
    }

    /*
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        Debug.Log("room properties");
        if (propertiesThatChanged.ContainsKey("revealedColor"))
        {
            ProcessWinLose((int)propertiesThatChanged["revealedColor"]);
            
            Debug.Log("revealed color in the property : " + (int)propertiesThatChanged["revealedColor"]);
        }
    }
    */
    

    IEnumerator WaitForDealer()
    {
        yield return new WaitUntil(() => FindObjectOfType<Dealer>() != null);
        dealer = FindObjectOfType<Dealer>();
        transform.LookAt(dealer.transform);
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
            selectedColor = (int)ColorNames.green;
            playerUI.SetColor(Color.green);

            // Network communication
            playerProperties["selectedColor"] = (int)ColorNames.green;

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
            selectedColor = (int)ColorNames.red;
            playerUI.SetColor(Color.red);

            // Network communication
            playerProperties["selectedColor"] = (int)ColorNames.red;

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

        if (currentChipAmount <= 0)
        {
            currentChipAmount += 100;
            // TODO: CHIP REFILL ANIMATION/INSTANTIATION
        }

        // Reinitialize Flags
        betAmount = 0;
        hasBet = false;
        hasSelectedColor = false;
        selectedColor = -1;
    }
}
