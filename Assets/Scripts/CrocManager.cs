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
    GameObject selectedCroc; // for picking a croc and triggering it to move up; MAY NOT BE NEEDED IDK YET

    [Header("Timer")]
    public Timer SpawnCrocTimer; // timer for handling when to pop up the next croc
    public int AmountOfPopUps; // number of crocs that can be up at once

    [Header("References")]
    [SerializeField] public GameObject[] Crocs; // list of crocs for reference

    // U N I T  M E T H O D S 
    void Awake()
    {
        SpawnCrocTimer = new Timer();
        // initialise things 
    }

    // Start is called before the first frame update
    void Start()
    {
        //start game 
    }

    // Update is called once per frame
    void Update()
    {
        //update the timer

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

    public void OnCrocHit() // replaces MoleHitHammer(int i)
    {
        // the croc already plays sound so this doesn't need too
        
        // select the next croc to pop up
    }

    void SelectNextCrocIfGameIsRunning() // replaces SelectNextMole()
    {
        // while the game is running, 
           /*  - check if there are active crocs
            *       -- if there are active crocs, 
            *           -- select a croc
            * 
            */
    }

    void SelectCrocs() // replaces MoleSelect() from SelectPopUpMole.cs
    {
        // select a croc

        // if AmountOfPopUps != 0
            // -- spawn multiple crocs
    }

    public bool CheckActiveCrocs() // basically the same as CheckActiveMoles()
    {
        int temp = 0;

        for (int i = 0; i < Crocs.Length; i++)
        {
            if (Crocs[i] == true)
            {
                temp = temp + 1;
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
        //if the timer has ended 
            // adjust AmountOfPopUps value (but don't let it go above 2)
            // reset the timer
    }


}
