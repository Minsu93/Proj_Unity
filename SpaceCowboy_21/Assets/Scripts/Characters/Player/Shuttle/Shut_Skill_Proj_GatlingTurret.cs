using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ȱ��ȭ�Ǹ� ���� ��� �����Ѵ�. ���� �� ���� �Ѿ��� �Ҹ�Ǹ� �ѿ� ������ �ٽ� �Ѿ��� ä���� �� �ִ�. 
/// </summary>
public class Shut_Skill_Proj_GatlingTurret : Shut_Skill_Proj
{

    [SerializeField] int ammoPerHit = 10;


    public override void ActivateShuttleSkill(ResetDel del)
    {
        base.ActivateShuttleSkill(del);
        curAmmo = maxAmmo;
        projHitColl.enabled = false;
    }



    public override void DamageEvent(float damage, Vector2 hitPoint)
    {
        if (curAmmo < maxAmmo)
        {
            curAmmo += ammoPerHit;

            //�ʰ��� �� 
            if(curAmmo >= maxAmmo)
            {
                curAmmo = maxAmmo;
                projHitColl.enabled = false;
            }
        }
    }

    public override void Kicked(Vector2 hitPos)
    {
        useStart = true;
    }

    public override void CompleteEvent()
    {
        return;
    }

    protected override void Update()
    {
        base.Update();

        if(useStart)
        {
            BaseAttackMethod();
        }
    }

    void BaseAttackMethod()
    {
        //�ð� üũ
        checkTimer += Time.deltaTime;
        if (checkTimer >= enemyCheckInterval)
        {
            //�� üũ
            checkTimer = 0;
            targetTr = CheckEnemyIsNear();
        }

        if (targetTr != null && curAmmo > 0)
        {
            //�߻� 
            GunAttack(targetTr);
        }
    }

    protected void GunAttack(Transform targetTr)
    {
        if (Time.time - lastShootTime > attackProperty.shootCoolTime)
        {
            //�� �ð� üũ
            lastShootTime = Time.time;

            //���
            curAmmo--;
            if (curAmmo < maxAmmo)
            {
                projHitColl.enabled = true;
            }
            StartCoroutine(burstShootRoutine(targetTr));
        }
    }
}
