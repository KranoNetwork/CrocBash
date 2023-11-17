using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int totalNumOfEnemies = 0; //total number of enemies capable of spawing at once
    public GameObject[] specialEnemies;
    public int amountOfSpecialEnemies;

    public Quaternion PlayerDir; //Directoin player is looking

    public int score = 0;
    public float time = 120f;
    public int phases = 0;

    //UI
    public Text timeNum;
    public Text scoreTxt;


    // Start is called before the first frame update
    void Start()
    {

        timeNum.text = time.ToString();
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreTxt.text = score.ToString();
        if (time > 0)
        {
            time -= Time.deltaTime;
            timeNum.text = time.ToString();
        }
        else
        {
            time = 0;
            timeNum.text = time.ToString();

        }


        switch (phases) 
        {
            case 0:
                default: 
                break;
            case 1:
                break;
            case 2:
                break;
        }

        gameEnd();
    }

    void gameEnd()
    {
        if(time <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            time = 30f;
        }
    }
}
