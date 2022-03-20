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
    //#region Network
    private ExitGames.Client.Photon.Hashtable dealerProperties = new ExitGames.Client.Photon.Hashtable();
    //#endregion

    #region Color Reveal
    public GameObject colorBox;
    public Material material;
    private int revealedColor;
    #endregion

    #region Player
    List<bool> hasBets = new List<bool>();
    #endregion

    #region UI
    public TextMeshProUGUI text_currentAmount;
    public TextMeshProUGUI text_betAmount;
    #endregion

    private void Start()
    {
        // Network Communication
        dealerProperties.Add("revealedColor", -1);

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
        RevealColor();
        photonView.RPC("AnnounceColor", RpcTarget.All);
    }

    // REAVEAL THE COLOR
    private void RevealColor()
    {
        revealedColor = UnityEngine.Random.Range(0, 2);

        dealerProperties["revealedColor"] = revealedColor;
        PhotonNetwork.SetPlayerCustomProperties(dealerProperties);
    }

    [PunRPC]
    private void AnnounceColor()
    {
        if (revealedColor == (int)ColorNames.green)
            material.color = Color.green;
        else if (revealedColor == (int)ColorNames.red)
            material.color = Color.red;
    }
}
