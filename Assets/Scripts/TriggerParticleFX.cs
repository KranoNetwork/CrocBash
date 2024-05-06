using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers a particle effect that isn't attached to the same gameObject.
/// </summary>
public class TriggerParticleFX : MonoBehaviour
{
    // P R O P E R T I E S
    [SerializeField] ParticleSystem particleSystem1;
    [SerializeField] ParticleSystem particleSystem2;

    // M E T H O D S
    public void PlayParticleFX1()
    {
        particleSystem1.Play();
    }
    public void PlayParticleFX2()
    {
        particleSystem2.Play();
    }
}
