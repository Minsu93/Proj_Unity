using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neu_ShieldFlower : EnemyBrain
{
    public float recoverAmount = 1f;
    [SerializeField] PlayerHealth pHealth;

    public override void DetectSiutation()
    {
        if (pHealth != null)
        {
            pHealth.RecoverShield(recoverAmount);
        }
    }


    public override void DamageEvent(float dmg)
    {
        if (enemyState == EnemyState.Die)
            return;

        //�������� ����
        if (health.AnyDamage(dmg))
        {
            //�´� ȿ�� 
            //action.HitView();

            if (health.IsDead())
            {
                //���� ��� 
                enemyState = EnemyState.Die;

                gameObject.SetActive(false);
            }
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pHealth = collision.GetComponent<PlayerHealth>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            pHealth = null;
        }
    }


}
