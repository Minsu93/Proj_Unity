using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public HealthState healthState = HealthState.Default;

    public float maxHealth = 1;
    public float currHealth {  get; private set; }

    public float maxShield = 1;
    public float currShield { get; private set; }
    public float delayToRegeneration = 1.0f;
    float dTimer;
    public float regenerateTime = 2.0f;
    float rTimer;


    public float InvincibleTime;
    float _invincibleTimer;


    //데미지 공식이 복잡해질 수 있다. 
    //방어력 관련, 속성 관련

    public void ResetHealth()
    {
        currHealth = maxHealth;
        currShield = maxShield;
    }

    public bool AnyDamage(float dmg)
    {   //맞아서 데미지를 입었는지 검사
        bool isHit = false;

        if (_invincibleTimer > 0)
        {
            return isHit;
        }


        //무적 시간 초기화
        _invincibleTimer = InvincibleTime;

        //피해를 입는다
        if(currShield > 0)
        {
            currShield -= dmg;
            if (currShield < 0) currShield = 0;
        }
        else if (currHealth > 0) 
        {
            currHealth -= dmg;
            if (currHealth < 0) currHealth = 0;
        }

        isHit = true;

        if(isHit)
        {
            dTimer = delayToRegeneration;
            rTimer = 0;
            healthState = HealthState.OnHit;
        }

        return isHit;
    }

    public bool IsDead()
    {   //죽었는지 검사
        bool isDead = false;


        if (currHealth <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }

        return isDead;
    }

    protected virtual void Update()
    {
        if (_invincibleTimer > 0)
        {
            _invincibleTimer -= Time.deltaTime;
        }


       switch(healthState)
        {
            case HealthState.Default: break;
            case HealthState.OnHit: 
                //맞으면 딜레이동안 기다리고 OnRegen상태로 넘어간다
                if(dTimer > 0)
                {
                    dTimer -= Time.deltaTime;
                    if(dTimer <= 0)
                    {
                        healthState = HealthState.OnRegen;
                    }
                }
                break;
            case HealthState.OnRegen: 
                if(currShield < maxShield)
                {
                    //실드를 주기적으로 회복한다
                    rTimer += Time.deltaTime;
                    if(rTimer > regenerateTime)
                    {
                        rTimer = 0;
                        currShield++;
                    }

                    //실드가 다 찼으면 기본 상태로 돌아간다
                    if (currShield >= maxShield)
                    {
                        healthState = HealthState.Default;
                    }
                }
                break;
            case HealthState.Dead: break;
        }
    }


    public enum HealthState { Default, OnHit, OnRegen, Dead}

}
