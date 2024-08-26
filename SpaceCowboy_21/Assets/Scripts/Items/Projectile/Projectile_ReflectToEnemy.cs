using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Projectile_ReflectToEnemy : Projectile_Player
{
    [SerializeField] int maxReflect = 1;    //�ݻ� Ƚ��
    [SerializeField] float reflectCheckRadius = 10f;
    int reflectCount;
    float resetDist;    //ƨ�ܳ��� �Ѿ��� �Ÿ�.
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
                hittedTargets.Add(collision.transform); //������ �κ�
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

        //�ݻ�����
        if (reflectCount > 0)
        {
            reflectCount--;

            if (ReflectProjectile())
            {
                //�ݻ� (�ʱ�ȭ) ����
                return;
            }
            else
            {
                //��ó�� ���� ����
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
        //��ó�� ���� ã�´�. 
        Transform targetTr = FindNearEnemy();

        if (targetTr)
        {
            //��ó �� �������� ������ ư��. 
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
                //�̹� Ÿ���ߴ� ������� Ȯ��.
                if (hittedTargets.Contains(colls[i].transform))
                {
                    continue;
                }

                //��ֹ��� ������ Ȯ��
                Vector2 vec = colls[i].transform.position - transform.position;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, vec.normalized, reflectCheckRadius, LayerMask.GetMask("Planet"));
                if (hit.collider != null)
                    continue;

                //Ÿ�� Ȯ��
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
