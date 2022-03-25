using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Chip : MonoBehaviourPunCallbacks
{
    public PhotonView photonView_player { get; set; }
    public Transform parentObject { get; set; }
    public int chipNumber { get; set; }
    public Color color { get; set; }

    private Vector3 chipPosition = new Vector3(0, 0.1f, 0);
    private Vector3 xVector = new Vector3(0.1f, 0, 0);
    private Vector3 zVector = new Vector3(0, 0, 0.1f);

    private void Start()
    {
        if (photonView.IsMine)
        {
            // Posiiton
            photonView.RPC("PositionChip", RpcTarget.AllBuffered);
            // Color
            photonView.RPC("ColorChip", RpcTarget.AllBuffered);
        }

    }

    [PunRPC]
    private void PositionChip()
    {
        transform.SetParent(parentObject);

        if (chipNumber < 5)
            transform.localPosition = chipPosition - xVector * chipNumber;
        else
            transform.localPosition = chipPosition - zVector - xVector * (chipNumber - 5);
    }

    [PunRPC]
    private void ColorChip()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = color;
    }
}

    
