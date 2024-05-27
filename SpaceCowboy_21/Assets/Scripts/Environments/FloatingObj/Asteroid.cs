using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    /// <summary>
    /// �߷� �ѿ� ������ �������� ��������. 
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

        // ���� �浹�ߴ��� Ȯ��
        if (collision.CompareTag("Enemy"))
        {
            // �̹� ���� ���� �ƴ϶�� ó��
            if (!hitTargets.Contains(collision.gameObject))
            {
                // ���� ������ ���⿡ ����
                if (collision.TryGetComponent<IHitable>(out IHitable hitable))
                {
                    hitable.DamageEvent(collideDamage, transform.position);
                    ShowHitEffect(hitParticle, collision.transform.position, collision.transform.rotation);
                    // ���� ���� ����Ʈ�� �߰�
                    hitTargets.Add(collision.gameObject);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_activate) return;

        //���� ������
        if (collision.collider.CompareTag("Planet"))
        {
            gravity.CancelFixedGravity();
            rb.velocity = Vector3.zero;
            //���� ���� ����
            explodeNormal = (transform.position - collision.transform.position).normalized;
            //���� ����
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
