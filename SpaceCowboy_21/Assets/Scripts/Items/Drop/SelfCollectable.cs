using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelfCollectable : Collectable
{
    [SerializeField] protected float lifeTime = 10.0f;
    float lifeTimer;
    Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        lifeTimer = lifeTime;
    }

    private void Update()
    {
        if(lifeTimer > 0)
        {
            lifeTimer -= Time.deltaTime;

            if(lifeTimer <= 0 )
            {
                LifeTimerOver();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (ConsumeEvent())
            {
                GameManager.Instance.particleManager.GetParticle(consumeEffect, transform.position, transform.rotation);
                gameObject.SetActive(false);
            }
        }
    }

    protected abstract bool ConsumeEvent();

    void LifeTimerOver()
    {
        Debug.Log("Deactivate");
        //�ִϸ��̼� ���
        if(animator!= null)
            animator.SetTrigger("Deactivate");
        //�ӽ�
        Invoke("DeActivate", 1.0f);
    }
    void DeActivate()
    {
        this.gameObject.SetActive(false);
    }
}
