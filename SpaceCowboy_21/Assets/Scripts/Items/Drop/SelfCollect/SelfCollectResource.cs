using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfCollectResource : SelfCollectable
{
    [SerializeField] int index = 0;
    [SerializeField] float amount = 5.0f;

    protected override bool ConsumeEvent()
    {
        GameManager.Instance.playerManager.GainExp(index, amount);
        return true;
    }

}
