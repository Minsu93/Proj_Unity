using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 활성화되면 총을 쏘기 시작한다. 총을 쏠 수록 총알이 소모되며 총에 맞으면 다시 총알을 채워줄 수 있다. 
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

            //초과될 시 
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
        //시간 체크
        checkTimer += Time.deltaTime;
        if (checkTimer >= enemyCheckInterval)
        {
            //적 체크
            checkTimer = 0;
            targetTr = CheckEnemyIsNear();
        }

        if (targetTr != null && curAmmo > 0)
        {
            //발사 
            GunAttack(targetTr);
        }
    }

    protected void GunAttack(Transform targetTr)
    {
        if (Time.time - lastShootTime > attackProperty.shootCoolTime)
        {
            //쏜 시간 체크
            lastShootTime = Time.time;

            //사격
            curAmmo--;
            if (curAmmo < maxAmmo)
            {
                projHitColl.enabled = true;
            }
            StartCoroutine(burstShootRoutine(targetTr));
        }
    }
}
