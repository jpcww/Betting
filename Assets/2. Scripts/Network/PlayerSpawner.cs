using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject prefab_dealer;
    public GameObject prefab_player;
    public Transform[] seats;

    private void Start()
    {
        Transform seat = seats[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        PhotonNetwork.Instantiate("Player", seat.position, Quaternion.identity);

        if (FindObjectOfType<Dealer>() == null)
        {
            PhotonNetwork.InstantiateRoomObject("Dealer", new Vector3(0, 0.5f, -0.75f), Quaternion.identity);
        }
    }
}
