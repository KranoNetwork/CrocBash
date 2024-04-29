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

public enum CrocMode { TransformMotion, AnimationMotion }
public class CrocManager : MonoBehaviour
{
    /// <summary>
    /// Manages the crocs
    ///     - triggers them to move up
    ///     - selects the crocs 
    ///     - holds the list of crocs
    ///     
    /// is replacing the SelectPopUpMole.cs script
    /// </summary>
    /// 

    // P R O P E R T I E S
    [Header("Croc Management")]
    public bool CanSelectCroc; // looks to see if another croc can be selected
    List<GameObject> selectedCrocs; // for picking a croc and triggering it to move up; MAY NOT BE NEEDED IDK YET
    int previousCrocIndex; // for noting what the index of the previous croc was
    int currentCrocIndex;
    public CrocMode CrocModeToggle;

    public bool CanSpawnNewCroc;

    [Header("Timer")]
    public Timer PopMoreCrocTimer; // timer for handling when to pop up the multiple crocs
    [SerializeField] float numOfSecsBeforeMoreCrocsPopUpAtOnce;
    public int AmountOfPopUps; // number of crocs that can be up at once

    [Header("References")]
    [SerializeField] public GameObject[] Crocs; // list of crocs for reference
    [SerializeField] GameManager gameManager;

    // U N I T  M E T H O D S 
    void Awake()
    {
        // reference initializations 
        PopMoreCrocTimer = new Timer();
        selectedCrocs = new List<GameObject>();

        // var inits
        AmountOfPopUps = 1;
        numOfSecsBeforeMoreCrocsPopUpAtOnce = 30;
        // initialise things 
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //update the timer
        CountDown();

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
        // start game, gameplay will no longer start when
        //  unity calls Start() since the menu and gameplay are in the same scene
        PopMoreCrocTimer.StartTimer(Time.time, numOfSecsBeforeMoreCrocsPopUpAtOnce);
        SelectNextCroc();
    }
    public void OnRoundEnd()
    {
        StopCrocs();
    }


    public void OnCrocHit() // replaces MoleHitHammer(int i)
    {
        // the croc already plays sound so this doesn't need too
        // the croc already updates it's state so this does't need to do it

        gameManager.IncreaseScore(1);
        // select the next croc to pop up
        SelectNextCroc();
    }
    private void OnCrocDespawn()
    {

    }

    #region 'managing the crocs'
    private void SelectNextCroc() // replaces SelectNextMole()
    {
        /* while this method is running, 
         *  - check if there are active crocs
         *       -- if there are active crocs, 
         *           -- select a croc
         * 
         */

        bool _run = true;
        while (_run)
        {
            Debug.Log("While started");
            if (CheckActiveCrocs())
            {
                SelectCrocs();
                Debug.Log("if started");
            }
            else
            {
                _run = false;
                Debug.Log("else started");
            }
        }
    }


    private void SelectCrocs() // replaces MoleSelect() from SelectPopUpMole.cs
    {
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
            do
            {
                _randomNum = UnityEngine.Random.Range(0, Crocs.Length); 
                _tempCBref = Crocs[_randomNum].GetComponent<CrocBehaviour>(); // gets the CB script for the croc that is selected
                if (_tempCBref == null) // makes sure that script isn't null
                    Debug.Log("THIS IS THE TEMPCBREF YOOOOOO SKRT SKRT:     " + _tempCBref.ToString());
                    break;
            } while (CheckCurrentCroc(_tempCBref, _randomNum)); // do those things while this method returns true

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

        if (_tempCBref.State != CrocState.IsDown || _tempCBref.State != CrocState.IsHit)
        {
            return true;
        }
        else if (currentCrocIndex == previousCrocIndex)
        {
            return true;
        }

        return false;
    }

    private bool CheckActiveCrocs() // basically the same as CheckActiveMoles()
    {
        /// <summary>
        /// checks to see if there are selected crocs/ crocs that are up
        /// </summary>
        int temp = 0;

        foreach (GameObject croc in Crocs) //counts up how many cros are currently active
        {
            Debug.Log("FOREACH STARTED");
            CrocBehaviour tempCBref = croc.GetComponent<CrocBehaviour>();

            if (tempCBref != null ) // making sure there is a value
            {
                if (tempCBref.State != CrocState.IsDown || tempCBref.State != CrocState.IsHit) // checking the state of the croc
                    temp += 1;
            }
        }

        Debug.Log("TEMP VALUE RAHHHHHHHH: " + temp);

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
        CrocBehaviour tempCBscript = _croc.GetComponent<CrocBehaviour>();

        if (tempCBscript != null)
        {
            tempCBscript.GetComponent<CrocBehaviour>().PopUp();
        }
    }

    // don't need PlaySoundEffect() since the crocs already play the sound effect


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
    #endregion

    private void CountDown() // replaces CountDown()
    {
        // update timer 
        PopMoreCrocTimer.UpdateTimer(Time.time);

        //if the timer has ended 
        // adjust AmountOfPopUps value (but don't let it go above 2)
        // reset the timer
        if (PopMoreCrocTimer.State == TimerState.Ended)
        {
            if (AmountOfPopUps >= 2)
            {
                AmountOfPopUps = 2;
            }
            else
            {
                AmountOfPopUps++;
            }
            PopMoreCrocTimer.StartTimer(Time.time, numOfSecsBeforeMoreCrocsPopUpAtOnce);
        }
    }


    public void TriggerNewCrocToPopUp()
    {
        CanSpawnNewCroc = true;
    }
    private bool CanPopNewCroc()
    {
        //if the despawn timers have ended or if a croc has been hit, return true
        //else return false
    }
    private void PopNewCrocIfAble()
    {
        // select croc();
        //start timer
    }

}
