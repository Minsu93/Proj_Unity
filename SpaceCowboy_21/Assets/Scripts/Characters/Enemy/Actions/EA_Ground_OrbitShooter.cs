using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EA_Ground_OrbitShooter : EA_Ground
{
    protected override IEnumerator AttackCoroutine()
    {
        //ĳ���� ��ġ�� ȸ��.
        faceRight = Vector2.SignedAngle(transform.up, brain.playerDirection) < 0 ? true : false;
        FlipToDirectionView();

        //���� ����
        AimOnView();

        yield return StartCoroutine(DelayRoutine(preAttackDelay));

        var guntip = enemyview_s.GetGunTipPos();
        ShootAction(guntip.Item1, guntip.Item2, 0, gravity.nearestPlanet, faceRight);

        yield return StartCoroutine(DelayRoutine(afterAttackDelay));

        //���� �Ϸ�
        AimOffView();
    }
}
