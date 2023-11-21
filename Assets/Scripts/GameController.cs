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
    public float time = 0f;
    public int phases = 0;
    private bool isTimer = false;

    //UI
    public TMP_Text timeNum;
    public Text scoreTxt;
    //FONT https://www.1001fonts.com/digital-dismay-font.html

    // Start is called before the first frame update
    void Start()
    {

        //timeNum.text = time.ToString();
        isTimer = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        //scoreTxt.text = score.ToString();
        if (isTimer)
        {
            time -= Time.deltaTime;
            DisplayTime();
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
    void DisplayTime()
    {
        int min = Mathf.FloorToInt(time/60.0f);
        int sec = Mathf.FloorToInt(time - min * 60);
        timeNum.text = string.Format("{0:00}:{1:00}", min, sec);
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
