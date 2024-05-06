using Google.ProtocolBuffers.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using static Valve.VR.SteamVR_TrackedObject;

/// <summary>
/// Defines the behavior and actions of the croc manager.
/// </summary>
public class CrocManager : MonoBehaviour
{
    /// <summary>
    /// Manages the crocs
    ///     - triggers them to move 
    ///     - selects the crocs 
    ///     - holds the array of crocs
    ///  
    /// </summary>
    /// 

    // P R O P E R T I E S
    private GameStates gameState;

    [Header("Croc Management")]
    public bool CanSelectCroc; // looks to see if another croc can be selected
    private bool canSpawnNewCroc; //used to check whether or not a new croc can be spawned
    int previousCrocIndex; // for noting what the index of the previous/y selected croc was
    int currentCrocIndex; // for noting what the index of the currently selected croc is


    [Header("Timer")]
    [SerializeField] float timeBeforeCrocsDespawnFaster;
    public int AmountOfPopUps; // number of crocs that can be up at once
    public Timer DecreaseDespawnTimeTimer; // timer for handling when to pop up the multiple crocs

    [Header("References")]
    [SerializeField] public GameObject[] Crocs; // array of crocs for reference
    [SerializeField] GameManager gameManager;

    // U N I T  M E T H O D S 
    void Awake()
    {
        gameState = GameStates.Starting;

        // reference initializations 
        DecreaseDespawnTimeTimer = new Timer();

        // var inits
        AmountOfPopUps = 1;
        timeBeforeCrocsDespawnFaster = 30;
        // initialise things 
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        switch (gameState)
        {
            case GameStates.Playing:
                //update the timer
                CountDown();

                HasADespawnTimerTimedOut();
                PopNewCrocIfAble();
                break;
        }
        /// <summary>
        /// so, the original update method in the SelectPopupMole.cs script
        /// was also checking the collision of the crocs and triggering their hit behavior.
        /// 
        /// This is no longer needed in this script since the crocs
        /// will handle their own collision.
        /// 
        /// 
        /// </summary>

    }

    // C U S T O M  M E T H O D S
    public void StartGame()
    {
        gameState = GameStates.Playing;
        // start game, gameplay will no longer start when
        //  unity calls Start() since the menu and gameplay are in the same scene
        Debug.Log("STARTING GAME");
        DecreaseDespawnTimeTimer.StartTimer(Time.time, timeBeforeCrocsDespawnFaster);
        TriggerNewCrocToPopUp();
    }
    public void OnRoundEnd()
    {
        gameState = GameStates.Ending;
        StopCrocs();
    }


    public void OnCrocHit() // replaces MoleHitHammer(int i)
    {
        // the croc already plays sound so this doesn't need too
        // the croc already updates it's state so this does't need to do it

        gameManager.IncreaseScore(1);
        // trigger the next croc to pop up
        TriggerNewCrocToPopUp();
        //SelectNextCroc();
    }

    #region 'managing the crocs'
    private void SelectNextCroc() // replaces SelectNextMole()
    {
        /* while this method is running, 
         *  - check if there are active crocs
         *       -- if there are not active crocs, 
         *           -- select a croc
         * 
         */

        bool _run = true;
        Debug.Log("SELECT NEXT CROCS WHILE LOOP STARTED");
        while (_run)
        {
            Debug.Log("CHECKING ACTIVE CROCS");
            if (!AreThereActiveCrocs())
            {
                Debug.Log("WE CAN SPAWN A NEW CROC");
                SelectCrocs();
            }
            else
            {
                _run = false;
            }
        }
    }


    private void SelectCrocs() // replaces MoleSelect() from SelectPopUpMole.cs
    {
        Debug.Log("SELECT CROCS STARTED");
        int _randomNum;
        CrocBehaviour _tempCBref;

        /* select a croc

        // if AmountOfPopUps != 0
                -- popup multiple crocs
        */

        for (int i = 0; i < AmountOfPopUps; i++) // do for x amount of times
        {
            /// <summary> 
            /// DO/WHILE description 
            /// 
            /// do:
            ///     1. pick a random number
            ///     2. "select" a croc from the array of crocs using that num
            ///         - specically the CrocBehavior script on that croc
            ///     3. make sure that reference to the CB script is not null
            /// while: 
            ///     the croc state is "IsUp"
            ///     
            /// So basically, this logic picks a random croc from the array
            ///     if that croc is already up (is the state is "IsUp",
            ///         pick a new croc.
            /// It makes sure that the croc that has been selected isn't already up
            /// </summary>
            
            Debug.Log("DO/WHILE STARTED");
            do
            {
                _randomNum = UnityEngine.Random.Range(0, Crocs.Length); 
                _tempCBref = Crocs[_randomNum].GetComponent<CrocBehaviour>(); // gets the CB script for the croc that is selected
            } while (CheckCurrentCroc(_tempCBref, _randomNum)); // do those things while this method returns true

            Debug.Log("SELECTED A CROC SUCCESSFULLY!");
            PopUp(Crocs[_randomNum]);
        }
    }

