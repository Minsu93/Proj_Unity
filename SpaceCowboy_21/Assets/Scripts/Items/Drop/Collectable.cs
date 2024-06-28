using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public ParticleSystem consumeEffect;
    //protected CircleCollider2D collectCollider;
    protected Gravity gravity;
    protected Rigidbody2D rb;

    protected virtual void Awake()
    {
        gravity = GetComponent<Gravity>();
        rb = GetComponent<Rigidbody2D>();
    }
    protected virtual void OnEnable()
    {
        return;
    }
    protected virtual void OnDisable()
    {
        return;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ConsumeEffect();

            GameManager.Instance.particleManager.GetParticle(consumeEffect, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }

    protected abstract void ConsumeEffect();
}
