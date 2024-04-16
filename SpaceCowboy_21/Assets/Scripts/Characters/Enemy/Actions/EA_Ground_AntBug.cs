using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ground_AntBug : EA_Ground
{
    /// <summary>
    /// wait 상태에서는 지상에 파고들어 숨었다가, Chase 상태에서는 밖으로 나와서 플레이어 쪽으로 기어간다. 
    /// </summary>
    /// 

    //[Header("임시 View")]
    ////임시. 
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
    //            //Wait상태로 넘어갈 때는 하던 모든 행동을 멈추고 가만히 있는다. 
    //            StopAllCoroutines();
    //            BurrowIn();
    //            break;

    //        case EnemyState.Die:
    //            StopAllCoroutines();
    //            DieView();

    //            //총알에 맞는 Enemy Collision 해제
    //            hitCollObject.SetActive(false);
    //            gameObject.SetActive(false);
    //            break;
    //    }
    //}

    ////파고들기
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
