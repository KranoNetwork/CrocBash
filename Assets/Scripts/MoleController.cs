using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;
using TMPro;
using UnityEngine.SceneManagement;


public class MoleController : MonoBehaviour
{


    public GameObject selectMole;
    public GameObject gameController; // References Game Controller
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

    // Start is called before the first frame update

    void Start()

    {
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
        this.tag = "moles";
        originalPosition = transform.position;

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
        moveDistance = Mathf.Clamp01(moveDistance);
        Debug.Log("Popup script STARTED for " + this);
        //transform.position = Vector3.Lerp(this.transform.position, this.transform.position + new Vector3(0, moveDistance, 0), moveSpeed * Time.deltaTime);
        //transform.position = Vector3.Lerp(transform.position, transform.position + new Vector3(0, moveDistance, 0), Time.deltaTime / moveSpeed);
        transform.position = Vector3.Lerp(this.transform.position, this.transform.position + new Vector3(0, moveDistance, 0), moveDistance);

    }

    private void MoveDown()

    {

        transform.position = originalPosition;

    }

    public void OnCollisionEnter(Collision other)
    {

        if (other.gameObject.tag == "Plate")
        {
            Debug.Log("Collision from the floor");
            col = false;
            MoveDown();
            gameController.GetComponent<GameController>().score += 1;
            selectMole.GetComponent<SelectPopupMole>().MoleHitHammer(myNumber);
        }

    }

    public void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "Hammer" && this.tag == "moles")
        {
            var relativePosition = transform.InverseTransformPoint(other.transform.position);
            if (relativePosition.y > 1 && col == false)
            {
                Debug.Log("The object is above.");
                rb.isKinematic = false;
                col = true;
                Hit();
                MoveDown(); // CAN BE REMOVED IF DONT WANT MOLES TELEPORTING DOWN
                //this.gameObject.GetComponent<Rigidbody>().AddForce(0, -10, 0);
            }
            if (relativePosition.x > 0 && col == false)
            {
                Debug.Log("The object is to the right");
                rb.isKinematic = true;
            }
            if (relativePosition.x < 0 && col == false)
            {
                Debug.Log("The object is to the left");
                rb.isKinematic = true;
            }
            if (relativePosition.z > 0 && col == false)
            {
                Debug.Log("The object is in front.");
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
            isHit = true;
            isUp = false;
            //MoveDown(); 
            isMovingUp = false;
            downTimer = 0;
            upTimer = 0;
            moveTimer = 0;
            //this.tag = "moles";
            //gameController.GetComponent<GameController>().score += 1;

        }
    }
}
