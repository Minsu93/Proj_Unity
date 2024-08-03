using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : SelfCollectable
{
    [SerializeField] float healAmount = 5f;
    protected override bool ConsumeEffect()
    {
        return GameManager.Instance.playerManager.HealthUp(healAmount);
    }

}
