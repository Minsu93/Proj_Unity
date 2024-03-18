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
                
                if(dropitem != null)
                {
                    dropitem.GenerateItem();
                }

                gameObject.SetActive(false);
            }
        }
    }
}
