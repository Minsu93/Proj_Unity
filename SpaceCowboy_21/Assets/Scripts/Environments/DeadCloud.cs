using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadCloud : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Dead Player");
            var playerBehavior = collision.GetComponent<PlayerBehavior>();
            playerBehavior.DamageEvent(99f);
        }
    }
}
