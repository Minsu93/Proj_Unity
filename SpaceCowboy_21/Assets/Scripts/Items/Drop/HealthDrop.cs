using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : SelfCollectable
{
    [SerializeField] float healAmount = 5f;
    protected override bool ConsumeEvent()
    {
        return GameManager.Instance.playerManager.HealthUp(healAmount);
    }

}
