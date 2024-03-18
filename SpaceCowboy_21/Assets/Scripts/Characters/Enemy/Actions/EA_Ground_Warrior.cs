using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ground_Warrior : EA_Ground
{
    protected override IEnumerator AttackCoroutine()
    {
        attackOn = true;
        AimOnView();

        yield return StartCoroutine(DelayRoutine(preAttackDelay));

        //юс╫ц
        AttackView();
        yield return StartCoroutine(DelayRoutine(AttackDelay));

        AimOffView();

        yield return StartCoroutine(DelayRoutine(afterAttackDelay));

        brain.ChangeState(EnemyState.Chase, 0f);
        attackOn = false;
    }
}
