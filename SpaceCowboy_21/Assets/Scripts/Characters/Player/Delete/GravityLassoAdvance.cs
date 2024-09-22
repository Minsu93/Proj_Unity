using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLassoAdvance : MonoBehaviour
{
    //[SerializeField] public LassoState lassoState = LassoState.Disable;
    //[SerializeField] float throwTime;   //���ư��� �ð�
    //[SerializeField] float maxThrowDistance; //������ max �Ÿ�
    //[SerializeField] float pullTime;    //���� �ð�
    //float moveTime;    //��� ������ �����̴� �ð�
    //[SerializeField] float grabForce;   //���� ��
    //[SerializeField] float lassoLength = 3f;     //�ð��� ����

    //[SerializeField] float moveSpeed;   // ��� ������ ������ ���� �ӵ�
    //[SerializeField] float maxLassoDistance; //�ð��� �������� ������ �ִ� ���� 
    //[SerializeField] AnimationCurve throwCurve; //���� �� Ŀ��
    //[SerializeField] AnimationCurve pullCurve;  //��� �� Ŀ��

    //[SerializeField] GameObject lassoTip;
    //public Vector2 forceByLasso { get; set; }   //�ð��̷� �޴� ��

    //float timer;
    //Vector2 throwStartPos;
    //Transform grabTarget;
    //Weight grabWeight;
    //Vector3 grabLocalPos;
    //float grabObjSize;
    //bool grabPlanet;
    //bool canLaunch;
    //float lassoDist;    //�ð��� ���� 
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
    //            //����� ������. 
    //            LaunchGrabObj();

    //            //if (canLaunch)
    //            //{
    //            //    //����� ������. 
    //            //    LaunchGrabObj();
    //            //}

    //            ////�ð��̴� ���ƿ´�.
    //            //grabTarget = null;
    //            ////�ð��� ���� ���� ��ġ ����
    //            //preLassoTipPos = lassoTip.transform.position;
    //            ////Ÿ�̸� �ʱ�ȭ
    //            //timer = 0;
    //            ////Grab�� �ʱ�ȭ
    //            //forceByLasso = Vector2.zero;
    //            ////�ð��� ���¸� ���� ������ �����Ѵ�. 
    //            //lassoState = LassoState.PullBack;
    //        }
    //    }

    //    if(Input.GetKeyDown(KeyCode.E) && lassoState == LassoState.OnGrab)
    //    {
    //        ////�ð��̴� ���ƿ´�.
    //        //grabTarget = null;
    //        ////�ð��� ���� ���� ��ġ ����
    //        //preLassoTipPos = lassoTip.transform.position;
    //        ////Ÿ�̸� �ʱ�ȭ
    //        //timer = 0;
    //        ////Grab�� �ʱ�ȭ
    //        //forceByLasso = Vector2.zero;
    //        ////�ð��� ���¸� ���� ������ �����Ѵ�. 
    //        //lassoState = LassoState.PullBack;
    //        ReleaseLasso();
    //    }


    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        if(lassoState == LassoState.Disable)
    //        {
    //            //�ð��� ��ġ�� �ʱ�ȭ�Ѵ�.
    //            InitializeLasso();
    //            //Ÿ�̸Ӹ� �ʱ�ȭ�Ѵ�
    //            timer = 0;
    //            //�ð��� ���¸� ������ ������ �����Ѵ�.
    //            lassoState = LassoState.Throw;

    //        }
    //        //switch (lassoState)
    //        //{
    //        //    case LassoState.Disable:
    //        //      //�ð��� ��ġ�� �ʱ�ȭ�Ѵ�.
    //                //InitializeLasso();
    //        //      //Ÿ�̸Ӹ� �ʱ�ȭ�Ѵ�
    //        //      timer = 0;
    //        //      //�ð��� ���¸� ������ ������ �����Ѵ�.
    //        //      lassoState = LassoState.Throw;
    //        //}
    //    }

    //    if( Input.GetMouseButtonUp(1))
    //    {
    //        switch(lassoState)
    //        {
    //            case LassoState.Throw:
    //                //�ð��̸� ������ ���̶��, �������� ����ϰ� �ٽ� ���ƿ��� �����.
    //                ////�ð��� ���� ���� ��ġ ����
    //                //preLassoTipPos = lassoTip.transform.position;
    //                ////Ÿ�̸� �ʱ�ȭ
    //                //timer = 0;
    //                ////�ð��� ���¸� ���� ������ �����Ѵ�. 
    //                //lassoState = LassoState.PullBack;
    //                ReleaseLasso();
    //                break;

    //            case LassoState.OnGrab:
    //                //Ÿ�̸� �ʱ�ȭ
    //                timer = 0;
    //                //forceByLasso = Vector2.zero;

    //                //�ʿ��� ������
    //                moveStartVelocity = rb.velocity;

    //                float t = (lassoTip.transform.position - transform.position).magnitude / moveSpeed;
    //                moveTime = Mathf.Clamp(t, 0.7f, 3f);

    //                moveStartPosition = transform.position;
    //                //�ð��� ���¸� ��� �������� ���������� �����Ѵ�. 
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
    //            //lassoTip�� ���� ��ġ�� ������Ʈ�Ѵ�. 

    //            lassoTip.transform.position = grabTarget.TransformPoint(grabLocalPos);

    //            //if (grabPlanet)
    //            //{
    //            //    lassoTip.transform.position = grabTarget.TransformPoint(grabLocalPos);
    //            //}
    //            //else
    //            //{
    //            //    lassoTip.transform.position = grabTarget.transform.position;
    //            //}
    //            //���� �Ÿ� �̻� �־����� ������. 
    //            CheckLassoDistance();


    //            break;
    //        case LassoState.OnMove:
    //            //lassoTip�� ��ġ�� ������Ʈ
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
    //    //���� �̵� 2���� FixedUpdate����.
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


    ////�ð��� �ʱ�ȭ
    //private void InitializeLasso()
    //{
    //    lassoTip.SetActive (true);
    //    lineRenderer.enabled = true;
    //    lassoTip.transform.position = this.transform.position;

    //    Vector3 inputPos = Input.mousePosition;
    //    inputPos.z = 10;    //z�� ī�޶󿡼������� �Ÿ�
    //    Vector3 mousePos = Camera.main.ScreenToWorldPoint(inputPos);
    //    mousePos.z = 0;
    //    targetDir = (mousePos - transform.position).normalized;

    //    preLassoTipPos = transform.position;
    //    throwStartPos = transform.position;

    //}

    ////�ð��� ��Ȱ��ȭ
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

    ////dir �������� �ð��̸� ������
    //private void ThrowLasso(Vector2 dir)
    //{
    //    //�ð��� �����δ�. 
    //    timer += Time.deltaTime;
    //    float percent = timer / throwTime;

    //    Vector2 pos = Vector2.Lerp((Vector2)transform.position, throwStartPos + (targetDir * maxThrowDistance), throwCurve.Evaluate(percent)); ;
    //    lassoTip.transform.position = pos;

    //    //�ƹ��͵� �浹���� �ʰ� �ִ� �Ÿ��� �����ϸ�
    //    if (timer > throwTime)
    //    {
    //        //timer = 0;
    //        ////�ð��� ���� ���� ��ġ ����
    //        //preLassoTipPos = lassoTip.transform.position;

    //        //lassoState = LassoState.PullBack;
    //        ReleaseLasso();
    //    }
    //}

    //private void ReleaseLasso()
    //{
    //    //�ð��̴� ���ƿ´�.
    //    grabTarget = null;

    //    if (grabWeight)
    //    {
    //        grabWeight.ReleaseOBJ();
    //        grabWeight = null;
    //    }

    //    //�ð��� ���� ���� ��ġ ����
    //    preLassoTipPos = lassoTip.transform.position;
    //    //Ÿ�̸� �ʱ�ȭ
    //    timer = 0;
    //    //Grab�� �ʱ�ȭ
    //    //forceByLasso = Vector2.zero;
    //    //�ð��� ���¸� ���� ������ �����Ѵ�. 
    //    lassoState = LassoState.PullBack;
    //}

    ////�ð��̸� ����, ����Ѵ�. 
    //private void PullLasso(Vector2 lassoTipPos)
    //{
    //    //�ð��̴� �Ųٷ� ���ƿ´�.
    //    timer += Time.deltaTime;

    //    //Vector2 dir = (lassoTip - (Vector2)transform.position);
    //    float percent = timer / pullTime;
    //    Vector2 pos = Vector2.Lerp(lassoTipPos, (Vector2)transform.position, pullCurve.Evaluate(percent));
    //    lassoTip.transform.position = pos;

    //    //�÷��̾� ��ó�� �����ϸ�
    //    if (timer > pullTime)
    //    {
    //        timer = 0;
    //        DisableLasso();
    //        lassoState = LassoState.Disable;
    //    }
    //}


    ////�ð��̰� ���� ���·� ������ �� 
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
    //        //    //�༺�� �����϶� 
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

    //    ////��� ���� ���⿡ ��ġ�ϵ��� ȸ����Ų��.
    //    //Vector2 a = lassoTip.transform.position - grabTarget.transform.position;
    //    //float angle = Vector2.SignedAngle(vec, a);
    //    //var impulse = (-1f * 0.1f *  angle * Mathf.Deg2Rad) * grabTargetRb.inertia;
    //    //grabTargetRb.AddTorque(impulse, ForceMode2D.Force);

    //}

    //void CheckLassoDistance()
    //{
    //    //�ð��� ���̰� �ʹ� ��� ������. 
    //    if(lassoDist > maxLassoDistance)
    //    {
    //        ReleaseLasso();
    //    }
    //}



    ////�ð��̰� ���� ������ ���ư���
    //private void MoveToGrabPoint(Vector2 startPos, Vector2 startVel)
    //{
    //    //�ð��̸� ������, ���� �ӵ��� �����ϸ鼭 ���ӵ��� ������ �޾Ƽ� ���������� ���ϴ� �ð��� ���ϴ� ��ġ�� �����ϴ°�. 
    //    timer += Time.fixedDeltaTime;
    //    float percent = timer / moveTime;



    //    Vector2 maybePos = startPos + (startVel * percent);
    //    Vector2 pos = Vector2.Lerp(maybePos, lassoTip.transform.position, pullCurve.Evaluate(percent));
    //    rb.MovePosition(pos);



    //    //�������� �������� �� 
    //    if (timer > moveTime)
    //    {
    //        timer = 0;
    //        DisableLasso();
    //        lassoState = LassoState.Disable;

    //    }
    //}


    ////�浹 üũ
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
    //            //����comp�� ���� �� 
    //            grabWeight = weight;
    //            grabWeight.GrabOBJ();

    //            //������ �浹�ߴ�! 
    //            timer = 0;
    //            //�浹�� ����� transform�� �����´�
    //            grabTarget = hit.transform;
    //            //���� ��� ���Ѵ�. 
    //            grabLocalPos = hit.transform.InverseTransformPoint(hit.point); ;

    //            //���¸�onGrab���� �����Ѵ�
    //            lassoState = LassoState.OnGrab;
    //        }

    //        //Vector3 size = hit.collider.bounds.size;
    //        //grabObjSize = size.x * size.y;
            
    //        //if(grabObjSize < 25f)
    //        //{
    //        //    //��ü �߻� ����
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

           
    //        //�浹���� �ð��� ���̸� ���Ѵ�. 
    //        //grabDist = (transform.position - hit.transform.position).magnitude;
    //        //�浹�� ����� Rigidbody2D�� �����´�. 
    //        //grabTargetRb = grabTarget.GetComponent<Rigidbody2D>();



    //    }
    //    else
    //    {
    //        preLassoTipPos = lassoTip.transform.position;
    //    }
    //}

    ////���� ������ ���콺 �������� ������. 
    //private void LaunchGrabObj()
    //{
    //    Vector3 inputPos = Input.mousePosition;
    //    inputPos.z = 10;    //z�� ī�޶󿡼������� �Ÿ�
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
    //    //    //�༺�� ���� �� 
    //    //    //if(grabTarget.TryGetComponent(out Planet planet))
    //    //    //{
    //    //    //    planet.MovePlanet(launchVec * launchForce);
    //    //    //}
    //    //}
    //    //else
    //    //{
    //    //    //�༺ �̿��� ���� ���� �� 
    //    //    //grabTargetRb.AddForce(launchVec * launchForce, ForceMode2D.Impulse);
    //    //}

    //    //canLaunch = false;
    //}

    ////linerenderer ������Ʈ
    //private void UpdateLineRenderer()
    //{

    //    //�ð��� �Ӹ��� ȸ����Ų��
    //    Vector2 tipVec = lassoTip.transform.position - transform.position;
    //    lassoDist = tipVec.magnitude;

    //    Vector2 tipDir = tipVec.normalized;

    //    Vector3 upVec = Quaternion.Euler(0, 0, 90) * tipDir;
    //    Quaternion rot = Quaternion.LookRotation(forward: Vector3.forward, upwards: upVec);
    //    lassoTip.transform.rotation = rot;

    //    Vector2 p;
    //    ////����� ���� �� 
    //    //if (grabTarget && !grabPlanet)
    //    //{
    //    //    p = grabTarget.transform.position;
    //    //    lineRenderer.SetPosition(0, transform.position);
    //    //    lineRenderer.SetPosition(1, p);
    //    //}
    //    //else
    //    //{
    //    //    //���� ������ ����
    //    //    p = (Vector2)lassoTip.transform.position + (tipDir * -1f * 0.5f);
    //    //    lineRenderer.SetPosition(0, transform.position);
    //    //    lineRenderer.SetPosition(1, p);
    //    //}
        

    //    //���� ������ ����
    //    p = (Vector2)lassoTip.transform.position + (tipDir * -1f * 0.5f);
    //    lineRenderer.SetPosition(0, transform.position);
    //    lineRenderer.SetPosition(1, p);

    //}
}

//public enum LassoState { Throw, OnGrab, PullBack, OnMove, Disable}
