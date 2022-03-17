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
    public bool hasSelectedColor = false;
    public int currentChipAmount = 100;
    public int defaultBetAmount = 10;
    public int betAmount = 0;
    public bool hasBet = false;
    private Action fillChipsAction;
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
    }

    private void OnEnable()
    {
        if (playerInput == null)
        {
            playerInput = new PlayerInput();

            playerInput.Bet.SelectGreen.performed += i => SelectGreen();
            playerInput.Bet.SelectRed.performed += i => SelectRed();
            playerInput.Bet.AddBet.performed += i => DecideBetAmout();
            playerInput.Bet.ConfirmBet.performed += i => Bet();
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
        hasSelectedColor = true;
    }
    public void SelectRed()
    {
        selectedColor = (int)ColorNames.red;
        hasSelectedColor = true;
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
        if (!hasBet)
        {
            currentChipAmount -= betAmount;
            Text_betAmount.text = "Bet";
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

    IEnumerator ShowNotification(TextMeshProUGUI targetUI, string message, int time)
    {
        string prevText = targetUI.text;
        targetUI.text = message;
        yield return new WaitForSeconds(time);
        targetUI.text = prevText;
    }
}
