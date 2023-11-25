using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.SceneManagement;
using Liminal.Core.Fader;
using Liminal.Platform.Experimental.App.Experiences;
using Liminal.SDK.Core;
using Liminal.SDK.VR;
using Liminal.SDK.VR.Avatars;
using Liminal.SDK.VR.Input;

public class GameController : MonoBehaviour
{
    public int totalNumOfEnemies = 0; //total number of enemies capable of spawing at once
    public GameObject[] specialEnemies;
    public int amountOfSpecialEnemies;

    public Quaternion PlayerDir; //Directoin player is looking

    public int score = 0;
    public float time = 0f;
    public int phases = 0;
    private bool isTimer = false;

    //UI
    public TMP_Text timeNum;
    public Text scoreTxt;
    //FONT https://www.1001fonts.com/digital-dismay-font.html

    // Start is called before the first frame update
    void Start()
    {

        //timeNum.text = time.ToString();
        isTimer = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckVRController();

        //scoreTxt.text = score.ToString();
        if (isTimer)
        {
            time -= Time.deltaTime;
            DisplayTime();
        }

        gameEnd();
    }
    void CheckVRController()
    {
        var avatar = VRAvatar.Active;
        if (avatar == null)
            return;

        var rightInput = GetInput(VRInputDeviceHand.Right);
        var leftInput = GetInput(VRInputDeviceHand.Left);

        // Input Examples
        if (rightInput != null)
        {
            if (rightInput.GetButtonDown(VRButton.Back))
                Debug.Log("Back button pressed");

            if (rightInput.GetButtonDown(VRButton.One))
                Debug.Log("Trigger button pressed");
        }

        if (leftInput != null)
        {
            if (leftInput.GetButtonDown(VRButton.Back))
                Debug.Log("Back button pressed");

            if (leftInput.GetButtonDown(VRButton.One))
                Debug.Log("Trigger button pressed");
        }

        // Any input
        // VRDevice.Device.GetButtonDown(VRButton.One);
    }
    private IVRInputDevice GetInput(VRInputDeviceHand hand)
    {
        var device = VRDevice.Device;
        return hand == VRInputDeviceHand.Left ? device.SecondaryInputDevice : device.PrimaryInputDevice;
    }
    public void StartBTN()
    {
        SceneManager.LoadScene("ArtAssetsTestScene", LoadSceneMode.Additive);
    }
    void DisplayTime()
    {
        int min = Mathf.FloorToInt(time/60.0f);
        int sec = Mathf.FloorToInt(time - min * 60);
        timeNum.text = string.Format("{0:00}:{1:00}", min, sec);
    }
    void gameEnd()
    {
        if(time <= 0)
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //reload the same scene from start
            // go back to main menu screen
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }
}
