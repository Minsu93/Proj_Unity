using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_E_AirMine : Projectile_GuideMissile
{


    //protected override void OnTriggerEnter2D(Collider2D collision)
    //{
    //    //�Ѿ��� �浹���� �� 
    //    if (collision.CompareTag("Player"))
    //    {
    //        if (collision.TryGetComponent(out PlayerBehavior behavior))
    //        {
    //            //�÷��̾ ������ ���
    //            if (behavior.activate)
    //                return;

    //            //������ ����
    //            behavior.DamageEvent(damage);

    //            ShowHitEffect();
    //            AfterHitEvent();
    //        }
    //    }

    //    if (collision.CompareTag("SpaceBorder"))
    //    {
    //        ShowHitEffect();
    //        AfterHitEvent();
    //    }
                
    //}


    //public override void DamageEvent(float dmg, Vector2 objVel)
    //{
    //    //�ǰݽ� �ߵ��ϴ� �̺�Ʈ

    //    if (!hitByProjectileOn)     //projectile-player �� ����üũ�̱� �ϴ�. (�ʿ����)
    //        return;
    //    if (health.AnyDamage(dmg))  //��� ���ظ� �Ծ����� true
    //    {
    //        //HitFeedBack();   //Projectile �� �ִ�. View �� HitFeedback�� �����Ų��.
    //    }
    //    if (health.IsDead())
    //    {
    //        ShowHitEffect();
    //        AfterHitEvent();
    //    }

    //}
}
