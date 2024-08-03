using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExplodeAsteroid : MonoBehaviour, IHitable, ITarget, IKickable, IStageObject
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float touchDmg,knockbackAmount, damage, speed, lifeTime, range;
    [Space]
    [SerializeField] GameObject viewObj;
    [SerializeField] CircleCollider2D circleColl;
    private Health health;
    private WaitForSeconds collEnableDelaySec = new WaitForSeconds(1f);
    private Rigidbody2D rb;

    private void Awake()
    {
        health = GetComponent<Health>();
        health.ResetHealth();

        rb = GetComponent<Rigidbody2D>();
    }


    public void InitializeObject()
    {
        health.ResetHealth();

        viewObj.SetActive(true);

        circleColl.enabled = true;

        float randomScaleFactor = UnityEngine.Random.Range(1f, 2f);
        transform.localScale = Vector3.one * randomScaleFactor;

    }

    //목표 지점으로 천천히 이동하기
    public void MoveToTargetPoint(Vector2 target)
    {
        Vector2 vec = target - (Vector2)transform.position;
        Vector2 dir = vec.normalized;
        float speed = 2f;
        rb.AddForce(dir * speed ,ForceMode2D.Impulse);

        float randomTorque = UnityEngine.Random.Range(-30, 30);
        rb.AddTorque(randomTorque);
    }


    //플레이어 총알에 맞는다
    void IHitable.DamageEvent(float damage, Vector2 hitPoint)
    {
        if (health.AnyDamage(1.0f))
        {
            if (health.IsDead())
            {
                Explode();
                circleColl.enabled = false;
                viewObj.SetActive(false);
                StartCoroutine(DeactivateRoutine());
            }
        }

    }


    void IHitable.KnockBackEvent(Vector2 hitPos, float forceAmount)
    {
        Debug.Log("Knocked!");
        return;
    }

    public void Kicked()
    {
        ((IHitable)this).DamageEvent(1f, Vector2.zero);
    }

    //폭발 특수 효과
    void Explode()
    {
        //8방향으로 적군 projectile 공격.
        ShootAction(transform.position, Quaternion.Euler(0, 0, 0));
        ShootAction(transform.position, Quaternion.Euler(0, 0, 45));
        ShootAction(transform.position, Quaternion.Euler(0, 0, 90));
        ShootAction(transform.position, Quaternion.Euler(0, 0, 135));
        ShootAction(transform.position, Quaternion.Euler(0, 0, 180));
        ShootAction(transform.position, Quaternion.Euler(0, 0, 225));
        ShootAction(transform.position, Quaternion.Euler(0, 0, 270));
        ShootAction(transform.position, Quaternion.Euler(0, 0, 315));


    }

    public void ShootAction(Vector2 pos, Quaternion Rot)
    {
        //총알 생성
        GameObject proj = GameManager.Instance.poolManager.GetPoolObj(projectilePrefab, 1);
        proj.transform.position = pos;
        proj.transform.rotation = Rot;
        proj.GetComponent<Projectile>().Init(damage, speed, lifeTime, range);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(collision.TryGetComponent<IEnemyHitable>(out IEnemyHitable hitable))
            {
                hitable.DamageEvent(touchDmg, transform.position);
                //hitable.KnockBackEvent(transform.position, knockbackAmount);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("SpaceBorder"))
        {
            rb.velocity = Vector2.zero;
            viewObj.SetActive(false);
            StartCoroutine(DeactivateRoutine());
        }
    }

    ////적군 총알에 맞는다
    //void IEnemyHitable.DamageEvent(float damage, Vector2 hitPoint)
    //{
    //    Debug.Log("Damaged!");
    //}

    //void IEnemyHitable.KnockBackEvent(Vector2 hitPos, float forceAmount)
    //{
    //    Debug.Log("Knocked!");
    //}


    Collider2D ITarget.GetCollider()
    {
        return circleColl;
    }


    IEnumerator DeactivateRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);
    }
}
