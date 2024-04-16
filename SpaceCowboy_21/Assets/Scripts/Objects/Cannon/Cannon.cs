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
        //�� ������Ʈ�� �����Ѵ�
        activate = true;
        if (headSpr != null) headSpr.sprite = turnOnSprites[0];
        if (neckSpr != null) neckSpr.sprite = turnOnSprites[1];

    }

    public void DeactivateObject()
    {
        //������Ʈ�� �񰡵��Ѵ�. 
        activate = false;
    }

    void Update()
    {
        if (!activate) return;

        //�� Ž��
        if (enemyBrain == null)
        {
            //���� ���ٸ� ���ο� ���� ã�´�.
            if (brain.CheckNearestEnemyBrain(out EnemyBrain eBrain))
            {
                enemyBrain = eBrain;
            }
        }
        if(enemyBrain != null && !enemyBrain.activate)
        {
            //���� ������ ���� �׾��ٸ� ���ο� ���� ã�´�
            if (brain.CheckNearestEnemyBrain(out EnemyBrain eBrain))
            {
                enemyBrain = eBrain;
            }
        }
        //�Ѵ� �ƴϸ� ��������

        //���� 
        if (enemyBrain == null) return;

        Vector2 targetVec = enemyBrain.transform.position - muzzlePos.position;
        Vector2 upVec = Quaternion.Euler(0, 0, 90) * targetVec.normalized;
        Quaternion targetRot = Quaternion.LookRotation(Vector3.forward, upVec);
        muzzlePos.rotation = targetRot;

        //�߻� 
        if (!action.OnAttackCool)
            action.Attack(muzzlePos.position, targetRot);

    }

}
