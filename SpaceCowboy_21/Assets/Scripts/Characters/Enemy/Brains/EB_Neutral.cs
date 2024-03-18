using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Neutral : EnemyBrain
{
    DropItem dropitem;

    public override void Initialize()
    {
        dropitem = GetComponent<DropItem>();    
    }

    public override void DetectSiutation()
    {
        return;
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
                //StopAllCoroutines();
                //���� ��� 
                enemyState = EnemyState.Die;
                
                if(dropitem != null)
                {
                    dropitem.GenerateItem();
                }

                gameObject.SetActive(false);
            }
        }
    }
}
