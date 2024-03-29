using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Ship : EnemyBrain
{
    //함선. 플레이어를 천천히 쫓아오며 갖가지 포대로 공격한다. 포대들이 모두 파괴되어야 Ship은 파괴된다. 

    public override void BrainStateChange()
    {
        throw new System.NotImplementedException();
    }

    public override void WakeUp()
    {
        throw new System.NotImplementedException();
    }

    protected override void AfterHitEvent()
    {
        throw new System.NotImplementedException();
    }

    protected override void WhenDieEvent()
    {
        throw new System.NotImplementedException();
    }
}
