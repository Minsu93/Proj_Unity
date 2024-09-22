using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabLasso : MonoBehaviour
{
    //private enum LassoState { Disable, Throw, Return, Dragged}
    //private LassoState lassoState = LassoState.Disable;
    //public float lassoTipSize = 0.3f;   //올가미 끝 크기(Grabbable 감지에 사용)
    ////public float chargeSpeed = 5f; //차지 속도 

    //[SerializeField] float throwTime;   //던질때 날아가는 시간
    //[SerializeField] AnimationCurve throwCurve; //던질 때 커브
    //public float throwDistance = 5f;    //던지는 최대 거리 
    //float dist;     //실제 던지는 거리 
    //Vector2 throwTargetPoint;   //실제 던지는 목표지점 

    //[SerializeField] float pullTime;    //당기동안 날아오는 시간
    //[SerializeField] AnimationCurve pullCurve;  //당길 때 커브
    //[SerializeField] GameObject lassoTip;   //끝부분
    //public float lassoCoolTime;  //올가미 재충전시간 

    //[SerializeField] float moveTime;
    //[SerializeField] AnimationCurve dragCurve; //던질 때 커브
    //Vector2 dragStartPosition;  //Drag 시작 위치
    //Vector2 dragStartVelocity;  //Drag 시작 속도
    

    //public bool lassoON { get; private set; }   //올가미가 활성화되었는지 확인용 함수 
    //float timer;
    ////float chargeGauge;
    //Transform grabTarget;
    //Vector3 grabLocalPos;
    
    //LineRenderer lineRenderer;

    //int targetLayer;
    //PlayerBehavior playerBehavior;
    //Rigidbody2D rb;

    //bool chargeON;  //라쏘 충전중인가요?

    //private void Start()
    //{
    //    playerBehavior = GetComponentInParent<PlayerBehavior>();    
    //    lineRenderer = GetComponent<LineRenderer>();
    //    rb = GetComponentInParent<Rigidbody2D>();

    //    lassoTip.SetActive(false);
    //    lineRenderer.enabled = false;

    //    targetLayer = 1 << LayerMask.NameToLayer("Object") | 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("SpaceBorder");
    //}

    //private void Update()
    //{

    //    switch (lassoState)
    //    {
    //        //올가미가 비활성화 상태일 때 
    //        case LassoState.Disable:

    //            ////충전할까요?
    //            //if (chargeON)
    //            //{
    //            //    ChargeLasso();
    //            //}
    //            ////마우스 입력 감지 
    //            //if (Input.GetMouseButtonDown(1))
    //            //{
    //            //    //충전 시작 
    //            //    chargeON = true;
    //            //}

    //            return;

    //        //올가미가 던지는 중일 때 
    //        case LassoState.Throw:
    //            //맞추는 대상 감지
    //            ThrowLasso();
    //            break;


    //        //올가미가 돌아오는 중일 때 
    //        case LassoState.Return:
    //            PullObject();
    //            break;

    //        //올가미가 행성을 맞췄을 때 
    //        case LassoState.Dragged:
    //            DraggedToPlanet();
    //            break;
    //    }

    //    UpdateLineRenderer();
    //}

    //public void TryThrowLasso()
    //{
    //    if(lassoState != LassoState.Disable) return;

    //    //// 올가미 던지기 
    //    //Debug.Log("Throw Power is : " + chargeGauge);

    //    //거리 계산 
    //    //dist = throwDistance * (chargeGauge / 100f);
    //    throwTargetPoint = (Vector2)transform.position + (throwDistance * playerBehavior.aimDirection);


    //    //// 충전 초기화
    //    //chargeON = false;
    //    //chargeGauge = 0;

    //    // 올가미 활성화
    //    InitializeLasso();

    //    //상태 전환
    //    lassoState = LassoState.Throw;

    //}

    ////올가미 활성화 
    //private void InitializeLasso()
    //{
    //    lassoTip.SetActive(true);
    //    lineRenderer.enabled = true;
    //    lassoON = true;
    //}


    ////올가미 비활성화
    //private void DisableLasso()
    //{
    //    lassoTip.SetActive(false);
    //    lineRenderer.enabled = false;
    //    lassoON = false;
    //}


    ////올가미 충전하기 
    ////void ChargeLasso()
    ////{
    ////    if(chargeGauge < 100f)
    ////    {
    ////        chargeGauge += chargeSpeed * Time.deltaTime;
    ////        if(chargeGauge >= 100f)
    ////        {
    ////            chargeGauge = Mathf.Clamp(chargeGauge, 0, 100f);
    ////        }
    ////    }
    ////}


    ////올가미 발사하기
    //void ThrowLasso()
    //{
    //    //올가미 움직인다. 
    //    timer += Time.deltaTime;
    //    float percent = timer / throwTime;

    //    Vector2 pos = Vector2.Lerp((Vector2)transform.position, throwTargetPoint, throwCurve.Evaluate(percent));
    //    lassoTip.transform.position = pos;


    //    //충돌하는 물체를 체크한다
    //    if (CheckGrabbables())
    //    {
    //        //뭔가를 감지한 경우
    //        return;
    //    }

        
    //    //아무것도 충돌하지 않고 최대 거리에 도착하면
    //    if (timer > throwTime)
    //    {
    //        timer = 0f;

    //        throwTargetPoint = pos;

    //        lassoState = LassoState.Return;
    //    }
    //}



    ////올가미로 끌어당길 수 있는 대상을 체크한다 
    //bool CheckGrabbables()
    //{
    //    bool grabObj = false;

    //    //레이를 마우스 위치로 발사해서 대상에 맞는지 체크 
    //    RaycastHit2D hit = Physics2D.CircleCast(lassoTip.transform.position, lassoTipSize, Vector2.right, 0f, targetLayer);
    //    if (hit.collider != null)
    //    {
    //        grabObj = true;

    //        if (hit.collider.CompareTag("Object"))
    //        {
    //            //대상이 Object인 경우 

    //            //필요한 변수들을 채워넣는다
    //            grabTarget = hit.transform;
    //            grabLocalPos = hit.transform.InverseTransformPoint(hit.point);

    //            //맞은 위치를 기억해놓는다
    //            throwTargetPoint = hit.transform.position;

    //            //올가미 움직인 시간 초기화
    //            timer = 0f;

    //            //올가미의 상태 변경
    //            lassoState = LassoState.Return;

    //        }
    //        else
    //        {
    //            //대상이 행성인 경우

    //            //필요한 변수들을 채워넣는다
    //            //grabTarget = hit.transform;
    //            //grabLocalPos = hit.transform.InverseTransformPoint(hit.point);
    //            dragStartPosition = transform.position;
    //            dragStartVelocity = rb.velocity * 0.5f;
    //            throwTargetPoint = hit.point;


    //            //맞은 위치를 기억해놓는다
    //            //throwTargetPoint = hit.transform.position;

    //            //올가미 움직인 시간 초기화
    //            timer = 0f;

    //            //올가미의 상태 변경
    //            //lassoState = LassoState.Dragged;

    //            //올가미 방향으로 대시하기
    //            Vector2 dir = hit.point - (Vector2)transform.position;
    //            float dashPower = 10f;
    //            rb.velocity = Vector2.zero;
    //            rb.AddForce(dir.normalized * dashPower, ForceMode2D.Impulse);


    //            //lassoState = LassoState.Return;
    //            lassoState = LassoState.Dragged;
    //        }
    //    }

    //    return grabObj;
    //}



    ////올가미를 당긴다, 취소한다. 
    //private void PullObject()
    //{
    //    //올가미는 거꾸로 돌아온다.
    //    timer += Time.deltaTime;

    //    float percent = timer / pullTime;
    //    Vector2 pos = Vector2.Lerp(throwTargetPoint, (Vector2)transform.position, pullCurve.Evaluate(percent));

    //    if (grabTarget)
    //    {
    //        grabTarget.position = pos - (Vector2)grabLocalPos;
    //    }

    //    lassoTip.transform.position = pos;


    //    //플레이어 근처에 도착하면
    //    if (timer > pullTime)
    //    {
    //        timer = 0;

    //        //Object를 사용합니다. 

    //        grabTarget = null;
            
    //        DisableLasso();

    //        lassoState = LassoState.Disable;
    //    }
    //}

    ////올가미를 타고 행성으로 날아간다. 
    //void DraggedToPlanet()
    //{
    //    //올가미를 걸으면, 현재 속도를 유지하면서 가속도를 서서히 받아서 최종적으로 원하는 시간에 원하는 위치에 도달하는것. 
    //    timer += Time.deltaTime;


    //    float percent = timer / moveTime;

    //    lassoTip.transform.position = throwTargetPoint;

    //    Vector2 maybePos = dragStartPosition + (dragStartVelocity * percent);
    //    Vector2 pos = Vector2.Lerp(maybePos, lassoTip.transform.position, pullCurve.Evaluate(percent));
    //    rb.MovePosition(pos);

    //    //Vector2 vec = throwTargetPoint - (Vector2)transform.position;
    //    //rb.AddForce(vec.normalized * 10f, ForceMode2D.Force);
    //    //lassoTip.transform.position = throwTargetPoint;

    //    //float dist = vec.magnitude;



    //    //목적지에 도착했을 때 
    //    if (timer > moveTime)
    //    {
    //        timer = 0;

    //        grabTarget = null;

    //        DisableLasso();

    //        lassoState = LassoState.Disable;

    //    }

    //    ////행성에 거의 가까이 가면
    //    //if (dist < 1f)
    //    //{
    //    //    timer = 0;

    //    //    //grabTarget = null;

    //    //    DisableLasso();

    //    //    lassoState = LassoState.Disable;

    //    //}
    //}



    ////linerenderer 업데이트
    //private void UpdateLineRenderer()
    //{
    //    //라인 랜더러 조정
    //    lineRenderer.SetPosition(0, transform.position);
    //    lineRenderer.SetPosition(1, lassoTip.transform.position);
    //}
}


