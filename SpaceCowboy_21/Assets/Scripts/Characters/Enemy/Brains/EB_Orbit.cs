using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EB_Orbit : EnemyBrain
{
    /// �˵� ����. �˵��� ������ �ӵ��� ����, �÷��̾� �������� ����ü�� ��ų� �߰� ���� ������ ��ȯ�Ѵ�. 

    bool inAttackOnce;   //ĳ���� ȸ����.
    float prePlayerDistance;
    Vector2 observedPos;

    EA_Orbit orbitAction;

    protected override void Awake()
    {
        base.Awake();
        orbitAction = (EA_Orbit)action;
    }

    public override void BrainStateChange()
    {
        //���¿� ���� Ȱ�� 
        switch (enemyState)
        {
            //�÷��̾ �ִ� �������� �̵�.
            case EnemyState.Chase:
                if (inAttackRange && isVisible)
                {
                    ChangeState(EnemyState.Attack, 0);
                }
                break;

            //�������鼭 ����
            case EnemyState.Attack:

                if (!inAttackRange || !isVisible) 
                {
                    ChangeState(EnemyState.Chase, 0);
                }
                break;
        }
    }



}
