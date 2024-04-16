using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingPod : MonoBehaviour
{
    public float force;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerBehavior pb = collision.GetComponent<PlayerBehavior>();
            
            Vector2 vec = transform.up;
            pb.LauchPlayer(vec, force);
        }
    }

}
