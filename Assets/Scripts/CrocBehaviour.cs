using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Valve.VR.SteamVR_TrackedObject;

/// <summary>
/// The states a croc can be in.
/// </summary>
public enum CrocState { IsDown, IsMovingDown, IsMovingUp, IsUp, IsHit }

/// <summary>
/// Defines the behavior and actions of the crocs.
/// </summary>
public class CrocBehaviour : MonoBehaviour
{
    /// <summary>
    /// The behavior of the crocs
    ///     - they move up and down when triggered to
    ///     - they handle their own collision
    ///     - they move down when triggered to
    /// 
    /// this script shouldn't have the system for selecting a mole 
    /// </summary>

    // P R O P E R T I E S
    [Header("Movement")]
    [SerializeField] float moveDistance; //distance to move the croc up
    public CrocState State; //the croc's state
    bool isCoroutineRunning; //Used to track whether or not the coroutine animating and moving the crocs has ended or not
    Vector3 originalPosition; // original pos for putting the croc back down

    [Header("Timer")]
    public float timeBeforeDespawnIfNotHit; //amount of time to wait before despawning
    [SerializeField] float minimumTimeBeforeDespawn;
    [SerializeField] float rateOfDecrease;
    Timer despawnTimer; //timer for handling despawing if not hit

    [Header("Animation")]
    [SerializeField] ParticleSystem hitVFX; //the hit particle FX attached to the croc gameObject
    TriggerParticleFX GroundParticleFX; //Script that triggers a particle effect, in this use case it's the ground particle effect
    Animator animator;

    [Header("Audio")]
    [SerializeField] AudioClip[] hitSFX; //array of hit SFX, gets initialized in editor
    [SerializeField] AudioClip[] spawnSFX; //array of spawn SFX, gets initialized in editor
    [SerializeField] AudioClip[] despawnSFX; //array of despawn SFX, gets initialized in editor
    AudioSource audioSource; //reference to the audioSource for playing sound

    [Header("References")]
    [SerializeField] string playerTagName; // the tag the player has (through the liminal tag package)
    [SerializeField] CrocManager crocManager; //[TD] - dependency
    Rigidbody rb; // the rigidbody attached to the gameObj this script is attached to


