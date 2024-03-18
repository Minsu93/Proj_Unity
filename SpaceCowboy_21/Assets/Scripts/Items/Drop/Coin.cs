using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public ParticleSystem consumeEffect;
    void OnEnable()
    {
        Debug.Log("Enabled");
    }
    private void OnDisable()
    {
        Debug.Log("Disabled");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ParticleHolder.instance.GetParticle(consumeEffect, transform.position, transform.rotation);
            gameObject.SetActive(false);
        }
    }

}
