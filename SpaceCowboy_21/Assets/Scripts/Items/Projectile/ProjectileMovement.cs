using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileMovement : MonoBehaviour
{
    protected float speed;
    public bool rotateBySpeed = true;

    protected Rigidbody2D rb;
    protected Projectile proj;
    protected ProjectileGravity gravity;

    [Space]

    //�� ������Ʈ�� �˹��� �󸶳� �� ������ �����ִ� ������
    public float knockBackTime = 0.2f;
    protected float _knockBackTime; //���� ��� 
    public float knockBackForce = 2f;
    protected bool knockBackOn = false;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        proj = GetComponent<Projectile>();
        if (proj.gravityOn)
        {
            gravity = GetComponent<ProjectileGravity>();
        }
    }

    public abstract void StartMovement(float speed);


    public abstract void StopMovement();


    protected virtual void Update()
    {
        if(rotateBySpeed)
        {
            RotateBySpeed();
        }    
    }

    protected void RotateBySpeed()
    {
        //����ü�� �ӵ��� ���� ȸ�� ȸ��
        Vector2 direction = rb.velocity.normalized;
        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * direction;
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);

        transform.rotation = targetRotation;

    }


    public virtual void KnockBackEvent(Vector2 objVel)
    {

    }

}
