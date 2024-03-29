using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ground_Jumper : EA_Ground
{
    public Vector2 jumpAttackDirVec;
    public float jumpAttackForce = 10f;
    protected override IEnumerator AttackCoroutine()
    {
        //ĳ���� ��ġ�� ȸ��.
        faceRight = Vector2.SignedAngle(transform.up, brain.playerDirection) < 0 ? true : false;
        FlipToDirectionView();

        yield return StartCoroutine(DelayRoutine(preAttackDelay));

        //���� ����
        JumpAttack();

        yield return StartCoroutine(DelayRoutine(afterAttackDelay));
    }

    void JumpAttack()
    {
        //�������� ���� ���� �Ѵ�. 

        int right = brain.playerIsRight ? 1 : -1;
        Vector2 jumpDir = (transform.up * jumpAttackDirVec.y + (transform.right * right * jumpAttackDirVec.x)).normalized;

        onAir = true;
        lastJumpTime = Time.time;

        rb.AddForce(jumpDir * jumpAttackForce, ForceMode2D.Impulse);

    }
}
