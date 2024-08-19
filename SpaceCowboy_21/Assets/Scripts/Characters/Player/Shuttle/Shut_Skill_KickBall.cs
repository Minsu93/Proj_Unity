using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 발차기를 한 반대방향으로 날아다닌다. 화면의 가장자리에 맞으면 튕긴다. 인터렉션 하는 적들에게 피해를 준다. 
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

    //총알이 화면 밖으로 나갔을 때 
    float screenBorderLimit = 0.1f;

    bool OutSideScreenBorder(ref Vector2 velocity)
    {
        //화면 밖으로 나갔는지 체크
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

        //화면 내부 적들 체크
        Collider2D[] colls = new Collider2D[maxEnemyCount];
        Vector2 size = Camera.main.pixelRect.size;

        //ContactFIlter 설정
        ContactFilter2D filter2D = new ContactFilter2D();
        filter2D.useTriggers = true;
        filter2D.SetLayerMask(targetLayer);

        int count = Physics2D.OverlapBox(transform.position, size, 0, filter2D, colls);
        if (count > 0)
        {
            float minDist = float.MaxValue;

            for (int i = 0; i < count; i++)
            {
                //타겟 확인
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
        // 화면의 왼쪽 아래 코너를 월드 좌표로 변환
        Vector3 bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));

        // 화면의 오른쪽 위 코너를 월드 좌표로 변환
        Vector3 topRight = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));

        // 유닛 크기 계산
        float screenWidthInUnits = topRight.x - bottomLeft.x;
        float screenHeightInUnits = topRight.y - bottomLeft.y;

        return Vector2 size = new Vector2(screenWidthInUnits, screenHeightInUnits);
    }
}
