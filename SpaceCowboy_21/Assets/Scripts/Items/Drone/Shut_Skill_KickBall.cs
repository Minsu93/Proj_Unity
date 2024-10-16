using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 발차기를 한 반대방향으로 날아다닌다. 화면의 가장자리에 맞으면 튕긴다. 인터렉션 하는 적들에게 피해를 준다. 
/// </summary>
public class Shut_Skill_KickBall : MonoBehaviour
{
    //[Header("KickBall Property")]
    //[SerializeField] float moveSpeed = 5.0f;
    //float _mSpd;
    //private void FixedUpdate()
    //{
    //    if (!useStart) return;

    //    Vector2 vel = rb.velocity;
    //    if (OutSideScreenBorder(ref vel))
    //    {
    //        Transform targetTr = FindNearEnemy();
    //        if (targetTr == null)
    //        {
    //            Debug.Log("반사됨" + rb.velocity);
    //            rb.velocity = vel;
    //        }
    //        else
    //        {
    //            Debug.Log("적 방향으로 회전" + rb.velocity);
    //            Vector2 dir = ((Vector2)targetTr.position - rb.position).normalized;
    //            rb.velocity = dir * _mSpd;
    //        }
    //    }
    //}

    //[Header("Trigger Collider")]
    //[SerializeField] float damage = 5.0f;
    //[SerializeField] ParticleSystem hitEffect;

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (!useStart) return;

    //    if (collision.CompareTag("Enemy"))
    //    {
    //        Transform tr = collision.transform;

    //        if (tr.TryGetComponent<ITarget>(out ITarget target))
    //        {
    //            //데미지를 전달
    //            if (tr.TryGetComponent<IHitable>(out IHitable hitable))
    //            {
    //                hitable.DamageEvent(damage, transform.position);

    //                GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);
    //            }
    //            //적 넉백시키기 (필요할까?)
    //            if (tr.TryGetComponent<IKickable>(out IKickable kickable))
    //            {
    //                kickable.Kicked(this.transform.position);
    //            }
    //        }
    //    }
    //}


    //[SerializeField] int maxChargeHit = 10;
    //int projHitCount = 0;
    //[SerializeField] float maxChargeMultiplier = 2.0f;
    //bool maxCharged = false;
    //[SerializeField] float sizeChargeMultiplier = 2.0f;

    //public override void DamageEvent(float damage, Vector2 hitPoint)
    //{
    //    if(!maxCharged && !useStart)
    //    {
    //        projHitCount++;
    //        //총알에 맞아 충전이 끝나면 > 총알 타격 중지
    //        if(projHitCount >= maxChargeHit)
    //        {
    //            maxCharged = true;
    //            this.transform.localScale = Vector3.one * sizeDefaultMultiplier * sizeChargeMultiplier;
    //            projHitColl.enabled = false;
    //        }
    //    }
    //}

    //public override void Kicked(Vector2 hitPos)
    //{
    //    Vector2 dir = ((Vector2)transform.position - hitPos).normalized;
    //    if (maxCharged)
    //    {
    //        _mSpd = moveSpeed * maxChargeMultiplier;
    //    }
    //    else
    //    {
    //        _mSpd = moveSpeed;
    //    }
    //    rb.velocity = dir * _mSpd;

    //    useStart = true;
    //}

    //public override void CompleteEvent()
    //{
    //    rb.velocity = Vector2.zero;
    //    projHitCount = 0;
    //    maxCharged = false;
    //}


    ////셔틀이 화면 밖으로 나갔을 때 
    //float screenBorderLimit = 0.5f;

    //bool OutSideScreenBorder(ref Vector2 vel)
    //{
    //    //화면 밖으로 나갔는지 체크
    //    bool outside = false;
    //    Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

    //    if (screenPosition.x < 0 - screenBorderLimit)
    //    {
    //        outside = true;
    //        screenPosition.x = 0;
    //        if(vel.x < 0)
    //        {
    //            vel.x = (Mathf.Abs(vel.x));
    //        }
    //    }
    //    else if (screenPosition.x > Camera.main.pixelWidth + screenBorderLimit)
    //    {
    //        outside = true;
    //        screenPosition.x = Camera.main.pixelWidth;
    //        if(vel.x > 0)
    //        {
    //            vel.x = -1f * (Mathf.Abs(vel.x));
    //        }
    //    }

    //    if (screenPosition.y < 0 - screenBorderLimit)
    //    {
    //        outside = true;
    //        screenPosition.y = 0;
    //        if(vel.y < 0)
    //        {
    //            vel.y = (Mathf.Abs(vel.y));
    //        }
            
    //    }
    //    else if (screenPosition.y > Camera.main.pixelHeight + screenBorderLimit)
    //    {
    //        outside = true;
    //        screenPosition.y = Camera.main.pixelHeight;
    //        if(vel.y > 0)
    //        {
    //            vel.y = -1f * (Mathf.Abs(vel.y));
    //        }
    //    }

    //    //나갔으면 화면 안으로 위치 제한
    //    if (outside)
    //    {
    //        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
    //        rb.position = worldPos;
    //    }

    //    return outside;
    //}


    //Transform FindNearEnemy()
    //{
    //    Transform tr = null;

    //    int targetLayer = 1 << LayerMask.NameToLayer("Enemy");
    //    int maxEnemyCount = 10;

    //    //화면 내부 적들 체크
    //    Collider2D[] colls = new Collider2D[maxEnemyCount];
    //    Vector2 size = GetScreenSize(); // 매번 할 필요 없음(수정 필요)

    //    //ContactFIlter 설정
    //    ContactFilter2D filter2D = new ContactFilter2D();
    //    filter2D.useTriggers = true;
    //    filter2D.SetLayerMask(targetLayer);
        
    //    int count = Physics2D.OverlapBox(rb.position, size, 0, filter2D, colls);
    //    if (count > 0)
    //    {
    //        float minDist = float.MaxValue;

    //        for (int i = 0; i < count; i++)
    //        {
    //            //타겟 확인
    //            float dist = Vector2.Distance(transform.position, colls[i].transform.position);
    //            if (dist < minDist)
    //            {
    //                minDist = dist;
    //                tr = colls[i].transform;
    //            }
    //        }
    //    }
    //    return tr;
    //}

    //Vector2 GetScreenSize()
    //{
    //    // 화면의 왼쪽 아래 코너를 월드 좌표로 변환
    //    Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));

    //    // 화면의 오른쪽 위 코너를 월드 좌표로 변환
    //    Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

    //    // 유닛 크기 계산
    //    float screenWidthInUnits = topRight.x - bottomLeft.x;
    //    float screenHeightInUnits = topRight.y - bottomLeft.y;

    //    float Border = 2.0f;
    //    return new Vector2(screenWidthInUnits - (2 * Border), screenHeightInUnits - (2 * Border));
    //}
}
