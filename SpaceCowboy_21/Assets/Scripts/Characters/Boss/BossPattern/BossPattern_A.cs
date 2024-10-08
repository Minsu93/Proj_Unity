using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossPattern_A : AttackPattern
{
    ///보스A 의 패턴에 필요한 정보들을 담아놓는 곳.
    ///위치로 이동
    /// 사격
    /// 소환
    /// 

    #region Condition

    protected bool PlayerIsNear(float checkRange)
    {
        Vector2 playerPos = GameManager.Instance.player.position;
        float dist = (playerPos - (Vector2)transform.position).magnitude;
        if (checkRange > dist)
        {
            return true;
        }
        else return false;
    }

    protected bool PlayerIsVisible()
    {
        Vector2 playerPos = GameManager.Instance.player.position;
        Vector2 vec = (playerPos - (Vector2)transform.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, vec.normalized, vec.magnitude, LayerMask.GetMask("Planet"));
        if (hit.collider != null)
        {
            return false;
        }
        else return true;
    }

    #endregion



    #region Action
    //주어진 위치로 날아간다.
    protected IEnumerator FlyToPosition(Vector2 pos, float moveDuration, Rigidbody2D rb, AnimationCurve moveCurve)
    {
        float time = 0;
        Vector2 startPos = transform.position;
        while (time < moveDuration)
        {
            time += Time.fixedDeltaTime;
            rb.MovePosition(Vector2.Lerp(startPos, pos, moveCurve.Evaluate(time / moveDuration)));

            yield return new WaitForFixedUpdate();
        }
        
    }


    //원하는 GameObject를 발사한다. 
    protected IEnumerator Shoot(Vector2 pos, Quaternion rot, ProjectileAttackProperty attackProperty)
    {
        float totalSpread = attackProperty.projectileSpread * (attackProperty.numberOfProjectile - 1);       //우선 전체 총알이 퍼질 각도를 구한다  
                                                                                                             //시작 각도
        Quaternion targetRotation = rot * Quaternion.Euler(0, 0, -(totalSpread / 2));

        for (int i = 0; i < attackProperty.numberOfBurst; i++)
        {
            //랜덤 각도 추가
            float randomAngle = UnityEngine.Random.Range(-attackProperty.randomSpreadAngle * 0.5f, attackProperty.randomSpreadAngle * 0.5f);
            Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

            //멀티샷
            for (int j = 0; j < attackProperty.numberOfProjectile; j++)
            {
                Quaternion tempRot = targetRotation * Quaternion.Euler(0, 0, attackProperty.projectileSpread * (j));

                //총알 생성
                GameObject projectile = GameManager.Instance.poolManager.GetPoolObj(attackProperty.projectilePrefab,1);
                projectile.transform.position = pos;
                projectile.transform.rotation = tempRot * randomRotation;
                Projectile proj = projectile.GetComponent<Projectile>();
                proj.Init(attackProperty.damage, attackProperty.speed, attackProperty.lifeTime, attackProperty.range);
            }

            yield return new WaitForSeconds(attackProperty.burstInterval);
        }
    }

    //원하는 Enemy를 x마리 소환한다.
    protected void SpawnEnemy(string name, int count)
    {
        WaveManager.instance.SpawnObjects(name, count);
    }

    #endregion
}
