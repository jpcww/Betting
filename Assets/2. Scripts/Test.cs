using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Prepare the values in advance to avoid using "new" in Update()
    private Vector3 origin = Vector3.zero;
    private Vector3 moveAmount = Vector3.one;

    Test_ObjectManager Test_ObjectManager;

    List<GameObject> sceneObjects = new List<GameObject>();

    private void Awake()
    {
        Test_ObjectManager = FindObjectOfType<Test_ObjectManager>();
        Test_ObjectManager.gameObjectChanged += UpdateGameObjects;
    }

    private void Start()
    {
        UpdateGameObjects();
    }

    private void Update()
    {
        if (sceneObjects != null)
        {
            foreach (GameObject i in sceneObjects)
            {
                if (Vector3.Distance(i.transform.position, origin) > 5)
                {
                    i.transform.position = origin;
                }

                else
                {
                    i.transform.position += moveAmount;
                }
            }
        }
    }

    private void UpdateGameObjects()    // Search for the game objects only where there are changes
    {
        sceneObjects.AddRange(GameObject.FindGameObjectsWithTag("scene"));
    }
}
