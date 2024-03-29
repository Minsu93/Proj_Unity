using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neutral : EnemyBrain
{
    public override void BrainStateChange()
    {
        return;
    }

    protected override void AfterHitEvent()
    {
        return;
    }

    protected override void WhenDieEvent()
    {
        return;
    }
}
