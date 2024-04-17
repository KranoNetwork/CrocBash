using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KR_SeaRotation : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField]
    float maxrotation;

    [Range(1f, 200f)]
    [SerializeField]
    float RotationSpeed;

    [Range(0, 2)]
    [SerializeField]
    int Axis;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Axis == 0)
        {
            this.gameObject.transform.Rotate(maxrotation * Mathf.Sin(Time.deltaTime * (RotationSpeed / 100)), 0,0);
        }
        else if(Axis == 1)
        {
            this.gameObject.transform.Rotate(0, maxrotation * Mathf.Sin(Time.deltaTime * (RotationSpeed / 100)), 0);
        }
        else
        {
            this.gameObject.transform.Rotate(0, 0, maxrotation * Mathf.Sin(Time.deltaTime * (RotationSpeed / 100)));
        }
    }
}
