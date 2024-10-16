using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDrop : AutoCollectable
{
    [SerializeField] ParticleSystem particle;

    public void InitializeResource(float min, float max)
    {
        var mainModule = particle.main;
        mainModule.startSize = new ParticleSystem.MinMaxCurve(min, max);
    }
}
