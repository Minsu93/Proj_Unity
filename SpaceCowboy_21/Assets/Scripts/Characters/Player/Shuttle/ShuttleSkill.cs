using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 셔틀 스킬의 부모 
/// </summary>
public class ShuttleSkill : MonoBehaviour, ITarget, IHitable, IKickable
{
    public float skillCoolTime; // 셔틀 스킬 재사용 대기 시간. 완전히 사용이 끝난 이후부터 재사용 대기 시작이 차기 시작한다. 
    [SerializeField] CircleCollider2D coll;

    //초기화 시 
    void ShuttleInitialize()
    {
        coll.enabled = false;
    }


    //스킬 사용시 
    public void PressSkillButton(Rigidbody2D rb ,float maxDistance, float minSmoothTime, float maxSmoothTime)
    {
        //마우스 위치를 가져온다. 
        Vector2 mousePos = GameManager.Instance.playerManager.playerBehavior.mousePos;
        //위치로 이동시킨다. 
        MoveToTargetPosition(rb, mousePos, maxDistance, minSmoothTime,maxSmoothTime);
    }

    Vector2 velocity = Vector2.zero;

    //특정 위치로 이동
    void MoveToTargetPosition(Rigidbody2D rb,Vector2 targetPos, float maxDistance, float minSmoothTime, float maxSmoothTime)
    {
        Vector2 targetPosition = targetPos;

        // 현재 위치와 목표 위치 간의 거리
        float distance = Vector2.Distance(transform.position, targetPosition) / maxDistance;

        // 거리 기반으로 smoothTime 조절
        float smoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, distance);

        // 부드럽게 이동 (SmoothDamp)
        rb.MovePosition(Vector2.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));
    }



    //변신
    void TransformMethod()
    {
        coll.enabled = true;
    }

    //플레이어 총에 맞았을 때
    void HitByPlayerProj()
    {

    }

    //플레이어 발차기에 맞았을 때 
    void HitByPlayerKick()
    {

    }

    //대기시간 카운트
    void CountWaitTimer()
    {

    }

    //사용시간 카운트
    void CountUseTimer()
    {

    }

    //변신 해제
    void CompleteMethod()
    {

    }

    public Collider2D GetCollider()
    {
        throw new System.NotImplementedException();
    }

    public void DamageEvent(float damage, Vector2 hitPoint)
    {
        throw new System.NotImplementedException();
    }

    public void Kicked(Vector2 hitPos)
    {
        throw new System.NotImplementedException();
    }
}
