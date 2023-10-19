using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : EnemyAction
{
    [Header("Trap")]
    public bool damagable ;     //데미지를 줄 수 있는가
    public float trapDamage;
    public float pushTime;          //밀치는 시간
    public float pushSpeed;         //밀치는 힘

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
                //데미지 전달
                if(damagable)
                    playerBehavior.DamageEvent(trapDamage);

                playerBehavior.PlayerKnockBack(transform.position, pushTime, pushSpeed);
            }

        }
    }

}
