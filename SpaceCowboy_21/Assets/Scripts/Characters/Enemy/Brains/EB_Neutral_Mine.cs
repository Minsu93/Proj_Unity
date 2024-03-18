using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neutral_Mine : EB_Neutral
{
    public float mineDamage = 3f;
    public ParticleSystem hitEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerBehavior>().DamageEvent(mineDamage);
            DamageEvent(99f);
        }
    }
    public override void DamageEvent(float dmg)
    {
        if (enemyState == EnemyState.Die)
            return;

        //데미지를 적용
        if (health.AnyDamage(dmg))
        {
            //맞는 효과 
            //action.HitView();

            if (health.IsDead())
            {
                //StopAllCoroutines();
                //죽은 경우 
                enemyState = EnemyState.Die;

                if(hitEffect != null)
                {
                    ParticleHolder.instance.GetParticle(hitEffect, transform.position, transform.rotation);
                }

                gameObject.SetActive(false);
            }
        }
    }
}
