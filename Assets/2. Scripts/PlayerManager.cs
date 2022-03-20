using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerManager : MonoBehaviour
{
    #region Event
    private Dealer dealer;
    #endregion

    #region UI
    public GameObject pref_playerUI;
    #endregion

    #region Network
    public PhotonView photonView;
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
        dealer = FindObjectOfType<Dealer>();
        transform.LookAt(dealer.transform);

        // Network
        photonView = GetComponent<PhotonView>();

        // UI
        //InstantiatePlayerUI();
    }

    private void OnEnable()
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

    private void OnDisable()
    {
        playerInput.Disable();
    }


    private void Update()
    {
        SelectGreen();
        SelectRed();
        DecideBetAmout();
        Bet();
    }

    private void InstantiatePlayerUI()
    {
        GameObject playerUIGO = Instantiate(pref_playerUI);
        PlayerUI playerUI = playerUIGO.GetComponent<PlayerUI>();
        playerUI.SetPlayer(this, photonView.Controller.NickName);
    }

    private void SelectGreen()
    {
        if (selectGreenInput && !hasBet)
        {
            selectedColor = (int)ColorNames.green;
            Debug.Log("selected green");
            hasSelectedColor = true;
            selectGreenInput = false;
        }
    }
    private void SelectRed()
    {
        if (selectRedInput && !hasBet)
        {
            selectedColor = (int)ColorNames.red;
            Debug.Log("selected red");
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
            dealer.warningEvent.Invoke(dealer.text_betAmount, "Please Select Color");
            decideBetAmountInput = false;
            return;
        }

        if (!hasBet && betAmount < currentChipAmount)
        {
            betAmount += defaultBetAmount;
            dealer.uiEvent.Invoke(dealer.text_betAmount, string.Format("${0}", betAmount));
            decideBetAmountInput = false;
            return;
        }

        else if(!hasBet && betAmount >= currentChipAmount)
        {
            dealer.warningEvent.Invoke(dealer.text_betAmount, "Not Enough Chips");
            decideBetAmountInput = false;
            return;
        }

        else if(hasBet)
        {
            dealer.warningEvent.Invoke(dealer.text_betAmount, "Alreadt bet");
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
            dealer.warningEvent.Invoke(dealer.text_betAmount, "Please Select Color");
            betInput = false;
            return;
        }

        if (hasSelectedColor)
        {
            currentChipAmount -= betAmount;
            dealer.warningEvent.Invoke(dealer.text_betAmount, "Already Bet");
            dealer.uiEvent.Invoke(dealer.text_betAmount, string.Format("${0}", betAmount));
            dealer.uiEvent.Invoke(dealer.text_currentAmount, string.Format("${0}", currentChipAmount));

            hasBet = true;
            betInput = false;

            dealer.betAction();
        }
    }

    // EARN CHIPS
    public void EarnChips()
    {
        Debug.Log(photonView.ViewID + " earn");
        currentChipAmount += betAmount;
        dealer.warningEvent.Invoke(dealer.text_betAmount, "Win!");
        dealer.uiEvent.Invoke(dealer.text_currentAmount, string.Format("${0}", currentChipAmount));

        betAmount = 0;
        hasBet = false;
        hasSelectedColor = false;
        selectedColor = -1;
    }
    // LOSE CHIPS
    public void LoseChips()
    {
        Debug.Log(photonView.ViewID + " lose");
        dealer.warningEvent.Invoke(dealer.text_betAmount, "Lose!");
        dealer.uiEvent.Invoke(dealer.text_currentAmount, string.Format("${0}", currentChipAmount));

        if (currentChipAmount <= 0)
        {
            currentChipAmount += 100;
            dealer.warningEvent.Invoke(dealer.text_currentAmount, "Good Luck");
            dealer.uiEvent.Invoke(dealer.text_currentAmount, string.Format("${0}", currentChipAmount));
        }

        betAmount = 0;
        hasBet = false;
        hasSelectedColor = false;
        selectedColor = -1;
    }
}
