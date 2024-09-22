using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabLasso : MonoBehaviour
{
    //private enum LassoState { Disable, Throw, Return, Dragged}
    //private LassoState lassoState = LassoState.Disable;
    //public float lassoTipSize = 0.3f;   //�ð��� �� ũ��(Grabbable ������ ���)
    ////public float chargeSpeed = 5f; //���� �ӵ� 

    //[SerializeField] float throwTime;   //������ ���ư��� �ð�
    //[SerializeField] AnimationCurve throwCurve; //���� �� Ŀ��
    //public float throwDistance = 5f;    //������ �ִ� �Ÿ� 
    //float dist;     //���� ������ �Ÿ� 
    //Vector2 throwTargetPoint;   //���� ������ ��ǥ���� 

    //[SerializeField] float pullTime;    //��⵿�� ���ƿ��� �ð�
    //[SerializeField] AnimationCurve pullCurve;  //��� �� Ŀ��
    //[SerializeField] GameObject lassoTip;   //���κ�
    //public float lassoCoolTime;  //�ð��� �������ð� 

    //[SerializeField] float moveTime;
    //[SerializeField] AnimationCurve dragCurve; //���� �� Ŀ��
    //Vector2 dragStartPosition;  //Drag ���� ��ġ
    //Vector2 dragStartVelocity;  //Drag ���� �ӵ�
    

    //public bool lassoON { get; private set; }   //�ð��̰� Ȱ��ȭ�Ǿ����� Ȯ�ο� �Լ� 
    //float timer;
    ////float chargeGauge;
    //Transform grabTarget;
    //Vector3 grabLocalPos;
    
    //LineRenderer lineRenderer;

    //int targetLayer;
    //PlayerBehavior playerBehavior;
    //Rigidbody2D rb;

    //bool chargeON;  //��� �������ΰ���?

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
    //        //�ð��̰� ��Ȱ��ȭ ������ �� 
    //        case LassoState.Disable:

    //            ////�����ұ��?
    //            //if (chargeON)
    //            //{
    //            //    ChargeLasso();
    //            //}
    //            ////���콺 �Է� ���� 
    //            //if (Input.GetMouseButtonDown(1))
    //            //{
    //            //    //���� ���� 
    //            //    chargeON = true;
    //            //}

    //            return;

    //        //�ð��̰� ������ ���� �� 
    //        case LassoState.Throw:
    //            //���ߴ� ��� ����
    //            ThrowLasso();
    //            break;


    //        //�ð��̰� ���ƿ��� ���� �� 
    //        case LassoState.Return:
    //            PullObject();
    //            break;

    //        //�ð��̰� �༺�� ������ �� 
    //        case LassoState.Dragged:
    //            DraggedToPlanet();
    //            break;
    //    }

    //    UpdateLineRenderer();
    //}

    //public void TryThrowLasso()
    //{
    //    if(lassoState != LassoState.Disable) return;

    //    //// �ð��� ������ 
    //    //Debug.Log("Throw Power is : " + chargeGauge);

    //    //�Ÿ� ��� 
    //    //dist = throwDistance * (chargeGauge / 100f);
    //    throwTargetPoint = (Vector2)transform.position + (throwDistance * playerBehavior.aimDirection);


    //    //// ���� �ʱ�ȭ
    //    //chargeON = false;
    //    //chargeGauge = 0;

    //    // �ð��� Ȱ��ȭ
    //    InitializeLasso();

    //    //���� ��ȯ
    //    lassoState = LassoState.Throw;

    //}

    ////�ð��� Ȱ��ȭ 
    //private void InitializeLasso()
    //{
    //    lassoTip.SetActive(true);
    //    lineRenderer.enabled = true;
    //    lassoON = true;
    //}


    ////�ð��� ��Ȱ��ȭ
    //private void DisableLasso()
    //{
    //    lassoTip.SetActive(false);
    //    lineRenderer.enabled = false;
    //    lassoON = false;
    //}


    ////�ð��� �����ϱ� 
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


    ////�ð��� �߻��ϱ�
    //void ThrowLasso()
    //{
    //    //�ð��� �����δ�. 
    //    timer += Time.deltaTime;
    //    float percent = timer / throwTime;

    //    Vector2 pos = Vector2.Lerp((Vector2)transform.position, throwTargetPoint, throwCurve.Evaluate(percent));
    //    lassoTip.transform.position = pos;


    //    //�浹�ϴ� ��ü�� üũ�Ѵ�
    //    if (CheckGrabbables())
    //    {
    //        //������ ������ ���
    //        return;
    //    }

        
    //    //�ƹ��͵� �浹���� �ʰ� �ִ� �Ÿ��� �����ϸ�
    //    if (timer > throwTime)
    //    {
    //        timer = 0f;

    //        throwTargetPoint = pos;

    //        lassoState = LassoState.Return;
    //    }
    //}



    ////�ð��̷� ������ �� �ִ� ����� üũ�Ѵ� 
    //bool CheckGrabbables()
    //{
    //    bool grabObj = false;

    //    //���̸� ���콺 ��ġ�� �߻��ؼ� ��� �´��� üũ 
    //    RaycastHit2D hit = Physics2D.CircleCast(lassoTip.transform.position, lassoTipSize, Vector2.right, 0f, targetLayer);
    //    if (hit.collider != null)
    //    {
    //        grabObj = true;

    //        if (hit.collider.CompareTag("Object"))
    //        {
    //            //����� Object�� ��� 

    //            //�ʿ��� �������� ä���ִ´�
    //            grabTarget = hit.transform;
    //            grabLocalPos = hit.transform.InverseTransformPoint(hit.point);

    //            //���� ��ġ�� ����س��´�
    //            throwTargetPoint = hit.transform.position;

    //            //�ð��� ������ �ð� �ʱ�ȭ
    //            timer = 0f;

    //            //�ð����� ���� ����
    //            lassoState = LassoState.Return;

    //        }
    //        else
    //        {
    //            //����� �༺�� ���

    //            //�ʿ��� �������� ä���ִ´�
    //            //grabTarget = hit.transform;
    //            //grabLocalPos = hit.transform.InverseTransformPoint(hit.point);
    //            dragStartPosition = transform.position;
    //            dragStartVelocity = rb.velocity * 0.5f;
    //            throwTargetPoint = hit.point;


    //            //���� ��ġ�� ����س��´�
    //            //throwTargetPoint = hit.transform.position;

    //            //�ð��� ������ �ð� �ʱ�ȭ
    //            timer = 0f;

    //            //�ð����� ���� ����
    //            //lassoState = LassoState.Dragged;

    //            //�ð��� �������� ����ϱ�
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



    ////�ð��̸� ����, ����Ѵ�. 
    //private void PullObject()
    //{
    //    //�ð��̴� �Ųٷ� ���ƿ´�.
    //    timer += Time.deltaTime;

    //    float percent = timer / pullTime;
    //    Vector2 pos = Vector2.Lerp(throwTargetPoint, (Vector2)transform.position, pullCurve.Evaluate(percent));

    //    if (grabTarget)
    //    {
    //        grabTarget.position = pos - (Vector2)grabLocalPos;
    //    }

    //    lassoTip.transform.position = pos;


    //    //�÷��̾� ��ó�� �����ϸ�
    //    if (timer > pullTime)
    //    {
    //        timer = 0;

    //        //Object�� ����մϴ�. 

    //        grabTarget = null;
            
    //        DisableLasso();

    //        lassoState = LassoState.Disable;
    //    }
    //}

    ////�ð��̸� Ÿ�� �༺���� ���ư���. 
    //void DraggedToPlanet()
    //{
    //    //�ð��̸� ������, ���� �ӵ��� �����ϸ鼭 ���ӵ��� ������ �޾Ƽ� ���������� ���ϴ� �ð��� ���ϴ� ��ġ�� �����ϴ°�. 
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



    //    //�������� �������� �� 
    //    if (timer > moveTime)
    //    {
    //        timer = 0;

    //        grabTarget = null;

    //        DisableLasso();

    //        lassoState = LassoState.Disable;

    //    }

    //    ////�༺�� ���� ������ ����
    //    //if (dist < 1f)
    //    //{
    //    //    timer = 0;

    //    //    //grabTarget = null;

    //    //    DisableLasso();

    //    //    lassoState = LassoState.Disable;

    //    //}
    //}



    ////linerenderer ������Ʈ
    //private void UpdateLineRenderer()
    //{
    //    //���� ������ ����
    //    lineRenderer.SetPosition(0, transform.position);
    //    lineRenderer.SetPosition(1, lassoTip.transform.position);
    //}
}


