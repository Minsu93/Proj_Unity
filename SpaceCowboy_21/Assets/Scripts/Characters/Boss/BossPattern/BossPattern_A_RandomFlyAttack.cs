using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern_A_RandomFlyAttack : BossPattern_A
{
    [SerializeField] float moveDuration;
    [SerializeField] ProjectileAttackProperty attackProperty;
    [SerializeField] AnimationCurve animCurve;



    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    protected override bool Condition()
    {
        return true;
    }

    protected override IEnumerator PatternFunction()
    {
        yield return new WaitForSeconds(startDelay);

        //1. 이동한다. 
        Vector2 moveTargetPos = WaveManager.instance.GetRandomPointNearPlayer(2f, 10f);
        yield return StartCoroutine(FlyToPosition(moveTargetPos, moveDuration, rb, animCurve));

        //2. 대기
        yield return new WaitForSeconds(delay);

        //3. 사격한다
        Vector2 attackPos = transform.position;
        Vector2 playerPos = GameManager.Instance.player.position;

        // 방향 벡터 계산
        Vector2 direction = playerPos - attackPos;
        Vector2 upVec = new Vector2(-direction.y, direction.x);

        Quaternion attackTargetRot = Quaternion.LookRotation(Vector3.forward, upVec);
        yield return StartCoroutine(Shoot(attackPos, attackTargetRot, attackProperty));

        //4. 대기
        yield return new WaitForSeconds(endDelay);

    }
}
