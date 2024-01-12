using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class Weight : MonoBehaviour
{
    /// <summary>
    /// GravityLasso 로 던질 수 있는지 알려주는 Comp. 
    /// 던지기 관련 정보들이 들어가있다. 
    /// </summary>

    public bool canThrow = false; //이 오브젝트를 던질 수 있나요? 
    public float radius = 1f;
    public WeightState weightState = WeightState.Normal;
    
    public int weight = 1; //무게가 1이라는 것은 q를 한번만 눌러도 던져진다는 뜻. 
    int curWeight; //현재까지 모인 에너지 

    public float throwSpeed = 1f; //던졌을 때 날아가는 속도 
    float curSpeed;

    public float throwTime = 1f;    //몇초동안 날아갈 것이냐 
    float timer;

    public bool isRangeLimited = false; //움직일 수 있는 범위가 제한적인가?(Planet, FOBJ)
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
        //던지기 상태일 때 

        switch (weightState)
        {
            case WeightState.Normal:

                if (curSpeed > 0.1f)
                {
                    rb.MovePosition(rb.position + curSpeed * throwVec * Time.deltaTime);
                    curSpeed *= 0.98f; //감소수치
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
        //올가미에 잡혔을 때 

        //본체의 중력 영향을 안받게 조절. 
    }

    public void ReleaseOBJ()
    {
        //올가미에서 놓여졌을 때 
        weightState = WeightState.Normal;
    }

    public void ThrowCharge(Vector2 dir)
    {
        //Q를 눌러 던지기 파워를 모은다. 
        curWeight += 1;

        if(curWeight >= weight)
        {
            ThrowOBJ(dir);
        }
    }

    public void ChargeReset()
    {
        //올가미가 해제되거나 던져진 후, 던지기 파워가 리셋됨

        //throwVec = Vector2.zero;
        curWeight = 0;


    }
    public void ThrowOBJ(Vector2 v)
    {
        //파워가 weight를 넘어섰을 때, 던져지거나 다른 효과(무장 해제, 발판 생성 등)가 실행된다. 
        throwVec = v;
        curSpeed = throwSpeed;
        timer = throwTime;
        prePos = transform.position;

        //이벤트 
        weightState = WeightState.OnThrow;


        //던져진 이후 
        ChargeReset();
    }

    void HitCheck()
    {
        //던졌을 때 대상과 충돌 체크
        Vector2 vec = (Vector2)transform.position - prePos;

        //이 물체의 범위를 구한다. 
        RaycastHit2D[] hits = Physics2D.CircleCastAll(prePos, radius, vec.normalized, vec.magnitude, targetLayer);
        if(hits.Length > 0)
        {
            foreach(RaycastHit2D hit in hits)
            {
                if(hit.transform != this.transform)
                {
                    //물체를 정지시킨다. 
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
            //        //물체를 정지시킨다. 
            //        //curSpeed = 0f;
            //        weightState = WeightState.Normal;
            //        break;
            //    }
            //}

            //물체를 정지시킨다. 
            curSpeed = 0f;
            weightState = WeightState.Normal;
            //break;
        }
    }

    //public void ThrowDamageSelfNOther()
    //{
    //    //던졌을 때 스스로와 적에게 피해를 주는 함수 
    //}
    public void GrabOBJEvent()
    {
        //잡혀있는 동안  벌어지는 이벤트 
    }
    public void ThrowHitSomething()
    {
        //던져져서 무언가와 부딪혔을 때 발생하는 이벤트 
    }

    //던져지는 동안일 때 
    public enum WeightState { Normal, OnThrow}



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
