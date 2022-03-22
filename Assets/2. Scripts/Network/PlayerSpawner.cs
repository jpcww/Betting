using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner playerSpawner;

    public Transform[] seats;
    private Dealer dealer;

    private void OnEnable()
    {
        if (playerSpawner == null)
            PlayerSpawner.playerSpawner = this;

        dealer = PhotonView.Find(1).gameObject.GetComponent<Dealer>();

        GameObject playerInstance = PhotonNetwork.Instantiate("player", Vector3.zero, Quaternion.identity);
        playerInstance.transform.SetParent(seats[playerInstance.GetPhotonView().ViewID%3].transform);
        playerInstance.transform.localPosition = Vector3.zero;
        playerInstance.transform.LookAt(dealer.transform);
        playerInstance.GetComponentInChildren<Camera>().enabled = true;
        playerInstance.GetComponentInChildren<Canvas>().enabled = true;
    }

}
