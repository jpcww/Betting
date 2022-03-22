using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public enum Colors
{
    green,
    red,
}

public class Dealer : MonoBehaviourPunCallbacks, IOnEventCallback
{
    #region Network
    public Hashtable dealerProperties = new Hashtable();

    public const byte PlayerInitiatedEventCode = 1;
    #endregion

    #region Color Reveal
    public GameObject colorBox;
    public Material material;
    private int revealedColor;
    #endregion

    #region Player
    List<bool> hasBets = new List<bool>();
    #endregion

    #region Coin
    private Vector3 coinPosition = new Vector3(0, 0.1f, 0);
    private Vector3 xVector = new Vector3(0.1f, 0, 0);
    private Vector3 zVector = new Vector3(0, 0, 0.1f);
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

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == PlayerInitiatedEventCode)
        {
            int playerPhotonViewID = (int)photonEvent.CustomData;
            InstantiateChips(playerPhotonViewID);
        }
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
        if (revealedColor == (int)Colors.green)
            material.color = Color.green;
        else if (revealedColor == (int)Colors.red)
            material.color = Color.red;
    }

    public void InstantiateChips(int playerPhotonViewID)
    {
        Debug.Log("playerPhotonViewID : " + playerPhotonViewID);
        // Access Player
        PhotonView playerPhotonView = PhotonView.Find(playerPhotonViewID);
        GameObject playerInstance = playerPhotonView.gameObject;
        Transform transForm_availableChips = playerInstance.transform.GetChild(2);
        Transform transForm_betChips = playerInstance.transform.GetChild(3);
        PlayerManager playerManager = playerInstance.GetComponent<PlayerManager>();

        Color color = new Color();

        for (int i = 0; i < 10; i++)
        {
            // Instantiate
            GameObject myChipStack = PhotonNetwork.InstantiateRoomObject("ChipStack", Vector3.zero, Quaternion.identity, 0, null);
            PhotonView photonView_myChipStack = myChipStack.GetPhotonView();
            photonView_myChipStack.TransferOwnership(playerPhotonView.Owner);

            // Position
            myChipStack.transform.SetParent(transForm_availableChips);

            if (i < 5)
                myChipStack.transform.localPosition = coinPosition - xVector * i;
            else
                myChipStack.transform.localPosition = coinPosition - zVector - xVector * (i - 5);

            // Color
            int colorNumber = UnityEngine.Random.Range(0, 11);
            Renderer myChipRenderer = myChipStack.GetComponent<Renderer>();
            switch (colorNumber)
            {
                case 0:
                    color = Color.green;
                    break;
                case 1:
                    color = Color.red;
                    break;
                case 2:
                    color = Color.blue;
                    break;
                case 3:
                    color = Color.black;
                    break;
                case 4:
                    color = Color.cyan;
                    break;
                case 5:
                    color = Color.white;
                    break;
                case 6:
                    color = Color.gray;
                    break;
                case 7:
                    color = Color.yellow;
                    break;
                case 8:
                    color = Color.magenta;
                    break;
                case 9:
                    color = Color.HSVToRGB(63, 183, 209);
                    break;
            }

            myChipRenderer.material.color = color;

            playerManager.ownedChips.Add(myChipStack);

            // BET CHIPS
            // Instantiate
            GameObject chipStack = PhotonNetwork.InstantiateRoomObject("ChipStack", Vector3.zero, Quaternion.identity, 0, null);
            PhotonView photonView_chipStack = chipStack.GetPhotonView();
            photonView_chipStack.TransferOwnership(playerPhotonView.Owner);

            // Position
            chipStack.transform.SetParent(transForm_betChips);

            if (i < 5)
                chipStack.transform.localPosition = coinPosition - xVector * i;
            else
                chipStack.transform.localPosition = coinPosition - zVector - xVector * (i - 5);

            Renderer betChipRenderer = myChipStack.GetComponent<Renderer>();
            betChipRenderer.material.color = color;

            chipStack.SetActive(false);
            playerManager.betChips.Add(chipStack);
        }
    }

}
