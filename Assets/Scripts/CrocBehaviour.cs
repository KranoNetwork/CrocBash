using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum CrocState { IsDown, IsMovingUp, IsUp, IsHit }
public class CrocBehaviour : MonoBehaviour
{
    /// <summary>
    /// The behavior of the crocs
    /// they move up and down when called to
    /// 
    /// this script shouldn't have the system for selecting a mole 
    /// </summary>

    // P R O P E R T I E S
    [Header("Movement")]
    [SerializeField] Vector3 originalPosition;
    [SerializeField] float moveDistance;
    [SerializeField] float moveSpeed;
    public CrocState State;

    [Header("Timer")]
    [SerializeField] float downTimer;
    [SerializeField] float maxUpTime;

    [Header("Referemces")]
    [SerializeField] string playerTagName;
    Rigidbody rb;



    // U N I T Y  M E T H O D S
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        if (TagManager.CompareTags(collision.gameObject, playerTagName))
        {
            // hit behavior
            Debug.Log("Hit!");
            
        }
    }

    // C U S T O M  M E T H O D S
    public void MoveUp()
    {
        State = CrocState.IsMovingUp;
        moveDistance += moveSpeed * Time.deltaTime;
        moveDistance = Mathf.Clamp01(moveDistance) + moveDistance;
        transform.position = Vector3.Lerp(this.transform.position, this.transform.position + new Vector3(0, 2, 0), moveDistance);
    }

    public void MoveDown()
    {
        State = CrocState.IsDown;
        transform.position = originalPosition;
    }
}
