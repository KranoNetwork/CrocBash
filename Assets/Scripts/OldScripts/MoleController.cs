using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.SceneManagement;
using static UnityEngine.GraphicsBuffer;


public class MoleController : MonoBehaviour
{
    [SerializeField] TagObject tags;


    public GameObject selectMole;
    public GameObject gameController; // References Game Controller
    public GameObject Player;
    public float moveDistance = 0f;
    [SerializeField] private float moveSpeed = .5f;
    float downTimer = 0;
    float upTimer = 0;
    int randoLimit = 5;
    [SerializeField] private int randoLimitMax = 7;
    [SerializeField] private float maxUpTime = 5;
    private float moveTimer = 0;
    private bool isHit = true;
    private bool isUp = false;
    private bool isMovingUp = false;
    private Vector3 originalPosition;
    public int myNumber;
    private float pushForce = 1;

    bool col;

    public Rigidbody rb;

    AudioSource music;

    public AudioSource bonk;

    // Start is called before the first frame update

    void Start()

    {
        music = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        for (int i = 0; i < 5; i++)
        {
            if (this.name == selectMole.GetComponent<SelectPopupMole>().molePrefab[i].name)
            {
                myNumber = i;
                break;
            }
        }
        
        originalPosition = transform.position;

        // Get the direction to the player
        Vector3 directionToPlayer = Player.transform.position - transform.position;

        // Flatten the direction to only consider the XZ plane (Y-axis is ignored)
        directionToPlayer.y = 0;

        // Create the rotation angle only on the Y-axis
        Quaternion rotationAngle = Quaternion.LookRotation(directionToPlayer);

        // Apply the rotation only on the Y-axis
        transform.rotation = rotationAngle;

    }

    // Update is called once per frame

    void Update()
    {


        TickTimers();
        if (isUp)
        {
            isMovingUp = false;
            if (downTimer >= maxUpTime)

            {

                MoveDown();

                isUp = false;
                downTimer = 0;
            }
        }

    }

    private void FixedUpdate()

    {

    }

    public void Popup()

    {
        isHit = false;
        isMovingUp = true;
        moveDistance += moveSpeed * Time.deltaTime;
        moveDistance = Mathf.Clamp01(moveDistance) + moveDistance;
        Debug.Log("Popup script STARTED for " + this);
        //transform.position = Vector3.Lerp(this.transform.position, this.transform.position + new Vector3(0, moveDistance, 0), moveSpeed * Time.deltaTime);
        //transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, moveDistance, 0), Time.deltaTime / moveSpeed);
        transform.position = Vector3.Lerp(this.transform.position, this.transform.position + new Vector3(0, 2, 0), moveDistance);

    }

    private void MoveDown()

    {

        transform.position = originalPosition;

    }

    public void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.CompareTags("Moles") && this.gameObject.CompareTags("SelectedMole"))
        {
            var relativePosition = transform.InverseTransformPoint(other.transform.position);
            if (relativePosition.y > 0 && col == false)
            {
                col = true;
                if(col == true)
                {
                    Debug.Log("The object is above.");
                    rb.isKinematic = false;
                    Hit();
                    //MoveDown(); 
                    //this.gameObject.GetComponent<Rigidbody>().AddForce(0, -10, 0);
                }
            }
            if (relativePosition.y < 1 && col == false)
            {
                //Debug.Log("The object is to the Below");
                //MoveDown();
                rb.isKinematic = true;
            }
            if (relativePosition.x > 0 && col == false)
            {
                //Debug.Log("The object is to the right");
                rb.isKinematic = true;
            }
            if (relativePosition.x < 0 && col == false)
            {
                //Debug.Log("The object is to the left");
                rb.isKinematic = true;
            }
            if (relativePosition.z > 0 && col == false)
            {
                //Debug.Log("The object is in front.");
                rb.isKinematic = true;
            }
        }

	}
    private void TickTimers()

    {

        if (isMovingUp)

        {

            moveTimer += Time.deltaTime;

        }

        else if (isUp)

        {

            downTimer += Time.deltaTime;

        }

    }

    public void Hit()
    {
        if (isHit == false)
        {
            col = false;
            rb.isKinematic = true;
            isHit = true;
            isUp = false;
            MoveDown();
            isMovingUp = false;
            downTimer = 0;
            upTimer = 0;
            moveTimer = 0;
            bonk.Play();
            gameController.GetComponent<GameController>().score += 1;
            selectMole.GetComponent<SelectPopupMole>().MoleHitHammer(myNumber);

        }
    }
}
