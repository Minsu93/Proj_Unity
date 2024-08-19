using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shut_Skill_Proj_Bouncer : Shut_Skill_Proj
{
    public override void DamageEvent(float damage, Vector2 hitPoint)
    {
        int enemyCount = CheckEnemies(out Collider2D[] enemies);

        if (enemyCount > 0)
        {
            //적 방향으로 사격
            int ranInt = UnityEngine.Random.Range(0,enemyCount);
            StartCoroutine(burstShootRoutine(enemies[ranInt].transform));
        }
        else
        {
            //랜덤 방향으로 사격
            StartCoroutine(burstShootRoutine());
        }
    }

    protected IEnumerator burstShootRoutine()
    {
        for (int i = 0; i < attackProperty.numberOfBurst; i++)
        {
            Vector2 pos = transform.position;
            Vector2 randomVec = UnityEngine.Random.insideUnitCircle.normalized;
            Shoot(pos, randomVec);
            yield return new WaitForSeconds(attackProperty.burstInterval);
        }
    }

    protected int CheckEnemies(out Collider2D[] enemyColls)
    {
        enemyColls = new Collider2D[10];
        int num = Physics2D.OverlapCircleNonAlloc(transform.position, enemyCheckRange, enemyColls, LayerMask.GetMask("Enemy"));

        return num;
    }

    public override void Kicked(Vector2 hitPos)
    {
        return;
    }
    public override void CompleteEvent()
    {
        return;
    }


}
