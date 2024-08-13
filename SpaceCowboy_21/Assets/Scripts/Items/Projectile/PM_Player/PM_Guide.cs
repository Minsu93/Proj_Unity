using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PM_Guide : ProjectileMovement
{
    //[SerializeField] int guideAmount = 1;
    [SerializeField] float guideRadius = 5f;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float guideInterval = 0.2f;
    float timer;
    float rspd;

    int targetLayer ;
    bool activate = false;
    Transform targetTr;

    protected override void Awake()
    {
        base.Awake();
        targetLayer = 1 << LayerMask.NameToLayer("Enemy");
    }

    //���� �Ѿ��� ���
    public override void StartMovement(float speed)
    {
        //���� ON
        this.speed = speed;
        activate = true;
        targetTr = null;
        timer = 0;
        rspd = rotationSpeed;

        //�����δ�
        rb.velocity = transform.right * speed;

    }


    public override void StopMovement()
    {
        if (activate) activate = false;
        rb.velocity = Vector2.zero;

    }

    private void Update()
    {
        if (targetTr) return;

        if(timer < guideInterval)
        {
            timer += Time.deltaTime;

        }else
        {
            timer = 0;
            targetTr = chaseNearEnemy();
        }
    }

    public void FixedUpdate()
    {
        if (!activate)
            return;

        if (targetTr)
        {
            //ȸ������ ���Ѵ�. 
            Vector3 targetVec = (targetTr.position - transform.position).normalized;
            Vector3 upVec = Quaternion.Euler(0, 0, 90) * targetVec;
            Quaternion targetRot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);

            float angle = Quaternion.Angle(transform.rotation, targetRot);
            angle = Mathf.Floor(Mathf.Clamp(angle, 0, 90f) / 30);
            Quaternion rot = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rspd * angle);

            //������ ȸ����Ų��. 
            rb.SetRotation(rot);

            rb.velocity = transform.right * speed;
        }

    }

    Transform chaseNearEnemy()
    {
        Transform tr = null;

        Collider2D[] colls = new Collider2D[10];
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, guideRadius, colls, targetLayer);
        if(count > 0)
        {
            float minDist = float.MaxValue;
            
            for(int i = 0; i < count; i++)
            {
                float dist = Vector2.Distance(transform.position, colls[i].transform.position);
                if(dist < minDist)
                {
                    minDist = dist;
                    tr = colls[i].transform;
                }
            }
        }
        return tr;
    }
}
