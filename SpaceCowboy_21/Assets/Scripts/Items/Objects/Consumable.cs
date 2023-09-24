using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : MonoBehaviour
{
    public float delayForDestroy = 2.0f;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ConsumeFuction();
            StartCoroutine(DestroyFuction(delayForDestroy));
        }

        if (collision.CompareTag("Obstacle") || collision.CompareTag("Planet"))
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    protected virtual void ConsumeFuction()
    {

    }
    
    IEnumerator DestroyFuction(float delay)
    {
        this.gameObject.SetActive(false);
        yield return new WaitForSeconds(delay);
    }
}
