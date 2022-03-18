using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public enum ColorNames
{
    green,
    red
}

public class Dealer : MonoBehaviourPunCallbacks
{
    #region Bet
    private bool allBet = true;
    #endregion

    #region Color Reveal
    public GameObject colorBox;
    public Material material;
    public int revealedColor;
    #endregion

    #region Player
    List<PlayerInstance> playerInstances = new List<PlayerInstance>();
    #endregion

    private void Start()
    {
        material = colorBox.GetComponent<MeshRenderer>().material;
        material.color = Color.grey;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log("targetPlayer : " + " " + targetPlayer + " changed props" + changedProps.ToString());

        playerInstances.Clear();
        playerInstances.AddRange(FindObjectsOfType<PlayerInstance>());

        foreach (PlayerInstance playerInstance in playerInstances)
        {
            Debug.Log("player instance count : " + playerInstances.Count);
            if (playerInstance.hasBet == false)
                allBet = false;
        }

        if (!allBet)
        {
            Debug.Log("allbet false");
            return;
        }
        else if(allBet)
        {
            Debug.Log("ready to reveal");
            RevealColor();
            ProcessWinLose();
        }


        /*
        if (!changedProps.ContainsKey("hasBet"))
        {
            Debug.Log("no hasBet");
            return;
        }

        else
        {
            Debug.Log("hasBet");
            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                if((bool)player.CustomProperties["hasBet"] == false)
                {
                    allBet = false;
                }
            }

            if (allBet == false)
                return;

            else
            {
                Debug.Log("ready to reveal");
                RevealColor();
                ProcessWinLose();
            }
        }
        */
    }

    // REAVEAL THE COLOR
    private void RevealColor()
    {
        Debug.Log("reveal color");
        revealedColor = Random.Range(0, 2);
        Debug.Log("revealed color : " + revealedColor);

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
        playerInstances.Clear();
        playerInstances.AddRange(FindObjectsOfType<PlayerInstance>());

        foreach(PlayerInstance playerInstance in playerInstances)
        {
            if (playerInstance.selectedColor == revealedColor)
                playerInstance.EarnChips();
            else
                playerInstance.LoseChips();
        }
    }
}
