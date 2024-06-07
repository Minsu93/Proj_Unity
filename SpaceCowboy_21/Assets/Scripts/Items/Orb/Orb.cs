using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Orb : MonoBehaviour, IHitable , ITarget
{
    /// <summary>
    /// 아이콘 변경될 일 없음. . 
    /// 체력. 
    /// 생성 수 >> 생성기에서 읽음.
    /// 생성 조건 >> 생성기에서 읽음.
    /// 효과 : 터졌을 때 효과. 
    /// </summary>
    [Header("Popper")]
    public int orbNumbers = 1;  //생성되는 오브의 수
    public int energyNeeded = 15;   //생성에 필요한 에너지의 양, 혹은 오브 쿨타임
    [Header("Orb")]
    //[SerializeField] private int orbHealth = 1;     //오브 최대 발동시킬수 있는 양, Hit당 한번
    [SerializeField] private int orbReActiveCount = 1;      //오브의 반복 활성화 횟수. 
    int repeatCount;
    [SerializeField] private float orbDuration = 8.0f; //오브 지속시간.
    public ParticleSystem deadEffect;
    
    [SerializeField] GameObject viewObj;
    
    bool activate = false;
    Health health;
    Collider2D coll; 


    void Awake()
    {
        health = GetComponent<Health>();
        health.ResetHealth();

        coll = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        ResetOrb();
        StartCoroutine(DeactivateRoutine(orbDuration));
    }

    void ResetOrb()
    {
        health.ResetHealth();
        activate = true;
        coll.enabled = true;
        viewObj.SetActive(true);
        repeatCount = 0;
    }

    public void DamageEvent(float damage, Vector2 hitPoint)
    {
        if (!activate) return;

        if (health.AnyDamage(1))
        {
            //맞는 효과 
            //오브 사용 효과

            if (health.IsDead())
            {
                ActivateOrb();
                repeatCount++;
                if(repeatCount >= orbReActiveCount)
                {
                    DeactivateOrb();
                }
                else
                {
                    health.ResetHealth();
                }
            }
        }
    }

    protected abstract void ActivateOrb();


    IEnumerator DeactivateRoutine(float sec)
    {
        yield return new WaitForSeconds(sec);
        DeactivateOrb();
    }

    //오브 사라짐
    void DeactivateOrb()
    {
        activate = false;
        coll.enabled = false;
        viewObj.SetActive(false);
        if (deadEffect != null) GameManager.Instance.particleManager.GetParticle(deadEffect, transform.position, transform.rotation);

        //3초 후 오브 Deactive
        StartCoroutine(DieRoutine());
    }


    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);
    }

    public Collider2D GetCollider()
    {
        return coll;
    }

    public void KnockBackEvent(Vector2 hitPos, float forceAmount)
    {
        throw new NotImplementedException();
    }
}
