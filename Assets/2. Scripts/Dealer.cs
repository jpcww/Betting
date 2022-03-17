using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorNames
{
    green,
    red
}

public class Dealer : MonoBehaviour
{
    public GameObject colorBox;
    public Material material;
    public int revealedColor;

    private void Start()
    {
        material = colorBox.GetComponent<MeshRenderer>().material;
        material.color = Color.grey;
    }


    // CHECK PLAYERS

    // WAIT FOR PLAYERS BETTING

    // REAVEAL THE COLOR
    private void RevealColor()
    {
        revealedColor = Random.Range(0, 2);

        // Change the color
        if (revealedColor == (int)ColorNames.green)
        {
            material.color = Color.green;
        }

        else
        {
            material.color = Color.red;
        }
    }

    // PROCESS WIN/LOSE

    // ANOUNCE THE RESULT


}
