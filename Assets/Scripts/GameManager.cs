using Google.ProtocolBuffers.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;
using UnityEngine;
using static Valve.VR.SteamVR_TrackedObject;

public class GameManager : MonoBehaviour
{
    // P R O P E R T I E S
    [Header("Score Management")]
    public float TimerLength;
    public Timer RoundTimer;
    public int Score = 0;
    public int HighScore = 0;
    

    [Header("UI References")]
    public TMP_Text TimeDisplay;
    public TMP_Text ScoreText;

    public GameObject EndUITextObjects;
    public TMP_Text EndScoreDisplay;
    public TMP_Text HighScoreDisplay;

    [Header("Object References")]
    public CrocManager crocManager;
    public GameObject RestartThing;

    // U N I T Y  M E T H O D S
    void Awake()
    {
        RoundTimer = new Timer();
    }
    void Start()
    {
        StartGame();
    }
    void Update()
    {
        CheckTimer();
        DisplayUITexts();
    }

    // M I S C  M E T H O D S
    public void StartGame()
    {
        if (EndUITextObjects.active)
        {
            EndUITextObjects.SetActive(false);
        }
        RoundTimer.StartTimer(Time.time, TimerLength);
        crocManager.StartGame();
    }

    public void EndRound()
    {
        crocManager.OnRoundEnd();
        //RoundTimer.ResetTimer();
        RestartThing.SetActive(true);
        SetHighScore();
        SetEndUiContent();
    }

    public void RestartGame()
    {
        Score = 0;
        StartGame();
    }

    void CheckTimer()
    {
        RoundTimer.UpdateTimer(Time.time);

        if (RoundTimer.State == TimerState.Ended)
        {
            EndRound();
        }
    }


    public void IncreaseScore(int _increase)
    {
        Score += _increase;
        UnityEngine.Debug.Log("METHOD SCORE: " +  Score);
    }

    void DisplayUITexts()
    {
        float tempNum = Mathf.Round(TimerLength - RoundTimer.CurrentTime);
        if (tempNum <= 0)
            tempNum = 0;
        TimeDisplay.text = tempNum.ToString();
        ScoreText.text = Score.ToString();
    }

    void SetHighScore()
    {
        if (Score >= HighScore)
            HighScore = Score;
    }
    void SetEndUiContent()
    {
        EndUITextObjects.SetActive(true);
        EndScoreDisplay.text = Score.ToString();
        HighScoreDisplay.text = HighScore.ToString();
    }
}
