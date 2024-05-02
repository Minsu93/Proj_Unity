using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Orb : MonoBehaviour, IHitable
{
    /// <summary>
    /// ������ ����� �� ����. . 
    /// ü��. 
    /// ���� �� >> �����⿡�� ����.
    /// ���� ���� >> �����⿡�� ����.
    /// ȿ�� : ������ �� ȿ��. 
    /// </summary>

    public int orbNumbers = 1;  //�����Ǵ� ������ ��
    public int energyNeeded = 15;   //������ �ʿ��� �������� �� 
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
            //�´� ȿ�� 

            if (health.IsDead())
            {
                activate = false;
                coll.enabled = false;
                viewObj.SetActive(false);
                if (deadEffect != null) ParticleHolder.instance.GetParticle(deadEffect, transform.position, transform.rotation);
                //���� ��� ȿ��
                WhenDieEvent();
                //3�� �� ���� Deactive
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
