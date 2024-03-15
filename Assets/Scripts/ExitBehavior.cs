using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Liminal.SDK.Core;
using Liminal.Core.Fader;

public class ExitBehavior : MonoBehaviour
{
    // P R O P E R T I E S
    public string PlayerTagName;
    [SerializeField] EndToggle endToggle;

    enum EndToggle { LiminalWay , UnityWay }

    public void OnCollisionEnter(Collision collision)
    {
        if ((TagManager.CompareTags(collision.gameObject, PlayerTagName)))
        {
            switch (endToggle)
            {
                case EndToggle.LiminalWay:
                    EndGameLiminalWay();
                    break;

                case EndToggle.UnityWay:
                    EndGameUnityWay();
                    break;
            }
            EndGameLiminalWay();
        }
    }

    void EndGameLiminalWay() // the way liminal wants its ~eXpErIeNcEs~ to  be ended
    {
        StartCoroutine(FadeExit(2f)); // starts coroutine

        // the coroutine
        IEnumerator FadeExit(float fadeTime)
        {
            float _elapsedTime = 0f; // start time
            var _startingVolume = AudioListener.volume; // start volume

            ScreenFader.Instance.FadeTo(Color.black, fadeTime); // fades the screen to black over x amount of seconds

            while (_elapsedTime < fadeTime) // while loop for fading the volume
            {
                _elapsedTime += Time.deltaTime;
                AudioListener.volume = Mathf.Lerp(_startingVolume, 0, _elapsedTime / fadeTime);
                yield return new WaitForEndOfFrame();
            }

            AudioListener.volume = 0f; // sets volume to 0 for the end
            ExperienceApp.End(); //ends game

        }
    }

    void EndGameUnityWay()
    {
        Application.Quit();
    }

    
}
