using Google.ProtocolBuffers.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using static Valve.VR.SteamVR_TrackedObject;

public class SelectPopupMole : MonoBehaviour
{
    public GameObject[] molePrefab; //holds all the pirate moles


    public bool[] selectedmoles; //create pirates and check using boolean value to determine which pirate is selected

    public float timeLeft = 10f; // time left untile nexty mole spawns
    public int amountOfPopUps = 1; // how many pirates pop up at a time

    public Material moleSkin; //temp material set // change to MESH
    public Material setMaterial; //replace with mesh renderer material for 3d renders

    //HitSounds
    AudioSource hitSound;
    bool s_Play;
    bool s_ToggleChange;


    // Start is called before the first frame update
    void Start()
    {
        hitSound = GetComponent<AudioSource>();
        Array.Resize(ref selectedmoles, molePrefab.Length);


        SelectNextMole();

    }

    // Update is called once per frame
    void Update()
    {
        CountDown();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (Input.GetMouseButton(0))
            {
                if (hit.collider.tag == "moles")
                {

                    for (int i = 0; i < selectedmoles.Length; i++)
                    {
                        if (selectedmoles[i] == true)
                        {
                            if (molePrefab[i].GetComponent<Collider>() == hit.collider)
                            {
                                s_Play = true;
                                molePrefab[i].GetComponent<MoleController>().Hit();
                                molePrefab[i].GetComponent<Renderer>().material = moleSkin;
                                selectedmoles[i] = false;
                                break; // Exit the loop once a mole is hit
                            }
                        }
                    }
                    s_Play = false;
                    SelectNextMole();

                }

            }

        }

        PlayHitSoundEffect();
    }

    public void MoleHitHammer(int i)
    {

        molePrefab[i].gameObject.tag = "moles";
        molePrefab[i].GetComponent<Renderer>().material = moleSkin;
        selectedmoles[i] = false;
        SelectNextMole();

    }
    public void SelectNextMole()
    {

        // This value runs the bool statement
        bool run = true;

        while (run)
        {
            if (CheckActiveMoles())
            {
                MoleSelect();

            }
            else
            {
                run = false;
            }
        }

    }
    public void MoleSelect()
    {
        for (int i = 0; i < amountOfPopUps; i++)
        {


            int randomMole;
            do
            {
                randomMole = UnityEngine.Random.Range(0, selectedmoles.Length);
            } while (selectedmoles[randomMole]);

            selectedmoles[randomMole] = true; // change value to true //random mole selected

            popUp(molePrefab[randomMole]); //run popUp function on selected mole
            molePrefab[randomMole].gameObject.tag = "SelectedMole";
            molePrefab[randomMole].GetComponent<Renderer>().material = setMaterial;


        }


    }
    public bool CheckActiveMoles()
    {

        int temp = 0;

        for (int i = 0; i < selectedmoles.Length; i++)
        {
            if (selectedmoles[i] == true)
            {
                temp = temp + 1;
            }
        }

        if (temp < amountOfPopUps)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void PlayHitSoundEffect()
    {
        //Check to see if you just set the toggle to positive
        if (s_Play == true && s_ToggleChange == true)
        {
            //Play the audio you attach to the AudioSource component
            hitSound.Play();
            //Ensure audio doesn’t play more than once
            s_ToggleChange = false;
        }
        //Check if you just set the toggle to false
        if (s_Play == false && s_ToggleChange == true)
        {
            //Stop the audio
            hitSound.Stop();
            //Ensure audio doesn’t play more than once
            s_ToggleChange = false;
        }
    }


    public void popUp(GameObject Mole)
    {

        Mole.GetComponent<MoleController>().Popup();
    }

    void CountDown()
    {
        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            if (amountOfPopUps >= 2)
            {
                amountOfPopUps = 2;
            }
            else
            {
                amountOfPopUps++;
            }
            timeLeft += 10f;
        }
    }
}
