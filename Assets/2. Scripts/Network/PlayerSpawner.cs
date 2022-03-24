using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner playerSpawner;

    private void OnEnable()
    {
        if (playerSpawner == null)
            PlayerSpawner.playerSpawner = this;

        GameObject playerInstance = PhotonNetwork.Instantiate("player", Vector3.zero, Quaternion.identity);
    }
}
