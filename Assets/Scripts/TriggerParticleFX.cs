using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerParticleFX : MonoBehaviour
{
    // P R O P E R T I E S
    [SerializeField] ParticleSystem particleSystem1;
    [SerializeField] ParticleSystem particleSystem2;

    // U N I T Y   M E T H O D S
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // C U S T O M   M E T H O D S
    public void PlayParticleFX1()
    {
        particleSystem1.Play();
    }
    public void PlayParticleFX2()
    {
        particleSystem2.Play();
    }
}
