﻿using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMainMenu : MonoBehaviour
{
    // P R O P E R T I E S
    [SerializeField] List<GameObject> mainMenuGameObjects;

    // M E T H O D S
    void Awake()
    {
        mainMenuGameObjects = new List<GameObject>();
    }

    public void OnTriggerEnter(Collider other)
    {
        HideMenuObjects();
    }


    void HideMenuObjects()
    {
        foreach (GameObject gameObject in mainMenuGameObjects)
        {
            gameObject.SetActive(true);
        }
    }
    void ShowMenuObjects()
    {
        foreach (GameObject gameObject in mainMenuGameObjects)
        {
            gameObject.SetActive(false);
        }
    }
}
