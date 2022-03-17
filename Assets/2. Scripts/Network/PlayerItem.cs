﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerItem : MonoBehaviour
{
    public TextMeshProUGUI playerName;

    public void SetPlayerInfo(Photon.Realtime.Player player)
    {
        playerName.text = player.NickName;
    }
}
