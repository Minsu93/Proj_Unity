using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Orb : MonoBehaviour, IHitable , ITarget
{
    /// <summary>
    /// ������ ����� �� ����. . 
    /// ü��. 
    /// ���� �� >> �����⿡�� ����.
    /// ���� ���� >> �����⿡�� ����.
    /// ȿ�� : ������ �� ȿ��. 
    /// </summary>
    [Header("Popper")]
    public int orbNumbers = 1;  //�����Ǵ� ������ ��
    public int energyNeeded = 15;   //������ �ʿ��� �������� ��, Ȥ�� ���� ��Ÿ��
    [Header("Orb")]
    //[SerializeField] private int orbHealth = 1;     //���� �ִ� �ߵ���ų�� �ִ� ��, Hit�� �ѹ�
    [SerializeField] private int orbReActiveCount = 1;      //������ �ݺ� Ȱ��ȭ Ƚ��. 
    int repeatCount;
    [SerializeField] private float orbDuration = 8.0f; //���� ���ӽð�.
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
            //�´� ȿ�� 
            //���� ��� ȿ��

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

    //���� �����
    void DeactivateOrb()
    {
        activate = false;
        coll.enabled = false;
        viewObj.SetActive(false);
        if (deadEffect != null) GameManager.Instance.particleManager.GetParticle(deadEffect, transform.position, transform.rotation);

        //3�� �� ���� Deactive
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
