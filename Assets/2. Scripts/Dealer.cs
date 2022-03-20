using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;

public enum ColorNames
{
    green,
    red
}

public class Dealer : MonoBehaviourPunCallbacks
{
    #region Color Reveal
    public GameObject colorBox;
    public Material material;
    public int revealedColor;
    #endregion

    #region Event
    public delegate void UIEvent(TextMeshProUGUI text, string message);
    public delegate IEnumerator WarningEvent(TextMeshProUGUI text, string message);
    public UIEvent uiEvent;
    public WarningEvent warningEvent;
    public Action betAction;
    #endregion

    #region Player
    List<PlayerManager> playerInstances = new List<PlayerManager>();  // make it as a list in case players join or leave in the middle
    List<bool> hasBets = new List<bool>();
    #endregion

    #region UI
    public TextMeshProUGUI text_currentAmount;
    public TextMeshProUGUI text_betAmount;
    #endregion

    private void Start()
    {
        material = colorBox.GetComponent<MeshRenderer>().material;
        material.color = Color.grey;

        // Event Subscription
        uiEvent += UpdateUI;
        warningEvent += ShowNotification;
        betAction += OnPlayerBet;
    }

    private void OnPlayerBet()
    {
        playerInstances.Clear();
        playerInstances.AddRange(FindObjectsOfType<PlayerManager>());

        hasBets.Clear();
        foreach (PlayerManager playerInstance in playerInstances)
        {
            Debug.Log(playerInstance.photonView.ViewID + " " +playerInstance.hasBet);
            hasBets.Add(playerInstance.hasBet);
        }

        if (hasBets.Contains(false))
        {
            return;
        }

        RevealColor();
        ProcessWinLose();
    }
   
    // REAVEAL THE COLOR
    private void RevealColor()
    {
        revealedColor = UnityEngine.Random.Range(0, 2);

        // Change the color
        if (revealedColor == (int)ColorNames.green)
        {
            material.color = Color.green;
        }

        else
        {
            material.color = Color.red;
        }
    }

    // PROCESS WIN/LOSE
    private void ProcessWinLose()
    {
        foreach(PlayerManager playerInstance in playerInstances)
        {
            if (playerInstance.selectedColor == revealedColor)
                playerInstance.EarnChips();
            else
                playerInstance.LoseChips();
        }

        playerInstances.Clear();
        hasBets.Clear();
    }

    private void UpdateUI(TextMeshProUGUI text, string message)
    {
        text.text = message;
    }

    IEnumerator ShowNotification(TextMeshProUGUI targetUI, string message)
    {
        string prevText = targetUI.text;
        targetUI.text = message;
        yield return new WaitForSeconds(1);
        targetUI.text = prevText;
    }
}
