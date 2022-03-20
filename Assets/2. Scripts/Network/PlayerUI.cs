using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TextMeshProUGUI text_playerName;
    public TextMeshProUGUI text_currentAmount;
    public TextMeshProUGUI text_betAmount;

    private PlayerManager playerManager;

    private void Awake()
    {
        GetComponent<Transform>().SetParent(GameObject.FindGameObjectWithTag("Canvas").GetComponent<Transform>());
    }

    private void Update()
    {
        if(playerManager == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void SetPlayer(PlayerManager playerManager, string playerName)
    {
        if (playerManager == null)
        {
            return;
        }

        this.playerManager = playerManager;

        if (text_playerName != null)
        {
            text_playerName.text = playerName;// TODO: check if the name is right
        }
    }
}
