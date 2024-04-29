using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Valve.VR.SteamVR_TrackedObject;

public enum CrocState { IsDown, IsMovingUp, IsUp, IsHit }

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
    [SerializeField] Vector3 originalPosition; // original pos for putting the croc back down
    [SerializeField] float moveDistance; //distance to move the croc up
    [SerializeField] float moveSpeed = 0.1f; //speed at which to move the croc up
    public CrocState State; //the croc state; replaces the bools: isHit, isUp, and isMovingUp in the MoleController script

    [Header("Timer")]
    Timer despawnTimer; //timer for handling despawing if not hit
    public float timeBeforeDespawnIfNotHit; //amount of time to wait before despawning
    public int popCounts; //keeps track of the number of time the croc has popped up -> used for decreasing timeBeforeDespawnIfNotHit so that as the game progresses, the crocs spawn and despawn faster.

    [Header("Animation")]
    [SerializeField] Animator animator;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] hitSFX; //gets initialized in editor
    [SerializeField] AudioClip[] spawnSFX;
    [SerializeField] AudioClip[] despawnSFX;

    [Header("References")]
    [SerializeField] string playerTagName; // the tag the player has (through the liminal tag package)
    Rigidbody rb; // the rigidbody attached to the gameObj this script is attached to

    [SerializeField] CrocManager crocManager; //[TD] - dependency

    // U N I T Y  M E T H O D S
    void Awake()
    {
        despawnTimer = new Timer();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

    }
    void OnEnable()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        UpdateBasedOnState();
    }
    void UpdateBasedOnState()
    {
        switch (State)
        {
            case CrocState.IsUp:
                this.despawnTimer.UpdateTimer(Time.time); //updates timer
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Still")  //|| coroutineState == CoroutineState.Ended || coroutineState == CoroutineState.Off
                    )
                    this.transform.position = originalPosition + new Vector3(0, moveDistance, 0); //corrects position to keep it from drifting 
                break;

            case CrocState.IsDown:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Still")
                   )
                {
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

        //if (collision.gameObject != null)
        //    UnityEngine.Debug.Log($"{this.name} collided with {collision.gameObject.name}");

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

    // M E T H O D S: MAJOR
    public void PopUp() // Replaces the PopUp() method from MoleController.cs
    {
        State = CrocState.IsMovingUp;
        audioSource.PlayOneShot(PickRandomSFXFromArray(spawnSFX));
        popCounts++;

        despawnTimer.StartTimer(Time.time, timeBeforeDespawnIfNotHit);
        StartCoroutine(PlayAnimationThenMove("MoveUp", MoveAnim.Up));

        State = CrocState.IsUp;
    }
    public void Despawn()
    {
        if (State == CrocState.IsUp)
        {
            rb.isKinematic = false;

            audioSource.PlayOneShot(PickRandomSFXFromArray(despawnSFX));
            StartCoroutine(PlayAnimationThenMove("MoveDown", MoveAnim.Down));

            despawnTimer.ResetTimer();

            State = CrocState.IsDown;
        }
    }
    private void Hit()
    {
        if (State == CrocState.IsUp || State == CrocState.IsMovingUp)
        {
            State = CrocState.IsHit;
            rb.isKinematic = false;

            despawnTimer.ResetTimer();

            audioSource.PlayOneShot(PickRandomSFXFromArray(hitSFX));
            StartCoroutine(PlayAnimationThenMove("Hit", MoveAnim.Hit));

            crocManager.OnCrocHit();

            State = CrocState.IsDown;
        }
    }
    public void Stop()
    {

        this.transform.position = originalPosition + new Vector3(0, -2, 0);
        this.State = CrocState.IsDown;
        this.despawnTimer.ResetTimer();
    }

    // TIMER MANAGEMENT
    public void StartDespawnTimer(float durationInSeconds)
    {
        despawnTimer.StartTimer(Time.time, durationInSeconds);
    }
    public void UpdateDespawnTimer()
    {
        despawnTimer.UpdateTimer(Time.time);
    }
    public void ResetDespawnTimer()
    {
        despawnTimer.ResetTimer();
    }

    // M E T H O D S: MINOR
    // for audioclip randomization
    AudioClip PickRandomSFXFromArray(AudioClip[] audioClips)
    {
        return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
    }

    private enum MoveAnim { Up, Down, Hit }
    IEnumerator PlayAnimationThenMove(string animationTriggerName, MoveAnim direction)
    {
        animator.SetTrigger(animationTriggerName); //triggers the animation
        yield return new WaitForSeconds(3); //waits 

        switch (direction) // handles movement behavior
        {
            case MoveAnim.Up:
                this.transform.position = originalPosition
                    + new Vector3(0, moveDistance, 0); //corrects position to keep it from drifting 
                break;

            case MoveAnim.Down:
            case MoveAnim.Hit:
                transform.position = originalPosition;
                break;
        }
    }

    // the original MoleController.cs had timers in it that weren't actually 
    // being used, so that logic (the TickTimers() method) has been removed
}
