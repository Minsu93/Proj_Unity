using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Orb : MonoBehaviour, IHitable
{
    /// <summary>
    /// 아이콘 변경될 일 없음. . 
    /// 체력. 
    /// 생성 수 >> 생성기에서 읽음.
    /// 생성 조건 >> 생성기에서 읽음.
    /// 효과 : 터졌을 때 효과. 
    /// </summary>

    public int orbNumbers = 1;  //생성되는 오브의 수
    public int energyNeeded = 15;   //생성에 필요한 에너지의 양 
    public ParticleSystem deadEffect;
    
    [SerializeField] GameObject viewObj;

    bool activate = false;
    Health health;
    Collider2D coll; 


    void Awake()
    {
        health = GetComponent<Health>();
        coll = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        ResetOrb();
    }

    void ResetOrb()
    {
        activate = true;
        coll.enabled = true;
        viewObj.SetActive(true);
    }

    public void DamageEvent(float damage, Vector2 hitPoint)
    {
        if (!activate) return;

        if (health.AnyDamage(damage))
        {
            //맞는 효과 

            if (health.IsDead())
            {
                activate = false;
                coll.enabled = false;
                viewObj.SetActive(false);
                if (deadEffect != null) ParticleHolder.instance.GetParticle(deadEffect, transform.position, transform.rotation);
                //오브 사용 효과
                WhenDieEvent();
                //3초 후 오브 Deactive
                StartCoroutine(DieRoutine());
            }
        }
    }

    protected abstract void WhenDieEvent();

    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);
    }

}
