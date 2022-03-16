using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_ObjectManager : MonoBehaviour
{
    public GameObject go;
    public Action gameObjectChanged;


    private void CreateGameObject()
    {
        // create a gameobject
        gameObjectChanged();
    }

    private void DestroyGameObject()
    {
        // destory a gameobject
        gameObjectChanged();
    }
}
