using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Weight : MonoBehaviour
{
    /// <summary>
    /// GravityLasso �� ���� �� �ִ��� �˷��ִ� Comp. 
    /// ������ ���� �������� ���ִ�. 
    /// </summary>

    public bool canThrow = false; //�� ������Ʈ�� ���� �� �ֳ���? 
    public float radius = 1f;
    public WeightState weightState = WeightState.Normal;
    
    public int weight = 1; //���԰� 1�̶�� ���� q�� �ѹ��� ������ �������ٴ� ��. 
    int curWeight; //������� ���� ������ 

    public float throwSpeed = 1f; //������ �� ���ư��� �ӵ� 
    float curSpeed;

    public float throwTime = 1f;    //���ʵ��� ���ư� ���̳� 
    float timer;

    public bool isRangeLimited = false; //������ �� �ִ� ������ �������ΰ�?(Planet, FOBJ)
    Vector2 throwVec;

    Rigidbody2D rb;
    int targetLayer;
    Vector2 prePos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        targetLayer = 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("Enemy");
        prePos = transform.position;
    }

    private void Update()
    {
        if(curSpeed > 0)
        {
            HitCheckCast();

        }
        //if (weightState == WeightState.OnThrow)
        //{
        //    //HitCheck();

        //}
    }
    private void FixedUpdate()
    {
        //������ ������ �� 

        switch (weightState)
        {
            case WeightState.Normal:

                if (curSpeed > 0.1f)
                {
                    rb.MovePosition(rb.position + curSpeed * throwVec * Time.deltaTime);
                    curSpeed *= 0.98f; //���Ҽ�ġ
                }

                break;

            case WeightState.OnThrow:
                
                if (timer > 0)
                {
                    timer -= Time.fixedDeltaTime;
                    rb.MovePosition(rb.position + curSpeed * throwVec * Time.deltaTime);
                    if (timer <= 0)
                    {
                        timer = 0;
                        weightState = WeightState.Normal;
                    }
                }

                break;
        }

    }
    public void GrabOBJ()
    {
        //�ð��̿� ������ �� 

        //��ü�� �߷� ������ �ȹް� ����. 
    }

    public void ReleaseOBJ()
    {
        //�ð��̿��� �������� �� 
        weightState = WeightState.Normal;
    }

    public void ThrowCharge(Vector2 dir)
    {
        //Q�� ���� ������ �Ŀ��� ������. 
        curWeight += 1;

        if(curWeight >= weight)
        {
            ThrowOBJ(dir);
        }
    }

    public void ChargeReset()
    {
        //�ð��̰� �����ǰų� ������ ��, ������ �Ŀ��� ���µ�

        //throwVec = Vector2.zero;
        curWeight = 0;


    }
    public void ThrowOBJ(Vector2 v)
    {
        //�Ŀ��� weight�� �Ѿ�� ��, �������ų� �ٸ� ȿ��(���� ����, ���� ���� ��)�� ����ȴ�. 
        throwVec = v;
        curSpeed = throwSpeed;
        timer = throwTime;
        prePos = transform.position;

        //�̺�Ʈ 
        weightState = WeightState.OnThrow;


        //������ ���� 
        ChargeReset();
    }

    void HitCheck()
    {
        //������ �� ���� �浹 üũ
        Vector2 vec = (Vector2)transform.position - prePos;

        //�� ��ü�� ������ ���Ѵ�. 
        RaycastHit2D[] hits = Physics2D.CircleCastAll(prePos, radius, vec.normalized, vec.magnitude, targetLayer);
        if(hits.Length > 0)
        {
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.transform != this.transform)
                {
                    //��ü�� ������Ų��. 
                    curSpeed = 0f;
                    weightState = WeightState.Normal;
                    break;
                }
            }
        }

        prePos = transform.position;

    }
    void HitCheckCast()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, radius, throwVec, 3f, targetLayer);
        if(hits.Length > 1)
        {
            //Debug.Log(hits.Length);
            //Debug.Log("hits 0 is : " + hits[0].transform.name);
            
            //foreach(RaycastHit2D hit in hits)
            //{
            //    if (hit.transform != this.transform)
            //    {
            //        //��ü�� ������Ų��. 
            //        //curSpeed = 0f;
            //        weightState = WeightState.Normal;
            //        break;
            //    }
            //}

            //��ü�� ������Ų��. 
            curSpeed = 0f;
            weightState = WeightState.Normal;
            //break;
        }
    }

    //public void ThrowDamageSelfNOther()
    //{
    //    //������ �� �����ο� ������ ���ظ� �ִ� �Լ� 
    //}
    public void GrabOBJEvent()
    {
        //�����ִ� ����  �������� �̺�Ʈ 
    }
    public void ThrowHitSomething()
    {
        //�������� ���𰡿� �ε����� �� �߻��ϴ� �̺�Ʈ 
    }

    //�������� ������ �� 
    public enum WeightState { Normal, OnThrow}



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
