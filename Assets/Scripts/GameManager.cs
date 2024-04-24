using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using static Valve.VR.SteamVR_TrackedObject;

public class GameManager : MonoBehaviour
{
    // P R O P E R T I E S
    [Header("State Management")]
    [SerializeField] private GameStates State;

    [Header("Score Management")]
    [SerializeField] float StartTimerLength;
    private Timer IntroTimer;
    [SerializeField] float RoundTimerLength;
    public Timer RoundTimer;
    public int Score = 0;
    public int HighScore = 0;


    [Header("UI References")]
    [SerializeField] private GameObject IntroUIObject;
    [SerializeField] private TMP_Text IntroTimerDiplay;

    [SerializeField] private GameObject GamePlayUiObject;
    [SerializeField] TMP_Text TimeDisplay;
    [SerializeField] TMP_Text ScoreText;
    [SerializeField] TMP_Text EndScoreText;

    [SerializeField] private GameObject EndUITextObjects;

    [Header("Audio Refs")]
    [SerializeField] private AudioSource startRoundSFX;
    [SerializeField] private AudioSource endRoundSFX;

    [Header("Object References")]
    [SerializeField] private CrocManager crocManager;

    private enum GameStates { Starting, Playing, Ending }

    // U N I T Y  M E T H O D S
    void Awake()
    {
        State = GameStates.Starting;
        IntroTimer = new Timer();
        RoundTimer = new Timer();

        GamePlayUiObject.SetActive(false);
    }
    void Start()
    {
        StartIntro();
    }
    void Update()
    {
        switch (State)
        {
            case GameStates.Starting:
                PlayIntro();
                break;

            case GameStates.Playing:
                UpdateGamePlay();
                break;

            case GameStates.Ending:
                // game end behavior
                break;
        }
    }

    // M I S C  M E T H O D S
    private void StartIntro()
    {
        IntroTimer.StartTimer(Time.time, StartTimerLength);
        IntroUIObject.SetActive(true);

    }
    private void PlayIntro()
    {
        DisplayIntroUITexts();
        CheckIntroTimer();
    }
    private void EndIntro()
    {
        startRoundSFX.Play();
        new WaitForSeconds(3);
        IntroUIObject.SetActive(false);
        IntroTimer.State = TimerState.Off;
    }
    private void StartGamePlay()
    {
        GamePlayUiObject?.SetActive(true);
        if (RoundTimerLength == 0)
            RoundTimerLength = 15;

        if (EndUITextObjects.activeSelf)
        {
            EndUITextObjects.SetActive(false);
        }
        RoundTimer.StartTimer(Time.time, RoundTimerLength);
        crocManager.StartGame();
    }
    private void UpdateGamePlay()
    {
        CheckGameplayTimer();
        DisplayGameplayUITexts();
    }

    public void EndRound()
    {
        crocManager.OnRoundEnd();
        RoundTimer = new Timer();
        //SetHighScore();
        endRoundSFX.Play();
        SetEndUiContent();
    }

    public void RestartGame()
    {
        //ExitThing.SetActive(false);

        Score = 0;
        StartGamePlay();
    }

    void CheckGameplayTimer()
    {
        RoundTimer.UpdateTimer(Time.time);

        if (RoundTimer.State == TimerState.Ended)
        {
            State = GameStates.Ending;
            EndRound();
        }
    }
    void CheckIntroTimer()
    {
        IntroTimer.UpdateTimer(Time.time);
        
        if (IntroTimer.State == TimerState.Ended)
        {
            State = GameStates.Playing;
            EndIntro();
            StartGamePlay();
        }
    }

    public void IncreaseScore(int _increase)
    {
        Score += _increase;
        //UnityEngine.Debug.Log("METHOD SCORE: " +  Score);
    }

    void DisplayGameplayUITexts()
    {
        float tempNum = Mathf.Round(RoundTimer.EndTime - RoundTimer.CurrentTime);
        if (tempNum <= 0)
            tempNum = 0;
        TimeDisplay.text = tempNum.ToString();
        ScoreText.text = Score.ToString();
    }
    void DisplayIntroUITexts()
    {

        if (IntroTimer.CurrentTime >= IntroTimer.EndTime - 5) //only show the count down to start time if there are 5 seconds left on the clock
        {
            float tempNum = Mathf.Round(IntroTimer.EndTime - IntroTimer.CurrentTime);
            if (tempNum <= 0)
                tempNum = 0;

            IntroTimerDiplay.text = tempNum.ToString();
        }
        else { IntroTimerDiplay.text = string.Empty; }


        
    }

    void SetHighScore()
    {
        if (Score >= HighScore)
            HighScore = Score;
    }
    void SetEndUiContent()
    {
        GamePlayUiObject.SetActive(false);
        EndUITextObjects.SetActive(true);
        EndScoreText.text = Score.ToString();
    }
}
