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
    [SerializeField] float moveSpeed; //speed at which to move the croc up
    private Vector3 ogScale;
    public CrocState State; //the croc state; replaces the bools: isHit, isUp, and isMovingUp in the MoleController script

    [Header("Audio")]
    [SerializeField] AudioSource hitSoundEffect; //stores bonk sfx

    [Header("Referemces")]
    [SerializeField] string playerTagName; // the tag the player has (through the liminal tag package)
    Rigidbody rb; // the rigidbody attached to the gameObj this script is attached to
    [SerializeField] CrocManager crocManager; //[TD] - dependency



    // U N I T Y  M E T H O D S
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        ogScale = transform.localScale;
    }
    void OnEnable()
    {
        originalPosition = transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // C O L L I S I O N
    public void OnCollisionEnter(Collision collision)
    {
        // used for making sure the things are hit from above and not from the side
        Vector3 _tempRelativePosition;

        if (TagManager.CompareTags(collision.gameObject, playerTagName))
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
            if (_tempRelativePosition.x < 0 )
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
        //moveDistance += moveSpeed * Time.deltaTime;
        //moveDistance = Mathf.Clamp01(moveDistance) + moveDistance;
        transform.position = Vector3.Lerp(this.transform.position, this.transform.position + new Vector3(0, moveDistance, 0), moveSpeed);
        State = CrocState.IsUp;
    }

    public void MoveDown()
    {
        State = CrocState.IsDown;
        transform.position = originalPosition;
    }

    public void Hit()
    {
        if (State == CrocState.IsUp)
        {
            
            State = CrocState.IsHit;
            rb.isKinematic = false;
            MoveDown();
            // reset timer
            hitSoundEffect.Play();
            //lets croc manager know it's been it
            crocManager.OnCrocHit(); // [TD] - dependency
        }
    }

    public void SetCrocState(CrocState state) //updates the croc's state 
    {
        State = state;
    }

    public void Stop()
    {
        this.transform.position = originalPosition;
        this.State = CrocState.IsDown;
    }

    // the original MoleController.cs had timers in it that weren't actually 
    // being used, so that logic (the TickTimers() method) has been removed
}
