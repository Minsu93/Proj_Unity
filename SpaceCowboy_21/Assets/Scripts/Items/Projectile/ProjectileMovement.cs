using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileMovement : MonoBehaviour
{
    protected float speed;
    //public bool rotateBySpeed = false;

    protected Rigidbody2D rb;
    protected Projectile proj;




    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        proj = GetComponent<Projectile>();

    }

    //움직이기 시작할 때 
    public abstract void StartMovement(float speed);


    public abstract void StopMovement();




    //protected void RotateBySpeed()
    //{
    //    //투사체는 속도에 따라서 회전 회전
    //    Vector2 direction = rb.velocity.normalized;
    //    Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * direction;
    //    Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

    //    transform.rotation = targetRotation;
    //}




}
