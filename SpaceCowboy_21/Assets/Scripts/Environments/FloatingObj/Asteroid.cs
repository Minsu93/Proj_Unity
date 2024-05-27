using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    /// <summary>
    /// 중력 총에 맞으면 지상으로 떨어진다. 
    /// </summary>
    /// 
    public float collideDamage = 20f;
    public float collideSpeed = 1.0f;
    public float explodeDamage = 10f;
    public float explodeRange = 3f;

    public ParticleSystem hitParticle;
    public ParticleSystem explodeParticle;
    public GameObject viewObj;

    Gravity gravity;
    Rigidbody2D rb;
    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    Vector2 explodeNormal;

    bool _activate = false;
    public bool Activate { get { return _activate; } set { _activate = value; } }

    private void Awake()
    {
        gravity = GetComponent<Gravity>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void GravityOnEvent()
    {
        gravity.FixedGravityFunction(GameManager.Instance.playerManager.playerNearestPlanet, collideSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_activate) return;

        // 적과 충돌했는지 확인
        if (collision.CompareTag("Enemy"))
        {
            // 이미 맞춘 적이 아니라면 처리
            if (!hitTargets.Contains(collision.gameObject))
            {
                // 피해 로직을 여기에 구현
                if (collision.TryGetComponent<IHitable>(out IHitable hitable))
                {
                    hitable.DamageEvent(collideDamage, transform.position);
                    ShowHitEffect(hitParticle, collision.transform.position, collision.transform.rotation);
                    // 적을 맞춘 리스트에 추가
                    hitTargets.Add(collision.gameObject);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_activate) return;

        //폭발 데미지
        if (collision.collider.CompareTag("Planet"))
        {
            gravity.CancelFixedGravity();
            rb.velocity = Vector3.zero;
            //폭발 각도 지정
            explodeNormal = (transform.position - collision.transform.position).normalized;
            //폭발 생성
            ExplodeAsteroid();
            StartCoroutine(DestroyRoutine(0.5f));
        }
    }

    void ExplodeAsteroid()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, explodeRange, Vector2.right, 0f, LayerMask.GetMask("Enemy"));
        
        foreach(RaycastHit2D hit in hits)
        {
            if(hit.transform.TryGetComponent<IHitable>(out IHitable hitable))
            {
                hitable.DamageEvent(explodeDamage, transform.position);
                ShowHitEffect(hitParticle, hit.transform.position, hit.transform.rotation);
            }
        }
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, explodeNormal);
        ShowHitEffect(explodeParticle, transform.position, rot);
    }

    protected void ShowHitEffect(ParticleSystem particle, Vector2 pos, Quaternion rot)
    {
        if (particle != null)
            GameManager.Instance.particleManager.GetParticle(particle, pos, rot);
    }

    IEnumerator DestroyRoutine(float time)
    {
        viewObj.SetActive(false);
        yield return new WaitForSeconds(time);
        _activate = false;
        gameObject.SetActive(false);
    }
   
}
