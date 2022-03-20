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
    #region Network
    ExitGames.Client.Photon.Hashtable dealerProperties = new ExitGames.Client.Photon.Hashtable();
    #endregion

    #region Color Reveal
    public GameObject colorBox;
    public Material material;
    public int revealedColor;
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
        // Network Communication
        dealerProperties.Add("revealColor", -1);

        // Color Reveal
        material = colorBox.GetComponent<MeshRenderer>().material;
        material.color = Color.grey;

        StartCoroutine(WaitUntilAllBet());
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        // Betting Check
        if(changedProps.ContainsKey("hasBet"))
        {
            Debug.Log("targetPlayer : " + targetPlayer.NickName);

            object hasBet;
            hasBets.Clear();
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                if(player.CustomProperties.TryGetValue("hasBet", out hasBet))
                {
                    hasBets.Add((bool)hasBet);
                }
            }
        }
    }

    IEnumerator WaitUntilAllBet()
    {
        yield return new WaitUntil(() => hasBets.Count == PhotonNetwork.PlayerList.Length && !hasBets.Contains(false));

        photonView.RPC("RevealColor", RpcTarget.Others);
    }

    // REAVEAL THE COLOR
    [PunRPC]
    private void RevealColor()
    {
        revealedColor = UnityEngine.Random.Range(0, 2);
        Debug.Log("revealed color : " + revealedColor);
        dealerProperties["revealedColor"] = revealedColor;

        if (revealedColor == (int)ColorNames.green)
            material.color = Color.green;
        else if (revealedColor == (int)ColorNames.red)
            material.color = Color.red;
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
}
