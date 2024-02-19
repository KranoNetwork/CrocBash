using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScript : MonoBehaviour
{
    float fadeTime;
    float currentDelayTime;
    public float fadeRate;
    public float fadeDelay;
    public float timeInbetween;
    bool fadingIn;
    bool delaying;
    bool fadingOut;

    public CanvasGroup CanvasGroup;
    public GameObject RHand;
    public GameObject LHand;


    // Start is called before the first frame update
    void Start()
    {

        CanvasGroup = gameObject.GetComponent<CanvasGroup>();
        RHand.SetActive(false);
        LHand.SetActive(false);
        CanvasGroup.alpha = 0;
        delaying = true;
        if (CanvasGroup == null)
        {
            Debug.LogWarning("Canvas Group is null");


        }

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKey(KeyCode.S))
        //{
        //    fadingIn = true;
        //}


        if (fadingOut)
        {
            FadeOut();
        }

        if (fadingIn)
        {
            FadeIn();
        }

        if (delaying)
        {
            Delay();
        }
    }

    public void FadeOut()
    {
        //CanvasGroup.alpha = 1;

        fadeTime -= fadeRate;
        CanvasGroup.alpha = fadeTime;

        if (fadeTime <= 0)
        {
            delaying = false;
            fadingOut = false;
        }

    }

    public void FadeIn()
    {
        fadeTime += fadeRate;
        CanvasGroup.alpha = fadeTime;
        RHand.SetActive(true);
        LHand.SetActive(true);
        if (fadeTime >= 1)
        {
            fadingIn = false;
        }
    }

    public void Delay()
    {
        currentDelayTime += 1;

        if (currentDelayTime > fadeDelay)
        {
            fadeDelay += timeInbetween;

            if (CanvasGroup.alpha > 0)
            {
                fadingOut = true;
            }
            else
            {
                fadingIn = true;
            }


        }
        //lean tween
    }
}
