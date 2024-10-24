using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_ChargeLaser : WType_Charge
{
    public ParticleSystem nonHitEffect;

    [SerializeField] Material laserMat;
    [SerializeField] Color laserColor;
    List<LineRenderer> lasers = new List<LineRenderer>();
    [SerializeField] protected ParticleSystem hitEffect;

    public override void Initialize(WeaponData weaponData, Vector3 gunTipLocalPos)
    {
        base.Initialize(weaponData, gunTipLocalPos);

        for(int i = 0; i < numberOfProjectile; i++)
        {
            lasers.Add(CreateLasers(i));
        }
        
    }

    LineRenderer CreateLasers(int i)
    {
        GameObject obj = new GameObject();
        obj.transform.parent = transform;
        obj.name = "Laser_" + i;
        LineRenderer line = obj.AddComponent<LineRenderer>();
        if(laserMat != null) line.material = laserMat;
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;
        line.startColor = laserColor;
        line.endColor = laserColor;
        line.sortingLayerName = "Effect";
        obj.SetActive(false);
        return line;
    }


    protected override void ChargeShoot(Vector2 pos, Vector3 dir, float power)
    {
        float totalSpread = projectileSpread * (numberOfProjectile - 1);       //�켱 ��ü �Ѿ��� ���� ������ ���Ѵ�

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, -(totalSpread / 2)) * dir;       // ù �߻� ������ ���Ѵ�. 
       

        for (int i = 0; i < numberOfProjectile; i++)
        {
            float dist = range;
            Vector2 vec = Quaternion.Euler(0, 0, projectileSpread * (i)) * rotatedVectorToTarget;

            //��ֹ� �浹 ����
            RaycastHit2D hit = Physics2D.Raycast(pos, vec, range, LayerMask.GetMask("Planet"));
            if(hit.collider != null)
            {
                dist = hit.distance;
                NonHitEvent(hit.point);

            }

            //����Ʈ
            StartCoroutine(ShootLaserVFX(i, pos, pos + (vec * dist)));

            //���� ������
            RaycastHit2D[] enemyHits = Physics2D.RaycastAll(pos, vec, dist, LayerMask.GetMask("Enemy"));
            if(enemyHits.Length> 0)
            {
                for(int j = 0; j < enemyHits.Length; j++)
                {
                    if(enemyHits[j].transform.TryGetComponent<IHitable>(out IHitable hitable))
                    {
                        HitEvent(hitable, enemyHits[j].point, power);
                    }
                }
            }

        }

        lastShootTime = Time.time;
        //���� ����
        GameManager.Instance.audioManager.PlaySfx(shootSFX);
    }

    IEnumerator ShootLaserVFX(int index,Vector2 startPos, Vector2 endPos)
    {
        lasers[index].gameObject.SetActive(true);
        lasers[index].SetPosition(0, startPos);
        lasers[index].SetPosition(1, endPos);

        yield return new WaitForSeconds(0.5f);
        lasers[index ].gameObject.SetActive(false); 

    }


    protected virtual void HitEvent(IHitable hitable, Vector2 pos, float power)
    {

        hitable.DamageEvent(damage * power, transform.position);

        ShowHitEffect(hitEffect, pos);
    }

    protected virtual void NonHitEvent(Vector2 pos)
    {
        ShowHitEffect(nonHitEffect, pos);
    }



}
