using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Player : MonoBehaviour
{
    #region GameObject
    public int playerID;
    private Dealer dealer;
    #endregion

    #region Bet
    public int selectedColor;
    public bool colorSelected = false;
    public int currentChipAmount = 100;
    public int defaultBetAmount = 10;
    public int betAmount = 0;
    public bool hasBet = false;
    private Action fillChipsAction;
    #endregion

    #region UI
    public TextMeshProUGUI Text_betAmount;
    public TextMeshProUGUI Text_moneyAmount;
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
    }

    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInput();

            playerInput.Bet.SelectGreen.performed += i => SelectGreen();
            playerInput.Bet.SelectRed.performed += i => SelectRed();
            playerInput.Bet.AddBet.performed += i => Bet();
            playerInput.Bet.ConfirmBet.performed += i => Confirm();
        }

        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    public void SelectGreen()
    {
        selectedColor = (int)ColorNames.green;
        colorSelected = true;
    }
    public void SelectRed()
    {
        selectedColor = (int)ColorNames.red;
        colorSelected = true;
    }

    // DECIDE AMOUNT OF BETTING
    public void Bet()
    {
        if (!hasBet && betAmount < currentChipAmount)
        {
            betAmount += defaultBetAmount;
            Text_betAmount.text = string.Format("${0}", betAmount);
        }

        else if(!hasBet && betAmount >= currentChipAmount)
        {
            Text_betAmount.text = "not enough chips!";
        }

        else if(hasBet)
        {
            Text_betAmount.text = "already bet";
        }
    }

    // BET CONFIRM
    public void Confirm()
    {
        if (!hasBet)
        {
            currentChipAmount -= betAmount;
            Text_betAmount.text = "bet";
            Text_moneyAmount.text = string.Format("${0}", currentChipAmount);

            if (currentChipAmount <= 0)
            {
                fillChipsAction();
                //TODO: subscribe somewhere else
            }
        }
        hasBet = true;
    }

    // EARN CHIPS
    public void EarnChips()
    {

    }
    // LOSE CHIPS
    public void LoseChips()
    {

    }
}
