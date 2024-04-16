using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Orbit_Shuttle : EA_Orbit
{
    [Header("Shuttle")]
    public GameObject enemyPrefab;  //������ ��
    public Transform SpawnPoint;   //���� ��ġ

    //protected override IEnumerator AttackCoroutine()
    //{
    //    yield return StartCoroutine(DelayRoutine(preAttackDelay));

    //    SummonAction();

    //}

    void SummonAction()
    {
        //gunTipRot, gunTipPos ������Ʈ
        Vector2 pos = SpawnPoint.position;
        Quaternion rot = transform.rotation;

        GameObject enemy = PoolManager.instance.GetEnemy(enemyPrefab);
        enemy.transform.position = pos;
        enemy.transform.rotation = rot;
        enemy.GetComponent<EnemyBrain>().ResetEnemyBrain(EnemyState.Sleep);

        //View���� �ִϸ��̼� ����
        //AttackView();
    }

    //public override void ChangeDirectionToRight(bool right)
    //{
    //    base.ChangeDirectionToRight(right);

    //    //�ӽ� viewObj ȸ��
    //    if (ViewObj != null)
    //    {
    //        ViewObj.transform.localScale = new Vector3(-direction * 2, 2, 2);
    //    }
    //}
}
