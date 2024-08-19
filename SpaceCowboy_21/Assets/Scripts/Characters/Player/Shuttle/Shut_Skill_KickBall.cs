using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �����⸦ �� �ݴ�������� ���ƴٴѴ�. ȭ���� �����ڸ��� ������ ƨ���. ���ͷ��� �ϴ� ���鿡�� ���ظ� �ش�. 
/// </summary>
public class Shut_Skill_KickBall : ShuttleSkill
{
    [Header("KickBall Property")]
    [SerializeField] float moveSpeed = 5.0f;
    private void FixedUpdate()
    {
        if (!useStart) return;

        Vector2 vel = rb.velocity;
        if (OutSideScreenBorder(ref vel))
        {
            Transform targetTr = FindNearEnemy();
            if (targetTr == null)
            {
                rb.velocity = vel;
            }
            else
            {
                Vector2 dir = ((Vector2)targetTr.position - rb.position).normalized;
                rb.velocity = dir * moveSpeed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("enemyHit");
        }
    }


    public override void DamageEvent(float damage, Vector2 hitPoint)
    {
        Debug.Log("ProjectileHit");
    }

    public override void Kicked(Vector2 hitPos)
    {
        Vector2 dir = ((Vector2)transform.position - hitPos).normalized;
        rb.velocity = dir * moveSpeed;

        useStart = true;
    }

    public override void CompleteEvent()
    {
        rb.velocity = Vector2.zero;
    }

    //�Ѿ��� ȭ�� ������ ������ �� 
    float screenBorderLimit = 0.1f;

    bool OutSideScreenBorder(ref Vector2 velocity)
    {
        //ȭ�� ������ �������� üũ
        bool outside = false;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        if (screenPosition.x < 0 - screenBorderLimit)
        {
            outside = true;
            velocity.x = (Mathf.Abs(velocity.x));
        }
        else if (screenPosition.x > Camera.main.pixelWidth + screenBorderLimit)
        {
            outside = true;
            velocity.x = -1f * (Mathf.Abs(velocity.x));
        }

        if (screenPosition.y < 0 - screenBorderLimit)
        {
            outside = true;
            velocity.y = (Mathf.Abs(velocity.y));
        }
        else if (screenPosition.y > Camera.main.pixelHeight + screenBorderLimit)
        {
            outside = true;
            velocity.y = -1f * (Mathf.Abs(velocity.y));
        }

        return outside;
    }

    Transform FindNearEnemy()
    {
        Transform tr = null;

        int targetLayer = 1 << LayerMask.NameToLayer("Enemy");
        int maxEnemyCount = 10;

        //ȭ�� ���� ���� üũ
        Collider2D[] colls = new Collider2D[maxEnemyCount];
        Vector2 size = Camera.main.pixelRect.size;

        //ContactFIlter ����
        ContactFilter2D filter2D = new ContactFilter2D();
        filter2D.useTriggers = true;
        filter2D.SetLayerMask(targetLayer);

        int count = Physics2D.OverlapBox(transform.position, size, 0, filter2D, colls);
        if (count > 0)
        {
            float minDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                //Ÿ�� Ȯ��
                float dist = Vector2.Distance(transform.position, colls[i].transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    tr = colls[i].transform;
                }
            }
        }
        return tr;
    }

    Vector2 GetScreenSize()
    {
        // ȭ���� ���� �Ʒ� �ڳʸ� ���� ��ǥ�� ��ȯ
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));

        // ȭ���� ������ �� �ڳʸ� ���� ��ǥ�� ��ȯ
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        // ���� ũ�� ���
        float screenWidthInUnits = topRight.x - bottomLeft.x;
        float screenHeightInUnits = topRight.y - bottomLeft.y;

        return Vector2 size = new Vector2(screenWidthInUnits, screenHeightInUnits);
    }
}
