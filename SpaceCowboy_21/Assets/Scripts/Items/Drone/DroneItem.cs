using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DroneItem : MonoBehaviour
{
    public Sprite sprite;
    public Sprite icon;
    public bool autoUse = false;    //자동 사용

    public Vector2 dronePos { get; set; }
    [SerializeField] float distFromPlayer = 2f;
    [SerializeField] float minSmoothTime = 0.1f; // 최소 감속 시간
    [SerializeField] float maxSmoothTime = 0.5f; // 최대 감속 시간
    [SerializeField] float useDuration = 5.0f;  //지속 시간

    [SerializeField] Transform viewTr;

    protected Rigidbody2D rb;
    protected Collider2D physicsColl;
    protected CharacterGravity gravity;

    protected bool activate;
    bool isRight = true;
    protected bool stopFollow;
    protected bool useDrone;
    float timer;
    Vector2 velocity = Vector2.zero;
    Vector3 viewScale;

    [SerializeField] float launchForce = 15.0f;



    // 셔틀 처음 스테이지에 생성 시 
    public void InitializeDrone()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();
        if(gravity == null)
            gravity = GetComponent<CharacterGravity>();
        if(physicsColl == null)
            physicsColl = GetComponentInChildren<Collider2D>();

        physicsColl.enabled = true;
        gravity.activate = false;
        viewScale = viewTr.localScale;


        stopFollow = false;
        useDrone = false;
        activate = true;

        timer = 0;
    }

    protected virtual void Update()
    {
        //지속시간이 끝나면 스킬 사용을 제거
        if (useDrone)
        {
            if(timer < useDuration)
            {
                timer += Time.deltaTime;
            }
            else
            {
                EndUseDrone();
            }
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!activate) return;
        if (stopFollow) return;
        if (useDrone) return;
            
        RigidBodyFollowPlayer();
    }

    /// <summary>
    /// 드론을 사용한다 
    /// </summary>
    public virtual void UseDrone(Vector2 mousePos, Quaternion quat)
    {
        //스킬이 있으면 스킬을 발동한다. 
        return;
    }


    protected void RigidBodyFollowPlayer()
    {
        Transform targetTr = GameManager.Instance.player;
        if (targetTr == null) return;

        // 목표 위치: 플레이어의 위치
        bool right = GameManager.Instance.playerManager.playerBehavior.aimRight;
        int minus = right ? 1 : -1;
        Vector2 targetPosition = targetTr.position + (targetTr.up * dronePos.y) + (targetTr.right * dronePos.x * minus);

        RigidBodyMoveToPosition(targetPosition, out float distanceToPlayer);

        if (distanceToPlayer < 1f)
        {
            physicsColl.enabled = true;
        }
        else if (distanceToPlayer > 5.0f)
        {
            physicsColl.enabled = false;
        }
    }

    protected void RigidBodyMoveToPosition(Vector2 targetPos, out float distance)
    {
        Vector2 targetPosition = targetPos;

        // 현재 위치와 목표 위치 간의 거리
        distance = Vector2.Distance(transform.position, targetPosition);

        // 거리 기반으로 smoothTime 조절
        float smoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, distance / distFromPlayer);

        // 부드럽게 이동 (SmoothDamp)
        rb.MovePosition(Vector2.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));

        //이동 속도에 따라 view Flip
        if (velocity.x > 0 && !isRight)
        {
            isRight = true;
            viewTr.localScale = new Vector3(viewScale.x, viewScale.y, viewScale.z);
        }
        else if (velocity.x < 0 && isRight)
        {
            isRight = false;
            viewTr.localScale = new Vector3(-viewScale.x, viewScale.y, viewScale.z);
        }
    }

    /// <summary>
    /// 드론 사용을 중지한다.
    /// </summary>
    public virtual void EndUseDrone()
    {
        stopFollow = true;
        useDrone = false;
        activate = false;
        gravity.activate = true;
        StartCoroutine(DeActivateDrone());
    }

    IEnumerator DeActivateDrone()
    {
        yield return null;
        //보고 있는 반대편으로 날아간다
        Vector2 launchDir = isRight? new Vector2(-1,0) : new Vector2(1,0);
        float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
        Vector2 randomPoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        launchDir += randomPoint;
        Vector2 force = launchDir.normalized * launchForce;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(3.0f);
        //터진다

        this.gameObject.SetActive(false);
    }
}
