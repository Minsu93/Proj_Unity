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
        ///속 * 시 = 거
        float throwTravelTime = distance / speed;

        // 폭죽이 목표 위치에 도달할 때까지 이동합니다.
        while (time <= throwTravelTime)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPosition, throwCurve.Evaluate(time / throwTravelTime));
            transform.position = pos;
            yield return null;
        }

        //폭발 이벤트 
        if (BombExplodeEvent != null) BombExplodeEvent(this.transform.position);
        this.gameObject.SetActive(false);
    }


    public override void StopMovement()
    {
        StopAllCoroutines();
    }


}
