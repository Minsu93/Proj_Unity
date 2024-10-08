using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern_A_BaseAttack : BossPattern_A
{
    /// <summary>
    /// �Ϲ� ���. �÷��̾ ��ó�� ���̸� ����Ѵ�. 
    /// </summary>
    [SerializeField] ProjectileAttackProperty attackProperty;
    [SerializeField] float attackCheckRange = 20f;

    protected override bool Condition()
    {
        return PlayerIsNear(attackCheckRange) && PlayerIsVisible();
    }
    protected override IEnumerator PatternFunction()
    {
        yield return new WaitForSeconds(startDelay);

        Vector2 attackPos = transform.position;
        Vector2 playerPos = GameManager.Instance.player.position;

        // ���� ���� ���
        Vector2 direction = playerPos - attackPos;
        Vector2 upVec = new Vector2(-direction.y, direction.x);

        Quaternion attackTargetRot = Quaternion.LookRotation(Vector3.forward, upVec);
        yield return StartCoroutine(Shoot(attackPos, attackTargetRot, attackProperty));

        yield return new WaitForSeconds(endDelay);  
    }

}
