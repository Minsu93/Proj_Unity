using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : Obstacle
{
    float checkInterval = 0.2f;
    float timer;

    protected override void HitEvent()
    {
        //피격 이펙트
    }

    protected override void DestroyEvent()
    {
        coll.enabled = false;
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if(timer > checkInterval)
        {
            timer = 0f;
            MoveAwayFromPlanet();
        }
    }
    //행성과 너무 가까우면 살짝 멀어지려는 노력 
    void MoveAwayFromPlanet()
    {
        //행성과의 거리 측정 
        float checkRadius = 3f;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, checkRadius, Vector2.right, 0f, LayerMask.GetMask("Planet"));

        if(hits.Length > 0 )
        {
            float minDist = float.MaxValue;
            Collider2D closestColl;
            Vector2 closestPoint = transform.position;

            foreach(RaycastHit2D hit in hits )
            {
                Collider2D coll = hit.transform.GetComponent<Collider2D>();
                Vector2 point = coll.ClosestPoint(transform.position);
                float dist = Vector2.Distance(point, transform.position);
                if(dist < minDist)
                {
                    minDist = dist;
                    closestColl = coll;
                    closestPoint = point;
                }
            }

            Vector2 dir = (Vector2)transform.position - closestPoint;
            float d = dir.magnitude;
            if(d > 0f)
            {
                rb.AddForce(dir.normalized, ForceMode2D.Force);
            }
        }
    }
}
