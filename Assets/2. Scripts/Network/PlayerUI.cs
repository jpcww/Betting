using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI text_playerName;
    public TextMeshProUGUI text_currentAmount;
    public TextMeshProUGUI text_betAmount;
    public Image image_selectedcolor;
    public TextMeshProUGUI text_winLose;

    private void Start()
    {
        text_winLose.gameObject.SetActive(false);
    }

    public void SetUserName(string userName)
    {
        text_playerName.text = userName;
    }

    public void SetColor(Color color)
    {
        image_selectedcolor.color = color;
    }

    public void DisplayBetAmount(string message)
    {
        text_betAmount.text = message;
    }

    public void DisplayWarningMessage(string message)
    {
        StartCoroutine(ExchangeMessage(text_betAmount, message));
    }

    public void DisplayerCurrentChipAmount(string message)
    {
        text_currentAmount.text = message;
    }

    public void DisplayWinLose(bool win)
    {
        if(win)
        {
            Debug.Log("Win");
            StartCoroutine(ShowNotification(text_winLose, "Check Your Chips"));
            SetColor(Color.gray);
        }
        else
        {
            Debug.Log("lose");
            StartCoroutine(ShowNotification(text_winLose, "Check Your Chips"));
            SetColor(Color.gray);
        }
    }

    IEnumerator ExchangeMessage(TextMeshProUGUI targetUI, string message)
    {
        string prevText = targetUI.text;
        targetUI.text = message;
        yield return new WaitForSeconds(1);
        targetUI.text = prevText;
    }

    IEnumerator ShowNotification(TextMeshProUGUI targetUI, string message)
    {
        targetUI.gameObject.SetActive(true);
        targetUI.text = message;
        yield return new WaitForSeconds(2);
        targetUI.gameObject.SetActive(false);
    }
}
