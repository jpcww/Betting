using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner playerSpawner;

    public Transform[] seats;

    private void OnEnable()
    {
        if (playerSpawner == null)
            PlayerSpawner.playerSpawner = this;

        PhotonNetwork.Instantiate("player", Vector3.zero, Quaternion.identity);
    }

}
