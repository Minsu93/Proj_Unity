using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_P_Bomb : ProjectileMovement
{
    [SerializeField] private AnimationCurve throwCurve;

    public event System.Action<Vector2> BombExplodeEvent;
    public override void StartMovement(float speed)
    {
        return;
    }

    public void StartMovement(float speed, float distance)
    {
        StartCoroutine(MoveAndExplode(speed, distance));
    }

    IEnumerator MoveAndExplode(float speed, float distance)
    {
        Vector2 startPos = transform.position;
        Vector3 targetPosition = transform.position + (transform.right * distance);
        float time = 0f;
        ///�� * �� = ��
        float throwTravelTime = distance / speed;

        // ������ ��ǥ ��ġ�� ������ ������ �̵��մϴ�.
        while (time <= throwTravelTime)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPosition, throwCurve.Evaluate(time / throwTravelTime));
            transform.position = pos;
            yield return null;
        }

        //���� �̺�Ʈ 
        if (BombExplodeEvent != null) BombExplodeEvent(this.transform.position);
        this.gameObject.SetActive(false);
    }


    public override void StopMovement()
    {
        StopAllCoroutines();
    }


}
