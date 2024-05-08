using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Triggers a particle effect that isn't attached to the same gameObject.
/// </summary>
public class TriggerParticleFX : MonoBehaviour
{
    // P R O P E R T I E S
    [Tooltip("First particle fx slot.")]
    [SerializeField] ParticleSystem particleSystem1;
    [Tooltip("Second particle fx slot.")]
    [SerializeField] ParticleSystem particleSystem2;

    // M E T H O D S
    /// <summary>
    /// Plays the particle fx in the first slot
    /// </summary>
    public void PlayParticleFX1()
    {
        particleSystem1.Play();
    }/// <summary>
     /// Plays the particle fx in the second slot
     /// </summary>
    public void PlayParticleFX2()
    {
        particleSystem2.Play();
    }
}
