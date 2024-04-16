using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Orbit_Shuttle : EA_Orbit
{
    [Header("Shuttle")]
    public GameObject enemyPrefab;  //스폰할 적
    public Transform SpawnPoint;   //스폰 위치

    //protected override IEnumerator AttackCoroutine()
    //{
    //    yield return StartCoroutine(DelayRoutine(preAttackDelay));

    //    SummonAction();

    //}

    void SummonAction()
    {
        //gunTipRot, gunTipPos 업데이트
        Vector2 pos = SpawnPoint.position;
        Quaternion rot = transform.rotation;

        GameObject enemy = PoolManager.instance.GetEnemy(enemyPrefab);
        enemy.transform.position = pos;
        enemy.transform.rotation = rot;
        enemy.GetComponent<EnemyBrain>().ResetEnemyBrain(EnemyState.Sleep);

        //View에서 애니메이션 실행
        //AttackView();
    }

    //public override void ChangeDirectionToRight(bool right)
    //{
    //    base.ChangeDirectionToRight(right);

    //    //임시 viewObj 회전
    //    if (ViewObj != null)
    //    {
    //        ViewObj.transform.localScale = new Vector3(-direction * 2, 2, 2);
    //    }
    //}
}
