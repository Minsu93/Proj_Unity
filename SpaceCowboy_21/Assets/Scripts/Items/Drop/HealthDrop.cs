using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : Collectable
{
    [SerializeField] float healAmount = 5f;
    protected override void ConsumeEffect()
    {
        GameManager.Instance.playerManager.playerBehavior.healEvent(healAmount);
    }

}
