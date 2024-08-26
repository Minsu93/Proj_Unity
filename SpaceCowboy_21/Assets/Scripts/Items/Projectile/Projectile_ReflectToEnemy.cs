using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Projectile_ReflectToEnemy : Projectile_Player
{
    [SerializeField] int maxReflect = 1;    //반사 횟수
    [SerializeField] float reflectCheckRadius = 10f;
    int reflectCount;
    float resetDist;    //튕겨나는 총알의 거리.
    List<Transform> hittedTargets = new List<Transform>();

    public override void Init(float damage, float speed, float lifeTime, float distance)
    {
        base.Init(damage, speed, lifeTime, distance);
        resetDist = distance;
        reflectCount = maxReflect;
        hittedTargets.Clear();
    }

    protected override void OverlapTarget(Collider2D collision)
    {
        Transform tr = collision.transform;
        if (collision.CompareTag("ProjHitCollider"))
        {
            tr = collision.transform.parent;
        }

        if (tr.TryGetComponent<ITarget>(out ITarget target))
        {
            if (tr.TryGetComponent<IHitable>(out IHitable hitable))
            {
                hittedTargets.Add(collision.transform); //수정한 부분
                HitEvent(target, hitable);
            }
            else
            {
                NonHitEvent(target);
            }

            WeaponImpactEvent();
        }
    }

    protected override void HitEvent(ITarget target, IHitable hitable)
    {
        hitable.DamageEvent(damage, transform.position);

        ShowHitEffect(hitEffect);

        //반사적용
        if (reflectCount > 0)
        {
            reflectCount--;

            if (ReflectProjectile())
            {
                //반사 (초기화) 성공
                return;
            }
            else
            {
                //근처에 적이 없음
                AfterHitEvent();
            }
        }
        else
        {
            AfterHitEvent();
        }
    }

    protected override void NonHitEvent(ITarget target)
    {

        ShowHitEffect(nonHitEffect);

        AfterHitEvent();

    }

    bool ReflectProjectile()
    {
        //근처의 적을 찾는다. 
        Transform targetTr = FindNearEnemy();

        if (targetTr)
        {
            //근처 적 방향으로 각도를 튼다. 
            Vector2 reflectDirection = (targetTr.position - transform.position).normalized;

            transform.rotation = Quaternion.LookRotation(Vector3.forward, Quaternion.Euler(0, 0, 90) * reflectDirection);
            distance = reflectCheckRadius + 0.5f;
            projectileMovement.StartMovement(speed);

            Debug.DrawLine(transform.position, targetTr.position, Color.yellow, 1.0f);
            return true;
        }
        else return false;
    }

    Transform FindNearEnemy()
    {
        int targetLayer = 1 << LayerMask.NameToLayer("Enemy");
        Transform tr = null;

        Collider2D[] colls = new Collider2D[10];
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, reflectCheckRadius, colls, targetLayer);
        if (count > 0)
        {
            float minDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                //이미 타격했던 대상인지 확인.
                if (hittedTargets.Contains(colls[i].transform))
                {
                    continue;
                }

                //장애물이 없는지 확인
                Vector2 vec = colls[i].transform.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, vec.normalized, reflectCheckRadius, LayerMask.GetMask("Planet"));
                if (hit.collider != null)
                    continue;

                //타겟 확인
                float dist = Vector2.Distance(transform.position, colls[i].transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    tr = colls[i].transform;
                }
            }
        }
        return tr;
    }
}
