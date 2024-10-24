using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Tester : EnemyAction
{
    protected override void Awake()
    {
        base.Awake();
        health.ResetHealth();
    }

    public override void BrainStateChange()
    {
        return;
    }

    public override void EnemyKnockBack(Vector2 hitPos, float forceAmount)
    {
        return;
    }

    protected override IEnumerator StrikeRoutine(Vector2 strikePos, Planet planet)
    {
        yield return null;
    }

}
