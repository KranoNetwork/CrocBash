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
    /// 
    /// this script shouldn't have the system for selecting a mole 
    /// </summary>

    // P R O P E R T I E S
    [Header("Movement")]
    [SerializeField] Vector3 originalPosition; // original pos for putting the croc back down
    [SerializeField] float moveDistance; //distance to move the croc up
    [SerializeField] float moveSpeed = 0.1f; //speed at which to move the croc up
    public CrocState State; //the croc state; replaces the bools: isHit, isUp, and isMovingUp in the MoleController script
    public CrocMode CrocModeToggle; //debug feature
    [SerializeField] private GameObject crocMesh; //ref to the mesh the animations play on

    private enum CoroutineState { Off, Started, Running, Ended }
    [SerializeField] private CoroutineState coroutineState; //for adjusting position after animation has finished playing

    [Header("Timer")]
    Timer despawnTimer; //timer for handling despawing if not hit
    [SerializeField] float timeBeforeDespawnIfNotHit; //amount of time to wait before despawning
    [SerializeField] int popCounts; //keeps track of the number of time the croc has popped up -> used for decreasing timeBeforeDespawnIfNotHit so that as the game progresses, the crocs spawn and despawn faster.

    [Header("Animation")]
    [SerializeField] Animator animator;
    private Transform originalTransform;
    private Transform upTransform;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] hitSFX;
    [SerializeField] AudioClip[] spawnSFX;
    [SerializeField] AudioClip[] despawnSFX;

    [Header("References")]
    [SerializeField] string playerTagName; // the tag the player has (through the liminal tag package)
    Rigidbody rb; // the rigidbody attached to the gameObj this script is attached to
    //[SerializeField] GameObject crocUp;
    //[SerializeField] GameObject crocDown;
    [SerializeField] CrocManager crocManager; //[TD] - dependency



    // U N I T Y  M E T H O D S
    void Awake()
    {
        despawnTimer = new Timer();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        coroutineState = CoroutineState.Off; // making sure this is true out the gate
        
    }
    void OnEnable()
    {
        originalPosition = transform.position;
        //crocMesh.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        SetCrocMode();
    }

    // Update is called once per frame
    void Update()
    {
        this.rb.velocity = Vector3.zero;

        
        UpdateBasedOnState();

        UpdateBasedOnTimerState();
        

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
                HandleSpawnRateIncrease();
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
    void HandleSpawnRateIncrease()
    {
        if (popCounts >= 2)
        {
            timeBeforeDespawnIfNotHit--;
            popCounts = 0;

            if (timeBeforeDespawnIfNotHit <= 2)
                timeBeforeDespawnIfNotHit = 1.5f;
        }
    }

    void UpdateBasedOnTimerState()
    {
        switch(despawnTimer.State)
        {
            case TimerState.Off:
                //crocMesh.SetActive(false);
                break;
            case TimerState.Started:
            case TimerState.Running:
                //do nothing
                break;

            
            case TimerState.Ended:
                if (State == CrocState.IsUp) 
                    Despawn();
                break;
        }
    }


    // S E T  U P  M E T H O D S
    void SetCrocMode() // for  toggling between the different modes of having the crocs pop up
    {
        if (crocManager != null)
            this.CrocModeToggle = crocManager.CrocModeToggle;
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

    // C U S T O M  M E T H O D S
    public void MoveUp() // Replaces the PopUp() method from MoleController.cs
    {
        State = CrocState.IsMovingUp;
        audioSource.PlayOneShot(PickRandomSFXFromArray(spawnSFX));
        popCounts++;
        despawnTimer.StartTimer(Time.time, timeBeforeDespawnIfNotHit);
        switch (CrocModeToggle)
        {
            case CrocMode.TransformMotion:
                animator.SetTrigger("MoveUp");
                transform.position = Vector3.Lerp(this.transform.position, this.transform.position + new Vector3(0, moveDistance, 0), moveSpeed);
                break;

            case CrocMode.AnimationMotion:
                Move(MoveAnim.Up);
                break;
        }
        
        State = CrocState.IsUp;
    }

    public void MoveDown()
    {
        switch (CrocModeToggle)
        {
            case CrocMode.TransformMotion:
                
                transform.position = originalPosition;
                //transform.position = Vector3.Lerp(transform.position, originalPosition, moveSpeed);
                break;

            case CrocMode.AnimationMotion:
                Move(MoveAnim.Down);
                break;
        }
        
    }
    private enum MoveAnim { Up, Down, Hit }
    IEnumerator PlayAnimationThenMove(string animationTriggerName, MoveAnim direction)
    {
        coroutineState = CoroutineState.Started;
        //crocMesh.SetActive(true);
        animator.SetTrigger(animationTriggerName);
        coroutineState = CoroutineState.Running;
        yield return new WaitForSeconds(3);

        switch (direction)
        {
            case MoveAnim.Up:
                this.transform.position = originalPosition 
                    + new Vector3(0, moveDistance, 0); //corrects position to keep it from drifting 
                break;

            case MoveAnim.Down:
            case MoveAnim.Hit:
                transform.position = originalPosition;
                //crocMesh.SetActive(false);
                break;
        }
        coroutineState = CoroutineState.Ended;
    }
    private void Move(MoveAnim direction)
    {
        switch (direction)
        {
            case MoveAnim.Up:
                despawnTimer.StartTimer(Time.time, timeBeforeDespawnIfNotHit);
                StartCoroutine(PlayAnimationThenMove("MoveUp", MoveAnim.Up));
                break;

            case MoveAnim.Down:
                despawnTimer.ResetTimer();
                StartCoroutine(PlayAnimationThenMove("MoveDown", MoveAnim.Down));
                break;

            case MoveAnim.Hit:
                despawnTimer.ResetTimer();
                StartCoroutine(PlayAnimationThenMove("MoveUp", MoveAnim.Up));
                break;
        }
    }
    public void Hit()
    {
        if (State == CrocState.IsUp)
        {
            
            State = CrocState.IsHit;
            rb.isKinematic = false;
            switch(CrocModeToggle)
            {
                case CrocMode.TransformMotion:
                    animator.SetTrigger("Hit");
                    MoveDown();
                    break;
                case CrocMode.AnimationMotion:
                    Move(MoveAnim.Hit);
                    break;
            }
            
            despawnTimer.ResetTimer();
            audioSource.PlayOneShot(PickRandomSFXFromArray(hitSFX));

            //lets croc manager know it's been it
            crocManager.OnCrocHit(); // [TD] - dependency


            State = CrocState.IsDown;
        }
    }
    private void Despawn()
    {
        if (State == CrocState.IsUp)
        {
            rb.isKinematic = false;
            
            switch(CrocModeToggle)
            {
                case CrocMode.TransformMotion:
                    MoveDown(); 
                    break;

                case CrocMode.AnimationMotion:
                    //transform.position = originalPosition;  //does smth weird -> sometimes moves them to the side :P
                    StartCoroutine(PlayAnimationThenMove("MoveDown", MoveAnim.Down));
                    break;
            }
            
            despawnTimer.ResetTimer();
            audioSource.PlayOneShot(PickRandomSFXFromArray(despawnSFX));
            //lets croc manager know the croc has despawned
            crocManager.OnCrocDespawn(); // [TD] - dependency


            State = CrocState.IsDown;
        }
    }

    public void SetCrocState(CrocState state) //updates the croc's state 
    {
        State = state;
    }

    public void Stop()
    {
        
        this.transform.position = originalPosition + new Vector3(0, -2, 0);
        this.State = CrocState.IsDown;
        this.despawnTimer.ResetTimer();
    }
    // for audioclip randomization
    AudioClip PickRandomSFXFromArray(AudioClip[] audioClips)
    {
        return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
    }

    // the original MoleController.cs had timers in it that weren't actually 
    // being used, so that logic (the TickTimers() method) has been removed
}
