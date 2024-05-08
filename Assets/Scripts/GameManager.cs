using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using static Valve.VR.SteamVR_TrackedObject;

/// <summary>
/// Defines the states of the game/experience
/// </summary>
public enum GameStates { Starting, Playing, Ending }

/// <summary>
/// Defines the behavior and actions of the game manager.
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>
    /// This manages and runs the different "sequences" of the game so to say
    ///  * Runs the intro with the instructions showing 
    ///  * Runs the game play
    ///     * Manages how long the gameplay lasts for 
    ///     * Manages the score
    ///  * Runs the outro showing the "Congrats!" UI and final score
    /// </summary>

    // P R O P E R T I E S
    #region 'Properties'
    [Header("State Management")]
    [Tooltip("The state of the game.")]
    public GameStates State;

    [Header("Score Management")]
    [Tooltip("Duration of the intro sequence.")]
    [SerializeField] float StartTimerLength; 
    private Timer IntroTimer; //timer for the intro
    [Tooltip("Duration of the gameplay sequence.")]
    [SerializeField] float RoundTimerLength; //length of the round -> the amount of time the gameplay runs for 
    public Timer RoundTimer; // timer for the gameplay
    [Tooltip("The score.")]
    public int Score = 0;


    [Header("UI References")]
    [Tooltip("Instructions screen.")]
    [SerializeField] private GameObject IntroUIObject;
    [Tooltip("Text field the time is displayed in.")]
    [SerializeField] private TMP_Text IntroTimerDiplay;

    [Tooltip("Gameplay UI")]
    [SerializeField] private GameObject GamePlayUiObject;
    [Tooltip("Text field the time is displayed in.")]
    [SerializeField] private TMP_Text TimeDisplay;
    [Tooltip("Text field the score is displayed in.")]
    [SerializeField] private TMP_Text ScoreText;

    [Tooltip("Text field the time is displayed in.")]
    [SerializeField] private TMP_Text EndScoreText;
    [Tooltip("End UI")]
    [SerializeField] private GameObject EndUITextObjects;

    [Header("Audio References")]
    [SerializeField] private AudioSource startRoundSFX;
    [SerializeField] private AudioSource endRoundSFX;

    [Header("Object References")]
    [Tooltip("Reference to the Croc Manager.")]
    [SerializeField] private CrocManager crocManager;

    #endregion

    // U N I T Y  M E T H O D S
    #region 'Unity Methods'
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
    #endregion

    // I N T R O   S E Q U E N C E 
    #region 'Intro Methods'
    /// <summary>
    /// Starts the beginning intro with the instructions.
    /// </summary>
    private void StartIntro()
    {
        IntroTimer.StartTimer(Time.time, StartTimerLength);
        IntroUIObject.SetActive(true);

    }
    /// <summary>
    /// Runs the intro according to the timer.
    /// </summary>
    private void PlayIntro()
    {
        DisplayIntroUITexts();
        CheckIntroTimer();
    }
    /// <summary>
    /// Ends the intro sequence.
    /// </summary>
    private void EndIntro()
    {
        startRoundSFX.Play();
        new WaitForSeconds(3);
        IntroUIObject.SetActive(false);
        IntroTimer.State = TimerState.Off;
    }
    /// <summary>
    /// Checks the timer to see if the intro sequence has ended.
    /// </summary>
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
    #endregion

    // G A M E P L A Y   S E Q U E N C E
    #region 'Gameplay Methods'
    /// <summary>
    /// Starts the gameplay.
    /// </summary>
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
    /// <summary>
    /// Runs the gameplay.
    /// </summary>
    private void UpdateGamePlay()
    {
        CheckGameplayTimer();
        DisplayGameplayUITexts();
    }
    /// <summary>
     /// Checks the timer for the gameplay to see if it ended.
     /// </summary>
    void CheckGameplayTimer()
    {
        RoundTimer.UpdateTimer(Time.time);

        if (RoundTimer.State == TimerState.Ended)
        {
            State = GameStates.Ending;
            EndRound();
        }
    }
    /// <summary>
    /// Ends the gameplay.
    /// </summary>
    public void EndRound()
    {
        State = GameStates.Ending;
        crocManager.OnRoundEnd();
        RoundTimer = new Timer();
        //SetHighScore();
        endRoundSFX.Play();
        SetEndUiContent();
    }
    
    #endregion

    // S C O R E   M A N A G E M E N T
    #region 'Score Management'
    public void IncreaseScore(int _increase)
    {
        Score += _increase;
    }
    #endregion

    // U I   M A N A G E M E N T
    #region 'UI Management'
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

    void SetEndUiContent()
    {
        GamePlayUiObject.SetActive(false);
        EndUITextObjects.SetActive(true);
        EndScoreText.text = Score.ToString();
    }
    #endregion
}
