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
/// Croc Manager Behavior -> this runs the mechanism that pops up the crocs and pops them down.
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
    #region 'Properties'
    private GameStates gameState;

    [Header("Croc Management")]
    [Tooltip("Variable used to see if another croc can be selected.")]
    public bool CanSelectCroc; 
    private bool canSpawnNewCroc; //used to check whether or not a new croc can be spawned
    int previousCrocIndex; // for noting what the index of the previous/y selected croc was
    int currentCrocIndex; // for noting what the index of the currently selected croc is
    [Tooltip("Number of crocs that can be up at once")]
    public int AmountOfPopUps; 

    [Header("References")]
    [Tooltip("Array of crocs.")]
    [SerializeField] public GameObject[] Crocs; // array of crocs for reference
    [Tooltip("Reference to the Game Manager.")]
    [SerializeField] GameManager gameManager;
    #endregion 

    // U N I T  M E T H O D S 
    #region 'Unity Methods'
    void Awake()
    {
        gameState = gameManager.State;

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
    #endregion

    // G A M E P L A Y   M A N A G E M E N T
    #region 'Gameplay Management'
    /// <summary>
    /// Starts the gameplay -> gets called by the GameManager.
    /// </summary>
    public void StartGame()
    {
        gameState = GameStates.Playing;
        Debug.Log("STARTING GAME");
        TriggerNewCrocToPopUp();
    }
    /// <summary>
    /// Ends the gameplay and moves all the crocs back down -> called by the GameManager.
    /// </summary>
    public void OnRoundEnd()
    {
        gameState = GameStates.Ending;
        StopCrocs();
    }

    /// <summary>
    /// Behavior to perform when a croc is hit -> called by the CrocBehavior script
    /// </summary>
    public void OnCrocHit()
    {
        // increases the score
        gameManager.IncreaseScore(1);
        // trigger the next croc to pop up
        TriggerNewCrocToPopUp();
    }
    #endregion

    // C R O C   M A N A G E M E N T
    #region 'Croc Management'

    /// <summary>
    /// Runs the system that selects the crocs to pop up
    /// </summary>
    private void SelectCrocs() 
    {
        Debug.Log("SELECT CROCS STARTED");
        int _randomNum;
        CrocBehaviour _tempCBref;


        for (int i = 0; i < AmountOfPopUps; i++) // Do this behavior for x amount of times, x is the amount of pop ups there can be at once
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

    /// <summary>
    /// Checks the currently selected croc to see if they can be popped up.
    /// </summary>
    /// <param name="_tempCBref">The reference to the CrocBehavior script of the currently selected croc.</param>
    /// <param name="_indexNum">The index of the currnetly selected croc in the array of crocs.</param>
    /// <returns></returns>
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
    /// Triggers a croc to pop up.
    /// </summary>
    /// <param name="_croc">The reference to the croc to pop up.</param>
    private void PopUp(GameObject _croc) 
    {
        Debug.Log("POPPING SELECTED CROC");

        //gets the croc behavior script off of the croc gameObject
        CrocBehaviour tempCBscript = _croc.GetComponent<CrocBehaviour>();

        //makes sure the croc behavior script isn't null
        if (tempCBscript != null)
        {
            Debug.Log("POPPING A CROC IN PROGRESS");
            _croc.GetComponent<CrocBehaviour>().PopUp();
        }
        else { Debug.Log("TEMPCBREF WAS NULL"); }
    }


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
    

    
    /// <summary>
    /// Triggers the next croc to pop up.
    /// </summary>
    public void TriggerNewCrocToPopUp() //public access point for the CrocBehavior script
    {
        Debug.Log("NEXT POP UP TRIGGERED");
        canSpawnNewCroc = true;
    }
    /// <summary>
    /// Checks the despawn timers on the crocs and despawns them, triggers more crocs to pop up if a croc has despawned.
    /// </summary>
    private void HasADespawnTimerTimedOut()
    {
        bool hasACrocDespawned = false;
        foreach (GameObject croc in Crocs)
        {
            CrocBehaviour cbTemp = croc.GetComponent<CrocBehaviour>(); //getting the script

            if (cbTemp != null) //check to make sure it isn't null to avoid throwing an error
            {
                if (cbTemp.CompareDespawnTimerState(TimerState.Ended)) //checks the despawn timer on the selected croc
                {
                    //if the despawn timer has ended, it despawns the croc
                    Debug.Log("TIMER TIMED OUT, DESPAWNING TIMED OUT CROC");
                    hasACrocDespawned = true; //records if a croc has despawned
                    cbTemp.Despawn();
                }
            }
        }

        //if a croc has despawned, trigger a new croc to pop up
        if (hasACrocDespawned)
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
        //if we can spawn a new croc, spawn a new croc
        if (canSpawnNewCroc)
        {
            Debug.Log("SPAWING NEW CROCS");
            SelectCrocs();
            //resets the bool
            canSpawnNewCroc = false;
        }
    }
    #endregion

}
