using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpRocket : MonoBehaviour
{
    //ĳ���Ͱ� �̰��� Ÿ��, ĳ���͸� �ڼ����� ���� �� ��ġ�� �̵���Ų��. 
    //���� �� �������� �̵��Ѵ�, ž���ϸ� ���� ĳ��Ʈ�� �� ��ġ�� ���δ�. 
    //���� �������� �����ϸ� ���ΰ��� ������ ����/������Ʈ ���� ����������. 

    public float maxDistance = 100f;
    public float speed = 3f;
    public float explodeRange = 2f;
    public float explodePower = 1f;
    public ParticleSystem launchParticle;
    public ParticleSystem explosionParticle;
    
    private Transform playerTr;
    private Vector2 targetPos;
    private int landTargetLayer;
    private int explodeLayer;


    Rigidbody2D rb;
    Collider2D col;
    SpriteRenderer spr;
    

    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spr = GetComponentInChildren<SpriteRenderer>();

        //LayerMash�ʱ�ȭ
        landTargetLayer = 1 << LayerMask.NameToLayer("Planet");
        explodeLayer = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Boxes");

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //�÷��̾ �ڼ����� ����� ������ �ʰ� �ٲ۴�. 
            playerTr = collision.transform;
            playerTr.SetParent(transform, false);
            playerTr.localPosition = Vector2.zero;
            playerTr.gameObject.SetActive(false);
            //��ǥ ������ Ȯ���Ѵ�
            targetPos = CheckTargetPos();
            //�߻�
            StartCoroutine(Launch());
        }
    }

    Vector2 CheckTargetPos()
    {
        Vector2 tPos;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, maxDistance, landTargetLayer); 
        if(hit.collider != null)
        {
            tPos = hit.point;
        }
        else
        {
            tPos = transform.position + transform.up * maxDistance;
        }

        return tPos;
    }

    void ExplosiveLanding()
    {
        //������ �ӵ� 0���� ����
        rb.velocity = Vector2.zero;
        
        //���� ��ƼŬ ���
        launchParticle.Stop();
        explosionParticle.Play();

        //�ֺ��� ����� ��
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explodeRange, explodeLayer);
        if (hits.Length > 1)
        {
            for(int i = 0; i < hits.Length; i++)
            {
                hits[i].TryGetComponent<Health>(out Health health);
                //health.KnockBack(transform, explodePower);
            }
        }


        //ĳ���� ������
        playerTr.SetParent(transform.parent,true);
        playerTr.gameObject.SetActive(true);


        //�浹 �� ���ּ� Model ��� ����
        col.enabled = false;
        spr.enabled = false;

        //����
        StartCoroutine(DestroyFuction());
    }



    IEnumerator Launch()
    {
        Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;
        
        //���� ��ƼŬ ���
        launchParticle.Play();
        
        while (true)
        {
            //����
            rb.AddForce(moveDir * speed * 100f * Time.deltaTime);

            //targetPos�� �����ϸ� �����ϸ� �÷��̾ �����ش�.
            if ((targetPos - rb.position).sqrMagnitude < 0.3f * 0.3f)
            {
                ExplosiveLanding();
                yield break;
            }

            yield return null;
        }
    }

    IEnumerator DestroyFuction()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }
}
