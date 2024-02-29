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
    public int Score = 0;
    public float TimerLength;
    public Timer RoundTimer;

    [Header("UI References")]
    public TMP_Text TimeDisplay;
    public Text ScoreText;

    [Header("Object References")]
    public CrocManager crocManager;

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
        RoundTimer.StartTimer(Time.time, TimerLength);
        crocManager.StartGame();
    }

    public void EndRound()
    {
        crocManager.OnRoundEnd();
        RoundTimer.ResetTimer();
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
    }

    void DisplayUITexts()
    {
        float tempNum = Mathf.Round(TimerLength - RoundTimer.CurrentTime);
        if (tempNum <= 0)
            tempNum = 0;
        TimeDisplay.text = tempNum.ToString();
        ScoreText.text = Score.ToString();
    }
}
