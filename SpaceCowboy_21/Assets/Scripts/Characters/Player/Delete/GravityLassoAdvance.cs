using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLassoAdvance : MonoBehaviour
{
    //[SerializeField] public LassoState lassoState = LassoState.Disable;
    //[SerializeField] float throwTime;   //날아가는 시간
    //[SerializeField] float maxThrowDistance; //던지는 max 거리
    //[SerializeField] float pullTime;    //당기는 시간
    //float moveTime;    //대상 쪽으로 움직이는 시간
    //[SerializeField] float grabForce;   //당기는 힘
    //[SerializeField] float lassoLength = 3f;     //올가미 길이

    //[SerializeField] float moveSpeed;   // 대상 쪽으로 움직일 때의 속도
    //[SerializeField] float maxLassoDistance; //올가미 끊어지기 전까지 최대 길이 
    //[SerializeField] AnimationCurve throwCurve; //던질 때 커브
    //[SerializeField] AnimationCurve pullCurve;  //당길 때 커브

    //[SerializeField] GameObject lassoTip;
    //public Vector2 forceByLasso { get; set; }   //올가미로 받는 힘

    //float timer;
    //Vector2 throwStartPos;
    //Transform grabTarget;
    //Weight grabWeight;
    //Vector3 grabLocalPos;
    //float grabObjSize;
    //bool grabPlanet;
    //bool canLaunch;
    //float lassoDist;    //올가미 길이 
    //Vector2 targetDir;
    //Vector2 preLassoTipPos;
    //Vector2 moveStartVelocity;
    //Vector2 moveStartPosition;


    //LineRenderer lineRenderer;
    //Rigidbody2D rb;
    //Rigidbody2D grabTargetRb;

    //private void Start()
    //{
    //    lineRenderer = GetComponent<LineRenderer>();
    //    rb = GetComponentInParent<Rigidbody2D>();

    //    lassoTip.SetActive(false);
    //    lineRenderer.enabled = false;
    //}

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        if(lassoState == LassoState.OnGrab)
    //        {
    //            //대상을 던진다. 
    //            LaunchGrabObj();

    //            //if (canLaunch)
    //            //{
    //            //    //대상을 던진다. 
    //            //    LaunchGrabObj();
    //            //}

    //            ////올가미는 돌아온다.
    //            //grabTarget = null;
    //            ////올가미 당기기 시작 위치 설정
    //            //preLassoTipPos = lassoTip.transform.position;
    //            ////타이머 초기화
    //            //timer = 0;
    //            ////Grab힘 초기화
    //            //forceByLasso = Vector2.zero;
    //            ////올가미 상태를 당기는 것으로 변경한다. 
    //            //lassoState = LassoState.PullBack;
    //        }
    //    }

    //    if(Input.GetKeyDown(KeyCode.E) && lassoState == LassoState.OnGrab)
    //    {
    //        ////올가미는 돌아온다.
    //        //grabTarget = null;
    //        ////올가미 당기기 시작 위치 설정
    //        //preLassoTipPos = lassoTip.transform.position;
    //        ////타이머 초기화
    //        //timer = 0;
    //        ////Grab힘 초기화
    //        //forceByLasso = Vector2.zero;
    //        ////올가미 상태를 당기는 것으로 변경한다. 
    //        //lassoState = LassoState.PullBack;
    //        ReleaseLasso();
    //    }


    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        if(lassoState == LassoState.Disable)
    //        {
    //            //올가미 위치를 초기화한다.
    //            InitializeLasso();
    //            //타이머를 초기화한다
    //            timer = 0;
    //            //올가미 상태를 던지는 것으로 변경한다.
    //            lassoState = LassoState.Throw;

    //        }
    //        //switch (lassoState)
    //        //{
    //        //    case LassoState.Disable:
    //        //      //올가미 위치를 초기화한다.
    //                //InitializeLasso();
    //        //      //타이머를 초기화한다
    //        //      timer = 0;
    //        //      //올가미 상태를 던지는 것으로 변경한다.
    //        //      lassoState = LassoState.Throw;
    //        //}
    //    }

    //    if( Input.GetMouseButtonUp(1))
    //    {
    //        switch(lassoState)
    //        {
    //            case LassoState.Throw:
    //                //올가미를 던지는 중이라면, 움직임을 취소하고 다시 돌아오게 만든다.
    //                ////올가미 당기기 시작 위치 설정
    //                //preLassoTipPos = lassoTip.transform.position;
    //                ////타이머 초기화
    //                //timer = 0;
    //                ////올가미 상태를 당기는 것으로 변경한다. 
    //                //lassoState = LassoState.PullBack;
    //                ReleaseLasso();
    //                break;

    //            case LassoState.OnGrab:
    //                //타이머 초기화
    //                timer = 0;
    //                //forceByLasso = Vector2.zero;

    //                //필요한 정보들
    //                moveStartVelocity = rb.velocity;

    //                float t = (lassoTip.transform.position - transform.position).magnitude / moveSpeed;
    //                moveTime = Mathf.Clamp(t, 0.7f, 3f);

    //                moveStartPosition = transform.position;
    //                //올가미 상태를 대상 방향으로 움직임으로 변경한다. 
    //                lassoState = LassoState.OnMove;

    //                break;
    //        }
    //    }

    //    switch (lassoState)
    //    {
    //        case LassoState.Throw:
    //            ThrowLasso(targetDir);
    //            CollisionCheck();
    //            break;
    //        case LassoState.PullBack:
    //            PullLasso(preLassoTipPos);
    //            break;
    //        case LassoState.OnGrab:
    //            //lassoTip의 로컬 위치를 업데이트한다. 

    //            lassoTip.transform.position = grabTarget.TransformPoint(grabLocalPos);

    //            //if (grabPlanet)
    //            //{
    //            //    lassoTip.transform.position = grabTarget.TransformPoint(grabLocalPos);
    //            //}
    //            //else
    //            //{
    //            //    lassoTip.transform.position = grabTarget.transform.position;
    //            //}
    //            //일정 거리 이상 멀어지면 끊어짐. 
    //            CheckLassoDistance();


    //            break;
    //        case LassoState.OnMove:
    //            //lassoTip의 위치를 업데이트
    //            lassoTip.transform.position = grabTarget.TransformPoint(grabLocalPos);

    //            //if (grabPlanet)
    //            //{
    //            //    lassoTip.transform.position = grabTarget.TransformPoint(grabLocalPos);
    //            //}
    //            //else
    //            //{
    //            //    lassoTip.transform.position = grabTarget.transform.position;
    //            //}
    //            break;

    //        case LassoState.Disable:
    //            return;
    //    }

    //    UpdateLineRenderer();
    //}

    //private void FixedUpdate()
    //{
    //    //물리 이동 2개만 FixedUpdate에서.
    //    switch(lassoState)
    //    {
    //        case LassoState.OnGrab:
    //            // OnGrab();
    //            break;
    //        case LassoState.OnMove: 
    //            MoveToGrabPoint(moveStartPosition, moveStartVelocity);
    //            break;

    //    }

    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.collider.CompareTag("Planet"))
    //    {
    //        Debug.Log("Off Grab");

    //        //timer = 0;
    //        ////DisableLasso();
    //        //forceByLasso = Vector2.zero;
    //        //lassoState = LassoState.PullBack;
    //        ReleaseLasso();
    //    }
    //}


    ////올가미 초기화
    //private void InitializeLasso()
    //{
    //    lassoTip.SetActive (true);
    //    lineRenderer.enabled = true;
    //    lassoTip.transform.position = this.transform.position;

    //    Vector3 inputPos = Input.mousePosition;
    //    inputPos.z = 10;    //z는 카메라에서부터의 거리
    //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputPos);
    //    mousePos.z = 0;
    //    targetDir = (mousePos - transform.position).normalized;

    //    preLassoTipPos = transform.position;
    //    throwStartPos = transform.position;

    //}

    ////올가미 비활성화
    //private void DisableLasso()
    //{
    //    lassoTip.SetActive (false);
    //    lineRenderer.enabled = false;
    //    if (grabTarget)
    //    {
    //        grabTarget = null;
    //        grabWeight = null;
    //        grabLocalPos = Vector3.zero;
    //    }

    //}

    ////dir 방향으로 올가미를 던진다
    //private void ThrowLasso(Vector2 dir)
    //{
    //    //올가미 움직인다. 
    //    timer += Time.deltaTime;
    //    float percent = timer / throwTime;

    //    Vector2 pos = Vector2.Lerp((Vector2)transform.position, throwStartPos + (targetDir * maxThrowDistance), throwCurve.Evaluate(percent)); ;
    //    lassoTip.transform.position = pos;

    //    //아무것도 충돌하지 않고 최대 거리에 도착하면
    //    if (timer > throwTime)
    //    {
    //        //timer = 0;
    //        ////올가미 당기기 시작 위치 설정
    //        //preLassoTipPos = lassoTip.transform.position;

    //        //lassoState = LassoState.PullBack;
    //        ReleaseLasso();
    //    }
    //}

    //private void ReleaseLasso()
    //{
    //    //올가미는 돌아온다.
    //    grabTarget = null;

    //    if (grabWeight)
    //    {
    //        grabWeight.ReleaseOBJ();
    //        grabWeight = null;
    //    }

    //    //올가미 당기기 시작 위치 설정
    //    preLassoTipPos = lassoTip.transform.position;
    //    //타이머 초기화
    //    timer = 0;
    //    //Grab힘 초기화
    //    //forceByLasso = Vector2.zero;
    //    //올가미 상태를 당기는 것으로 변경한다. 
    //    lassoState = LassoState.PullBack;
    //}

    ////올가미를 당긴다, 취소한다. 
    //private void PullLasso(Vector2 lassoTipPos)
    //{
    //    //올가미는 거꾸로 돌아온다.
    //    timer += Time.deltaTime;

    //    //Vector2 dir = (lassoTip - (Vector2)transform.position);
    //    float percent = timer / pullTime;
    //    Vector2 pos = Vector2.Lerp(lassoTipPos, (Vector2)transform.position, pullCurve.Evaluate(percent));
    //    lassoTip.transform.position = pos;

    //    //플레이어 근처에 도착하면
    //    if (timer > pullTime)
    //    {
    //        timer = 0;
    //        DisableLasso();
    //        lassoState = LassoState.Disable;
    //    }
    //}


    ////올가미가 잡은 상태로 유지될 떄 
    //private void OnGrab()
    //{
    //    Vector2 vec = transform.position - lassoTip.transform.position;
    //    if(vec.magnitude > lassoLength)
    //    {
    //        Vector2 normalVec = vec.normalized;
    //        float f = (vec.magnitude - lassoLength) / 2f;
    //        float dragForce = grabForce / Mathf.Sqrt(grabObjSize);
            

    //        //forceByLasso = -1f * f * grabForce * normalVec;
    //        //rb.AddForce(forceByLasso, ForceMode2D.Force);

    //        //if(grabPlanet)
    //        //{
    //        //    //행성을 움직일때 
    //        //    if (grabTarget.TryGetComponent(out Planet planet))
    //        //    {
    //        //        planet.MovePlanetForce(normalVec * f * dragForce);
    //        //    }
    //        //}
    //        //else
    //        //{
    //        //    grabTargetRb.AddForce(normalVec * f * dragForce, ForceMode2D.Force);
    //        //}
    //    }

    //    ////잡아 끄는 방향에 일치하도록 회전시킨다.
    //    //Vector2 a = lassoTip.transform.position - grabTarget.transform.position;
    //    //float angle = Vector2.SignedAngle(vec, a);
    //    //var impulse = (-1f * 0.1f *  angle * Mathf.Deg2Rad) * grabTargetRb.inertia;
    //    //grabTargetRb.AddTorque(impulse, ForceMode2D.Force);

    //}

    //void CheckLassoDistance()
    //{
    //    //올가미 길이가 너무 길면 끊어짐. 
    //    if(lassoDist > maxLassoDistance)
    //    {
    //        ReleaseLasso();
    //    }
    //}



    ////올가미가 잡은 쪽으로 날아간다
    //private void MoveToGrabPoint(Vector2 startPos, Vector2 startVel)
    //{
    //    //올가미를 걸으면, 현재 속도를 유지하면서 가속도를 서서히 받아서 최종적으로 원하는 시간에 원하는 위치에 도달하는것. 
    //    timer += Time.fixedDeltaTime;
    //    float percent = timer / moveTime;



    //    Vector2 maybePos = startPos + (startVel * percent);
    //    Vector2 pos = Vector2.Lerp(maybePos, lassoTip.transform.position, pullCurve.Evaluate(percent));
    //    rb.MovePosition(pos);



    //    //목적지에 도착했을 때 
    //    if (timer > moveTime)
    //    {
    //        timer = 0;
    //        DisableLasso();
    //        lassoState = LassoState.Disable;

    //    }
    //}


    ////충돌 체크
    //private void CollisionCheck()
    //{
    //    int targetLayer = 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("FloatingObj");
    //    Vector2 v = (Vector2)lassoTip.transform.position - preLassoTipPos;
    //    Vector2 dir = v.normalized;
    //    float dist = v.magnitude;
    //    RaycastHit2D hit = Physics2D.CircleCast(preLassoTipPos, 0.3f, dir,dist,targetLayer);

    //    if(hit)
    //    {
    //        if(hit.transform.TryGetComponent(out Weight weight))
    //        {
    //            //무게comp가 있을 때 
    //            grabWeight = weight;
    //            grabWeight.GrabOBJ();

    //            //뭔가랑 충돌했다! 
    //            timer = 0;
    //            //충돌한 대상의 transform을 가져온다
    //            grabTarget = hit.transform;
    //            //로컬 포즈를 구한다. 
    //            grabLocalPos = hit.transform.InverseTransformPoint(hit.point); ;

    //            //상태를onGrab으로 변경한다
    //            lassoState = LassoState.OnGrab;
    //        }

    //        //Vector3 size = hit.collider.bounds.size;
    //        //grabObjSize = size.x * size.y;
            
    //        //if(grabObjSize < 25f)
    //        //{
    //        //    //물체 발사 가능
    //        //    canLaunch = true;
    //        //}
    //        //else
    //        //{
    //        //    canLaunch = false;
    //        //}


    //        //if (hit.collider.CompareTag("Planet"))
    //        //{
    //        //    grabPlanet = true;
    //        //}
    //        //else
    //        //{
    //        //    grabPlanet = false;
    //        //}

           
    //        //충돌시의 올가미 길이를 구한다. 
    //        //grabDist = (transform.position - hit.transform.position).magnitude;
    //        //충돌한 대상의 Rigidbody2D를 가져온다. 
    //        //grabTargetRb = grabTarget.GetComponent<Rigidbody2D>();



    //    }
    //    else
    //    {
    //        preLassoTipPos = lassoTip.transform.position;
    //    }
    //}

    ////잡은 물건을 마우스 방향으로 던진다. 
    //private void LaunchGrabObj()
    //{
    //    Vector3 inputPos = Input.mousePosition;
    //    inputPos.z = 10;    //z는 카메라에서부터의 거리
    //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputPos);
    //    mousePos.z = 0;

    //    Vector2 launchVec = (mousePos - grabTarget.position).normalized;
    //    //float launchForce = 50f / Mathf.Sqrt(grabObjSize);

    //    if (grabWeight)
    //    {
    //        grabWeight.ThrowCharge(launchVec);
    //    }

    //    //if (grabPlanet)
    //    //{
    //    //    //행성을 던질 때 
    //    //    //if(grabTarget.TryGetComponent(out Planet planet))
    //    //    //{
    //    //    //    planet.MovePlanet(launchVec * launchForce);
    //    //    //}
    //    //}
    //    //else
    //    //{
    //    //    //행성 이외의 것을 던질 때 
    //    //    //grabTargetRb.AddForce(launchVec * launchForce, ForceMode2D.Impulse);
    //    //}

    //    //canLaunch = false;
    //}

    ////linerenderer 업데이트
    //private void UpdateLineRenderer()
    //{

    //    //올가미 머리를 회전시킨다
    //    Vector2 tipVec = lassoTip.transform.position - transform.position;
    //    lassoDist = tipVec.magnitude;

    //    Vector2 tipDir = tipVec.normalized;

    //    Vector3 upVec = Quaternion.Euler(0, 0, 90) * tipDir;
    //    Quaternion rot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
    //    lassoTip.transform.rotation = rot;

    //    Vector2 p;
    //    ////대상이 있을 때 
    //    //if (grabTarget && !grabPlanet)
    //    //{
    //    //    p = grabTarget.transform.position;
    //    //    lineRenderer.SetPosition(0, transform.position);
    //    //    lineRenderer.SetPosition(1, p);
    //    //}
    //    //else
    //    //{
    //    //    //라인 랜더러 조정
    //    //    p = (Vector2)lassoTip.transform.position + (tipDir * -1f * 0.5f);
    //    //    lineRenderer.SetPosition(0, transform.position);
    //    //    lineRenderer.SetPosition(1, p);
    //    //}
        

    //    //라인 랜더러 조정
    //    p = (Vector2)lassoTip.transform.position + (tipDir * -1f * 0.5f);
    //    lineRenderer.SetPosition(0, transform.position);
    //    lineRenderer.SetPosition(1, p);

    //}
}

//public enum LassoState { Throw, OnGrab, PullBack, OnMove, Disable}
