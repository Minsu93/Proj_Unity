using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : EnemyAction
{
    [Header("Trap")]
    public bool damagable ;     //�������� �� �� �ִ°�
    public float trapDamage;
    public float pushTime;          //��ġ�� �ð�
    public float pushSpeed;         //��ġ�� ��

    PlayerBehavior playerBehavior;

    protected override void Awake()
    {
        playerBehavior = GameManager.Instance.player.GetComponent<PlayerBehavior>();

        return;
    }

    protected override void Update()
    {
        return;
    }

    public override void ChaseAction()
    {
        return;
    }

    public override void AttackAction()
    {
        return;
    }

    public override void DieAction()
    {
        StopAllCoroutines();
        return;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (playerBehavior != null)
            {
                //������ ����
                if(damagable)
                    playerBehavior.DamageEvent(trapDamage);

                playerBehavior.PlayerKnockBack(transform.position, pushTime, pushSpeed);
            }

        }
    }

}