    private bool CheckCurrentCroc(CrocBehaviour _tempCBref, int _indexNum)
    {
        /// <summary>
        ///  Looks at the selected croc, 
        ///  
        ///  returns true if the CrocState is U 
        ///  or if the crocs is the same as the previous croc
        ///     (as is the same croc got selected)
        ///  
        /// </summary>

        //sets index nums for comparring
        previousCrocIndex = currentCrocIndex;
        currentCrocIndex = _indexNum;

        if (_tempCBref.State == CrocState.IsUp || _tempCBref.State == CrocState.IsHit)
        {
            return true;
        }
        else if (currentCrocIndex == previousCrocIndex)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// checks to see if there are selected crocs/ crocs that are up
    /// </summary>
    private bool AreThereActiveCrocs() // basically the same as CheckActiveMoles()
    {
        
        int temp = 0;

        Debug.Log("CHECK ACTIVE CROCS FOREACH STARTED");
        foreach (GameObject croc in Crocs) //counts up how many cros are currently active
        {
            Debug.Log("GETTING SCRIPT COMPONENT");
            CrocBehaviour tempCBref = croc.GetComponent<CrocBehaviour>();

            if (tempCBref != null ) // making sure there is a value
            {
                Debug.Log("SCRIPT COMPONENT IS NOT NULL");
                if (tempCBref.State == CrocState.IsUp || tempCBref.State == CrocState.IsMovingUp) // checking the state of the croc
                {
                    Debug.Log("CROC STATE =" + tempCBref.State.ToString());
                    temp += 1; //meant to be the amount of crocs currently active
                }
                else { Debug.Log("CROC STATE =" + tempCBref.State.ToString()); }

            }
            else { Debug.Log("SCRIPT COMPONENT IS NULL"); }
        }


        if (temp < AmountOfPopUps) // if the amount of active crocs is less than the amount of pop up allowed, return true
        {
            return true;
        }
        else //if the num crocs is greater than the amount of pop ups, return false
        {
            return false;
        }
    }

    private void PopUp(GameObject _croc) // replaces popUp(GameObject Mole)
    {
        Debug.Log("POPPING SELECTED CROC");

        CrocBehaviour tempCBscript = _croc.GetComponent<CrocBehaviour>();

        if (tempCBscript != null)
        {
            Debug.Log("POPPING A CROC IN PROGRESS");
            _croc.GetComponent<CrocBehaviour>().PopUp();
        }
        else { Debug.Log("TEMPCBREF WAS NULL"); }
    }

    // don't need PlaySoundEffect() since the crocs already play the sound effect

    /// <summary>
    /// Stops the movement behavior of all the crocs.
    /// </summary>
    private void StopCrocs()
    {
        CrocBehaviour _tempCB;
        foreach (GameObject croc in Crocs)
        {
            _tempCB = croc.gameObject.GetComponent<CrocBehaviour>();
            if (_tempCB != null)
                croc.gameObject.GetComponent<CrocBehaviour>().Stop();
        }
    }

    private void DespawnACroc(CrocBehaviour cbTemp)
    {

        cbTemp.Despawn();
    }
    #endregion

    /// <summary>
    /// Ticks the timer. Decreases the despawn time of the crocs if the timer has ended.
    /// </summary>
    private void CountDown() // replaces CountDown()
    {
        // update timer 
        DecreaseDespawnTimeTimer.UpdateTimer(Time.time);

        Debug.Log("DECREASE DESPAWN TIMER STATE:" + DecreaseDespawnTimeTimer.State.ToString());

        if (DecreaseDespawnTimeTimer.State == TimerState.Ended)
        {
            Debug.Log("DECREASE DESPAWN TIMER ENDED!");
            foreach (GameObject croc in Crocs)
            {
                Debug.Log("SELECTED CROC: " + croc.name);
                CrocBehaviour cbTemp = croc.GetComponent<CrocBehaviour>();
                if (cbTemp != null)
                    croc.GetComponent<CrocBehaviour>().DecreaseDespawnTime();
            }
            DecreaseDespawnTimeTimer.StartTimer(Time.time, timeBeforeCrocsDespawnFaster);
        }
        
    }

    /// <summary>
    /// Triggers the next croc to pop up.
    /// </summary>
    public void TriggerNewCrocToPopUp() //public access point
    {
        Debug.Log("NEXT POP UP TRIGGERED");
        canSpawnNewCroc = true;
    }
    /// <summary>
    /// Checks the despawn timers on the crocs
    /// </summary>
    private void HasADespawnTimerTimedOut()
    {
        int countOfTimedOutCrocs = 0;
        foreach (GameObject croc in Crocs)
        {
            CrocBehaviour cbTemp = croc.GetComponent<CrocBehaviour>(); //getting the script

            if (cbTemp != null) //check to make sure it isn't null
            {
                if (cbTemp.CompareDespawnTimerState(TimerState.Ended)   // || cbTemp.CompareDespawnTimerState(TimerState.Off)
                    )
                {
                    Debug.Log("TIMER TIMED OUT, DESPAWNING TIMED OUT CROC");
                    countOfTimedOutCrocs++;
                    cbTemp.Despawn();
                }
            }
        }

        Debug.Log("NUM OF TIMED OUT CROCS = " + countOfTimedOutCrocs);
        if (countOfTimedOutCrocs != 0)
        {
            Debug.Log("TRIGGERING MORE CROCS");
            TriggerNewCrocToPopUp();
        }

    }
    /// <summary>
    /// Pops next crocs(s) if able to
    /// </summary>
    private void PopNewCrocIfAble()
    {
        if (canSpawnNewCroc)
        {
            Debug.Log("SPAWING NEW CROCS");
            SelectCrocs();  
            canSpawnNewCroc = false;
        }
    }

}
