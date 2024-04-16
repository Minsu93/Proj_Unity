using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour, ISwitchable
{
    bool activate = false;
    public Transform muzzlePos;
    EnemyBrain enemyBrain;


    AllyBrain brain;
    AllyAction action;

    public Sprite[] turnOnSprites;
    public SpriteRenderer headSpr;
    public SpriteRenderer neckSpr;

    void Awake()
    {
        brain =  GetComponent<AllyBrain>();
        action = GetComponent<AllyAction>();
    }

    public void ActivateObject()
    {
        //이 오브젝트를 가동한다
        activate = true;
        if (headSpr != null) headSpr.sprite = turnOnSprites[0];
        if (neckSpr != null) neckSpr.sprite = turnOnSprites[1];

    }

    public void DeactivateObject()
    {
        //오브젝트를 비가동한다. 
        activate = false;
    }

    void Update()
    {
        if (!activate) return;

        //적 탐색
        if (enemyBrain == null)
        {
            //적이 없다면 새로운 적을 찾는다.
            if (brain.CheckNearestEnemyBrain(out EnemyBrain eBrain))
            {
                enemyBrain = eBrain;
            }
        }
        if(enemyBrain != null && !enemyBrain.activate)
        {
            //만약 조준한 적이 죽었다면 새로운 적을 찾는다
            if (brain.CheckNearestEnemyBrain(out EnemyBrain eBrain))
            {
                enemyBrain = eBrain;
            }
        }
        //둘다 아니면 다음으로

        //조준 
        if (enemyBrain == null) return;

        Vector2 targetVec = enemyBrain.transform.position - muzzlePos.position;
        Vector2 upVec = Quaternion.Euler(0, 0, 90) * targetVec.normalized;
        Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, upVec);
        muzzlePos.rotation = targetRot;

        //발사 
        if (!action.OnAttackCool)
            action.Attack(muzzlePos.position, targetRot);

    }

}
