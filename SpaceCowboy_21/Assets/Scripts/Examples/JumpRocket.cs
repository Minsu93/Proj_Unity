using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpRocket : MonoBehaviour
{
    //캐릭터가 이곳에 타면, 캐릭터를 자손으로 만든 뒤 위치를 이동시킨다. 
    //직선 위 방향으로 이동한다, 탑승하면 레이 캐스트로 그 위치가 보인다. 
    //도착 지점에서 폭발하며 주인공을 제외한 적들/오브젝트 들을 날려버린다. 

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

        //LayerMash초기화
        landTargetLayer = 1 << LayerMask.NameToLayer("Planet");
        explodeLayer = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("Boxes");

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //플레이어를 자손으로 만들고 보이지 않게 바꾼다. 
            playerTr = collision.transform;
            playerTr.SetParent(transform, false);
            playerTr.localPosition = Vector2.zero;
            playerTr.gameObject.SetActive(false);
            //목표 지점을 확인한다
            targetPos = CheckTargetPos();
            //발사
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
        //도착시 속도 0으로 만듬
        rb.velocity = Vector2.zero;
        
        //폭발 파티클 출력
        launchParticle.Stop();
        explosionParticle.Play();

        //주변에 충격파 줌
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explodeRange, explodeLayer);
        if (hits.Length > 1)
        {
            for(int i = 0; i < hits.Length; i++)
            {
                hits[i].TryGetComponent<Health>(out Health health);
                //health.KnockBack(transform, explodePower);
            }
        }


        //캐릭터 내리기
        playerTr.SetParent(transform.parent,true);
        playerTr.gameObject.SetActive(true);


        //충돌 및 우주선 Model 모양 숨김
        col.enabled = false;
        spr.enabled = false;

        //제거
        StartCoroutine(DestroyFuction());
    }



    IEnumerator Launch()
    {
        Vector2 moveDir = (targetPos - (Vector2)transform.position).normalized;
        
        //추진 파티클 출력
        launchParticle.Play();
        
        while (true)
        {
            //가속
            rb.AddForce(moveDir * speed * 100f * Time.deltaTime);

            //targetPos에 도착하면 폭발하며 플레이어를 내려준다.
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