    // U N I T Y  M E T H O D S
    void Awake()
    {
        despawnTimer = new Timer();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        GroundParticleFX = GetComponent<TriggerParticleFX>();
        isCoroutineRunning = false;
    }
    void OnEnable()
    {
        originalPosition = transform.position; //saves the original position of the croc
    }
    void Update()
    {
        UpdateBasedOnState();
    }
    /// <summary>
    /// Updates the croc based on its state.
    /// </summary>
    void UpdateBasedOnState()
    {
        switch (State)
        {
            case CrocState.IsUp:
                Debug.Log("Croc state = " + State.ToString());
                this.despawnTimer.UpdateTimer(Time.time); //updates timer
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Still")  //|| coroutineState == CoroutineState.Ended || coroutineState == CoroutineState.Off
                    )
                    this.transform.position = originalPosition + new Vector3(0, moveDistance, 0); //corrects position to keep it from drifting 
                break;

            case CrocState.IsDown:
                Debug.Log("Croc state = " + State.ToString());
                if (!isCoroutineRunning)
                {
                    //if (transform.InverseTransformPoint(transform.position).y > 0) 
                    this.transform.position = originalPosition; //corrects position to keep it from drifting 
                }
                break;
        }
    }

    // C O L L I S I O N
    public void OnCollisionEnter(Collision collision)
    {
        // used for making sure the things are hit from above and not from the side
        Vector3 _tempRelativePosition;

        if (TagManager.CompareTags(collision.gameObject, playerTagName)
            && (this.State == CrocState.IsUp || this.State == CrocState.IsMovingUp))
        {
            _tempRelativePosition = transform.InverseTransformPoint(collision.transform.position);
            if (_tempRelativePosition.y > 0)
            {
                // hit behavior
                Debug.Log("Hit!");
                Hit();
            }

            // logic for making sure the croc doesn't get hit from the side when it's down
            if (_tempRelativePosition.y < 1)
            {
                //Debug.Log("The object is to the Below");
                //MoveDown();
                rb.isKinematic = true;
            }
            if (_tempRelativePosition.x > 0)
            {
                //Debug.Log("The object is to the right");
                rb.isKinematic = true;
            }
            if (_tempRelativePosition.x < 0)
            {
                //Debug.Log("The object is to the left");
                rb.isKinematic = true;
            }
            if (_tempRelativePosition.z > 0)
            {
                //Debug.Log("The object is in front.");
                rb.isKinematic = true;
            }

        }
    }


    // A C T I O N S   A N D   B E H A V I O R
    #region 'Actions and Behavior'
    public void PopUp() // Replaces the PopUp() method from MoleController.cs
    {
        Debug.Log("POPPING UP!");
        GroundParticleFX.PlayParticleFX1();
        State = CrocState.IsMovingUp;
        audioSource.PlayOneShot(PickRandomSFXFromArray(spawnSFX));

        StartDespawnTimer(timeBeforeDespawnIfNotHit);
        StartCoroutine(PlayAnimationThenMove("MoveUp", MoveActionType.Up));

        State = CrocState.IsUp;
    }
    public void Despawn()
    {
        if (State == CrocState.IsUp)
        {
            Debug.Log("DESPAWNING");
            GroundParticleFX.PlayParticleFX1();
            State = CrocState.IsMovingDown;
            rb.isKinematic = false;

            audioSource.PlayOneShot(PickRandomSFXFromArray(despawnSFX));
            StartCoroutine(PlayAnimationThenMove("MoveDown", MoveActionType.Down));

            ResetDespawnTimer();


            State = CrocState.IsDown;
        }
    }
    private void Hit()
    {
        if (State == CrocState.IsUp )
        {
            State = CrocState.IsHit;
            Debug.Log("HIT!!");
            hitVFX.Play();
            rb.isKinematic = false;


            audioSource.PlayOneShot(PickRandomSFXFromArray(hitSFX));
            State = CrocState.IsMovingDown;
            StartCoroutine(PlayAnimationThenMove("Hit", MoveActionType.Hit));

            ResetDespawnTimer();

            crocManager.OnCrocHit();

            State = CrocState.IsDown;
        }
    }
    public void Stop()
    {
        Debug.Log("STOPPED!");
        this.transform.position = originalPosition + new Vector3(0, -2, 0);
        this.State = CrocState.IsDown;
        ResetDespawnTimer();
    }


    // M O V E M E N T   A N D    A N I M A T I O N

    /// <summary>
    /// Defines the different types of action movements of the crocs.
    /// </summary>
    private enum MoveActionType { Up, Down, Hit }

    /// <summary>
    /// Coroutine that will play the animation and then move the croc based off the movement type.
    /// </summary>
    /// <param name="animationTriggerName">The name of the animation to trigger.</param>
    /// <param name="action">The type of movement action to be performed.</param>
    /// <returns></returns>
    IEnumerator PlayAnimationThenMove(string animationTriggerName, MoveActionType action)
    {
        isCoroutineRunning = true;
        float secondsToWait = 2;
        switch (action)
        {
            case MoveActionType.Up: secondsToWait = 3; break;
            case MoveActionType.Down: secondsToWait = .7f; break;
            case MoveActionType.Hit: secondsToWait = .75f; break;
        }
        animator.SetTrigger(animationTriggerName); //triggers the animation
        yield return new WaitForSeconds(secondsToWait); //waits 

        switch (action) // handles movement behavior
        {
            case MoveActionType.Up:
                this.transform.position = originalPosition
                    + new Vector3(0, moveDistance, 0); //corrects position to keep it from drifting 
                break;

            case MoveActionType.Down:
            case MoveActionType.Hit:
                transform.position = originalPosition;
                break;
        }
        isCoroutineRunning = false;
    }
    #endregion

    // TIMER MANAGEMENT
    #region 'Timer Management'
    /// <summary>
    /// Starts the despawn timer.
    /// </summary>
    /// <param name="durationInSeconds">The duration of the timer in seconds.</param>
    public void StartDespawnTimer(float durationInSeconds)
    {
        despawnTimer.StartTimer(Time.time, durationInSeconds);
    }

    /// <summary>
    /// Updates the despsawn timer.
    /// </summary>
    public void UpdateDespawnTimer()
    {
        despawnTimer.UpdateTimer(Time.time);
    }

    /// <summary>
    /// Resets the despawn timer.
    /// </summary>
    public void ResetDespawnTimer()
    {
        despawnTimer.ResetTimer();
    }
    /// <summary>
    /// Checks the state of the timer.
    /// </summary>
    /// <param name="state">The state of the timer.</param>
    /// <returns></returns>
    public bool CompareDespawnTimerState(TimerState state)
    {
        if (despawnTimer.State == state)
            return true;

        return false;
    }

    /// <summary>
    /// Decreases the amount of time the croc waits before it despawns.
    /// </summary>
    public void DecreaseDespawnTime()
    {
        Debug.Log("DECREASING DESPAWN TIME");
        timeBeforeDespawnIfNotHit = timeBeforeDespawnIfNotHit / rateOfDecrease;

        if (timeBeforeDespawnIfNotHit < minimumTimeBeforeDespawn)
        {
            Debug.Log("DESPAWN TIME AT MINIMUM");
            timeBeforeDespawnIfNotHit = minimumTimeBeforeDespawn;
        }
            
    }
    #endregion

    // M E T H O D S: MINOR

    /// <summary>
    /// Picks a random SFX from the passed in array
    /// </summary>
    /// <param name="audioClips"> The array of audioclips to pick the sound from.</param>
    /// <returns></returns>
    AudioClip PickRandomSFXFromArray(AudioClip[] audioClips)
    {
        return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
    }

    

    
    // the original MoleController.cs had timers in it that weren't actually 
    // being used, so that logic (the TickTimers() method) has been removed
}
