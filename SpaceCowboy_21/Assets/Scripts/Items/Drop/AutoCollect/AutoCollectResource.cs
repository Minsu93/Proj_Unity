using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCollectResource : AutoCollectable
{
    [SerializeField] ParticleSystem particle;

    //public void InitializeResource(float min, float max)
    //{
    //    var mainModule = particle.main;
    //    mainModule.startSize = new ParticleSystem.MinMaxCurve(min, max);
    //}

    protected override bool ConsumeEvent()
    {
        //°ñµå È¹µæ
        return true;
    }
}
