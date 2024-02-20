using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Timer")]
    public Timer PopMoreCrocTimer; // timer for handling when to pop up the multiple crocs
    [SerializeField] float numOfSecsBeforeMoreCrocsPopUpAtOnce;
    public int AmountOfPopUps; // number of crocs that can be up at once

    [Header("References")]
    [SerializeField] public GameObject[] Crocs; // list of crocs for reference
    [SerializeField] GameController gameController;

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
        StartGame();
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
    public void OnCrocHit() // replaces MoleHitHammer(int i)
    {
        // the croc already plays sound so this doesn't need too
        // the croc already updates it's state so this does't need to do it


        gameController.IncreaseScore(1); 


        // select the next croc to pop up
        SelectNextCroc();
    }

    void SelectNextCroc() // replaces SelectNextMole()
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
            if (CheckActiveCrocs())
            {
                SelectCrocs();
            }
            else
            {
                _run = false;
            }
        }
    }

    void SelectCrocs() // replaces MoleSelect() from SelectPopUpMole.cs
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
                    break;
            } while (_tempCBref.State == CrocState.IsUp); // do those things will the croc is up/selected

            Crocs[_randomNum].GetComponent<CrocBehaviour>().SetCrocState(CrocState.IsMovingUp);
            PopUp(Crocs[_randomNum]);
        }
    }

    public bool CheckActiveCrocs() // basically the same as CheckActiveMoles()
    {
        /// <summary>
        /// checks to see if there are selected crocs/ crocs that are up
        /// </summary>
        int temp = 0;

        foreach (GameObject croc in Crocs)
        {
            CrocBehaviour tempCBref = croc.GetComponent<CrocBehaviour>();

            if (tempCBref != null ) // making sure there is a value
            {
                if (tempCBref.State == CrocState.IsUp) // checking the state of the croc
                    temp += 1;
            }
        }

        if (temp < AmountOfPopUps)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PopUp(GameObject _croc) // replaces popUp(GameObject Mole)
    {
        CrocBehaviour tempCBscript = _croc.GetComponent<CrocBehaviour>();

        if (tempCBscript != null)
        {
            _croc.GetComponent<CrocBehaviour>().MoveUp();
        }
    }

    // don't need PlaySoundEffect() since the crocs already play the sound effect

    void CountDown() // replaces CountDown()
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


}
