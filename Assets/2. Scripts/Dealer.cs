using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
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
    public Hashtable dealerProperties = new Hashtable();
    #endregion

    #region Color Reveal
    public GameObject colorBox;
    public Material material;
    private int revealedColor;
    #endregion

    #region Player
    List<bool> hasBets = new List<bool>();
    #endregion

    private void Start()
    {
        // Network Communication
        dealerProperties.Add("revealedColor", -1);
        dealerProperties.Add("hasRevealedColor", false);

        // Color Reveal
        material = colorBox.GetComponent<MeshRenderer>().material;
        material.color = Color.grey;

        // Start Coroutine
        StartCoroutine(WaitUntilAllBet());
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    { 
        // Betting Check
        if(changedProps.ContainsKey("hasBet"))
        {
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

        if(changedProps.ContainsKey("hasReceivedColor") && (bool)changedProps["hasReceivedColor"] == true)
        {
            dealerProperties["hasRevealedColor"] = false;
            PhotonNetwork.CurrentRoom.SetCustomProperties(dealerProperties);
        }
    }

    public IEnumerator WaitUntilAllBet()
    {
        yield return new WaitUntil(() => hasBets.Count == PhotonNetwork.PlayerList.Length && !hasBets.Contains(false));

        RevealColor();

        dealerProperties["hasRevealedColor"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(dealerProperties);

        photonView.RPC("AnnounceColor", RpcTarget.All);
    }

    // REAVEAL THE COLOR
    private void RevealColor()
    {
        Debug.Log("RevealColor()");
        revealedColor = UnityEngine.Random.Range(0, 2);

        dealerProperties["revealedColor"] = revealedColor;
        PhotonNetwork.CurrentRoom.SetCustomProperties(dealerProperties);

        for(int i = 0; i < hasBets.Count; i++)
        {
            hasBets[i] = false;
        }
    }

    [PunRPC]
    private void AnnounceColor()
    {
        Debug.Log("AnnounceColor : " + revealedColor);
        if (revealedColor == (int)ColorNames.green)
            material.color = Color.green;
        else if (revealedColor == (int)ColorNames.red)
            material.color = Color.red;
    }
}
