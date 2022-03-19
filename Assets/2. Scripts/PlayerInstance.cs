﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PlayerInstance : MonoBehaviour
{
    #region Event
    private Dealer dealer;
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

    #region UI
    public TextMeshProUGUI Text_betAmount;
    public TextMeshProUGUI Text_moneyAmount;
    private string warning_SelectColor = "Please Choose your color";
    private string warning_NotEnoughChip = "Not enough chip to bet";
    private string warning_AlreadyBet = "Already Bet";
    #endregion

    #region Input
    PlayerInput playerInput;
    #endregion

    private void Start()
    {
        // UI
        Text_moneyAmount.text += string.Format("${0}", currentChipAmount);

        // Dealer
        dealer = FindObjectOfType<Dealer>();
        transform.LookAt(dealer.transform);

        // Network
        photonView = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInput();

            playerInput.Bet.SelectGreen.performed += i => SelectGreen();
            playerInput.Bet.SelectRed.performed += i => SelectRed();
            playerInput.Bet.DecideBetAmount.performed += i => DecideBetAmout();
            playerInput.Bet.Bet.started += i => Bet();
        }

        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void SelectGreen()
    {
        if (!hasBet)
        {
            selectedColor = (int)ColorNames.green;
            Debug.Log("selected green");
            hasSelectedColor = true;
        }
    }
    private void SelectRed()
    {
        if (!hasBet)
        {
            selectedColor = (int)ColorNames.red;
            Debug.Log("selected red");
            hasSelectedColor = true;
        }
    }

    // DECIDE AMOUNT OF BETTING
    public void DecideBetAmout()
    {
        if(!hasSelectedColor)
        {
            StartCoroutine(ShowNotification(Text_betAmount, warning_SelectColor, 1));
            return;
        }

        if (!hasBet && betAmount < currentChipAmount)
        {
            betAmount += defaultBetAmount;
            Text_betAmount.text = string.Format("${0}", betAmount);
            return;
        }

        else if(!hasBet && betAmount >= currentChipAmount)
        {
            StartCoroutine(ShowNotification(Text_betAmount, warning_NotEnoughChip, 1));
            return;
        }

        else if(hasBet)
        {
            StartCoroutine(ShowNotification(Text_betAmount, warning_AlreadyBet, 1));
            return;
        }
    }

    // BET
    public void Bet()
    {
        if (hasBet)
            return;

        if (!hasSelectedColor)
        {
            StartCoroutine(ShowNotification(Text_betAmount, warning_SelectColor, 1));
            return;
        }

        if (hasSelectedColor)
        {
            currentChipAmount -= betAmount;
            Text_betAmount.text = "Bet";
            Text_moneyAmount.text = string.Format("${0}", currentChipAmount);

            hasBet = true;

            dealer.betAction();
        }
    }

    // EARN CHIPS
    public void EarnChips()
    {
        Debug.Log(photonView.ViewID + " earn");
        currentChipAmount += betAmount;
        ShowNotification(Text_moneyAmount, "Win", 1);
        Text_moneyAmount.text = string.Format("${0}", currentChipAmount);

        betAmount = 0;
        hasBet = false;
        hasSelectedColor = false;
        selectedColor = -1;
    }
    // LOSE CHIPS
    public void LoseChips()
    {
        Debug.Log(photonView.ViewID + " lose");
        ShowNotification(Text_moneyAmount, "Lose", 1);

        if (currentChipAmount <= 0)
        {
            currentChipAmount += 100;
            ShowNotification(Text_moneyAmount, "Good Luck", 1);
            Text_moneyAmount.text = string.Format("${0}", currentChipAmount);
        }

        betAmount = 0;
        hasBet = false;
        hasSelectedColor = false;
        selectedColor = -1;
    }

    IEnumerator ShowNotification(TextMeshProUGUI targetUI, string message, int time)
    {
        string prevText = targetUI.text;
        targetUI.text = message;
        yield return new WaitForSeconds(time);
        targetUI.text = prevText;
    }
}
