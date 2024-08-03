using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelfCollectable : Collectable
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (ConsumeEffect())
            {
                GameManager.Instance.particleManager.GetParticle(consumeEffect, transform.position, transform.rotation);
                gameObject.SetActive(false);
            }
        }
    }

    protected abstract bool ConsumeEffect();
}
