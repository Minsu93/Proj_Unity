using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileMovement : MonoBehaviour
{
    protected float speed;

    protected Rigidbody2D rb;
    protected Projectile proj;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        proj = GetComponent<Projectile>();
    }

    public abstract void StartMovement(float speed);


    public abstract void StopMovement();

    public void AddImpulseToProj(Vector2 force)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

}
