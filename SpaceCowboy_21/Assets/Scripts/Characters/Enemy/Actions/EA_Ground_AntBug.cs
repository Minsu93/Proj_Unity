using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ground_AntBug : EA_Ground
{
    /// <summary>
    /// wait ���¿����� ���� �İ��� �����ٰ�, Chase ���¿����� ������ ���ͼ� �÷��̾� ������ ����. 
    /// </summary>
    /// 

    //[Header("�ӽ� View")]
    ////�ӽ�. 
    //public SpriteRenderer characterView;
    //protected override void DoAction(EnemyState state)
    //{
    //    switch (state)
    //    {
    //        case EnemyState.Sleep:
    //            OnSleepEvent();
    //            StartIdleView();
    //            break;

    //        case EnemyState.Chase:
    //            StopAllCoroutines();
    //            BurrowOut();
    //            //StartCoroutine(ChaseRepeater());
    //            break;


    //        case EnemyState.Wait:
    //            //Wait���·� �Ѿ ���� �ϴ� ��� �ൿ�� ���߰� ������ �ִ´�. 
    //            StopAllCoroutines();
    //            BurrowIn();
    //            break;

    //        case EnemyState.Die:
    //            StopAllCoroutines();
    //            DieView();

    //            //�Ѿ˿� �´� Enemy Collision ����
    //            hitCollObject.SetActive(false);
    //            gameObject.SetActive(false);
    //            break;
    //    }
    //}

    ////�İ���
    //void BurrowIn()
    //{
    //    hitCollObject.SetActive(false);
    //    characterView.enabled = false;
    //}

    //void BurrowOut()
    //{
    //    hitCollObject.SetActive(true);
    //    characterView.enabled = true;
    //}
}
