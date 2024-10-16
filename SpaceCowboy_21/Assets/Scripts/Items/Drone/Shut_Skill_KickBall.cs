using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����⸦ �� �ݴ�������� ���ƴٴѴ�. ȭ���� �����ڸ��� ������ ƨ���. ���ͷ��� �ϴ� ���鿡�� ���ظ� �ش�. 
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
    //            Debug.Log("�ݻ��" + rb.velocity);
    //            rb.velocity = vel;
    //        }
    //        else
    //        {
    //            Debug.Log("�� �������� ȸ��" + rb.velocity);
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
    //            //�������� ����
    //            if (tr.TryGetComponent<IHitable>(out IHitable hitable))
    //            {
    //                hitable.DamageEvent(damage, transform.position);

    //                GameManager.Instance.particleManager.GetParticle(hitEffect, transform.position, transform.rotation);
    //            }
    //            //�� �˹��Ű�� (�ʿ��ұ�?)
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
    //        //�Ѿ˿� �¾� ������ ������ > �Ѿ� Ÿ�� ����
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


    ////��Ʋ�� ȭ�� ������ ������ �� 
    //float screenBorderLimit = 0.5f;

    //bool OutSideScreenBorder(ref Vector2 vel)
    //{
    //    //ȭ�� ������ �������� üũ
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

    //    //�������� ȭ�� ������ ��ġ ����
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

    //    //ȭ�� ���� ���� üũ
    //    Collider2D[] colls = new Collider2D[maxEnemyCount];
    //    Vector2 size = GetScreenSize(); // �Ź� �� �ʿ� ����(���� �ʿ�)

    //    //ContactFIlter ����
    //    ContactFilter2D filter2D = new ContactFilter2D();
    //    filter2D.useTriggers = true;
    //    filter2D.SetLayerMask(targetLayer);
        
    //    int count = Physics2D.OverlapBox(rb.position, size, 0, filter2D, colls);
    //    if (count > 0)
    //    {
    //        float minDist = float.MaxValue;

    //        for (int i = 0; i < count; i++)
    //        {
    //            //Ÿ�� Ȯ��
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
    //    // ȭ���� ���� �Ʒ� �ڳʸ� ���� ��ǥ�� ��ȯ
    //    Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));

    //    // ȭ���� ������ �� �ڳʸ� ���� ��ǥ�� ��ȯ
    //    Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

    //    // ���� ũ�� ���
    //    float screenWidthInUnits = topRight.x - bottomLeft.x;
    //    float screenHeightInUnits = topRight.y - bottomLeft.y;

    //    float Border = 2.0f;
    //    return new Vector2(screenWidthInUnits - (2 * Border), screenHeightInUnits - (2 * Border));
    //}
}
