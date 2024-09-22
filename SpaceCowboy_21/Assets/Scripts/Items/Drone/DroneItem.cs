using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DroneItem : MonoBehaviour
{

    public Sprite icon;
    public bool autoUse = false;    //�ڵ� ���

    [SerializeField] Vector2 dronePos = new Vector2(-1, 1);
    [SerializeField] float distFromPlayer = 2f;
    [SerializeField] float minSmoothTime = 0.1f; // �ּ� ���� �ð�
    [SerializeField] float maxSmoothTime = 0.5f; // �ִ� ���� �ð�
    [SerializeField] float useDuration = 5.0f;  //���� �ð�

    [SerializeField] Transform viewTr;

    protected Rigidbody2D rb;
    protected Collider2D physicsColl;

    bool isRight = true;
    protected bool stopFollow;
    protected bool useDrone;
    float timer;
    Vector2 velocity = Vector2.zero;
    Vector3 viewScale;





    // ��Ʋ ó�� ���������� ���� �� 
    public void InitializeDrone()
    {
        rb = GetComponent<Rigidbody2D>();

        physicsColl = GetComponentInChildren<Collider2D>();
        physicsColl.enabled = true;

        viewScale = viewTr.localScale;


        stopFollow = false;
        useDrone = false;

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
        if (stopFollow) return;
        if (useDrone) return;
            
        RigidBodyFollowPlayer();
    }

    /// <summary>
    /// ����� ����Ѵ� 
    /// </summary>
    public abstract void UseDrone(Vector2 mousePos, Quaternion quat);

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
    protected virtual void EndUseDrone()
    {
        DeActivateDrone();
        Debug.Log("End Use Drone");
    }

    void DeActivateDrone()
    {
        stopFollow = true;
        useDrone = false;
        this.gameObject.SetActive(false);
    }
}
