using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DroneItem : MonoBehaviour
{
    public Sprite sprite;
    public Sprite icon;
    public bool autoUse = false;    //�ڵ� ���

    public Vector2 dronePos { get; set; }
    [SerializeField] float distFromPlayer = 2f;
    [SerializeField] float minSmoothTime = 0.1f; // �ּ� ���� �ð�
    [SerializeField] float maxSmoothTime = 0.5f; // �ִ� ���� �ð�
    [SerializeField] float useDuration = 5.0f;  //���� �ð�

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



    // ��Ʋ ó�� ���������� ���� �� 
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
        //���ӽð��� ������ ��ų ����� ����
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
    /// ����� ����Ѵ� 
    /// </summary>
    public virtual void UseDrone(Vector2 mousePos, Quaternion quat)
    {
        //��ų�� ������ ��ų�� �ߵ��Ѵ�. 
        return;
    }


    protected void RigidBodyFollowPlayer()
    {
        Transform targetTr = GameManager.Instance.player;
        if (targetTr == null) return;

        // ��ǥ ��ġ: �÷��̾��� ��ġ
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

        // ���� ��ġ�� ��ǥ ��ġ ���� �Ÿ�
        distance = Vector2.Distance(transform.position, targetPosition);

        // �Ÿ� ������� smoothTime ����
        float smoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, distance / distFromPlayer);

        // �ε巴�� �̵� (SmoothDamp)
        rb.MovePosition(Vector2.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));

        //�̵� �ӵ��� ���� view Flip
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
    /// ��� ����� �����Ѵ�.
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
        //���� �ִ� �ݴ������� ���ư���
        Vector2 launchDir = isRight? new Vector2(-1,0) : new Vector2(1,0);
        float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
        Vector2 randomPoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        launchDir += randomPoint;
        Vector2 force = launchDir.normalized * launchForce;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(3.0f);
        //������

        this.gameObject.SetActive(false);
    }
}
