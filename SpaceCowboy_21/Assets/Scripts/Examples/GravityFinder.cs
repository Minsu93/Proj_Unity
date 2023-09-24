using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFinder: MonoBehaviour
{

    //�� ������Ʈ�� ������ �ִ� �༺�� ����Ʈ 
    public List<Transform> gravityPlanets = new List<Transform>();
    public Transform nearestGround;
    public Vector2 nearestPoint;
    public float forceMultiplier = 1f;
    public bool activate = true;

    LayerMask planetLayer;
    float force;
    public bool fixGravityOn = false;
    public Vector2 fixGravityDir = Vector2.zero;
    public bool charGravity = false;

    Rigidbody2D rb;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //force = GameManager.Instance.worldGravity;

        planetLayer = 1 << LayerMask.NameToLayer("Planet");
    }
    private void Update()
    {
        if (!activate)
            return;

        nearestGround = GetGroundPlanet();

        if (nearestGround == null)
            return;

        GetNearestPoint();
    }

    private void FixedUpdate()
    {
        if (!activate)
            return;

        if (fixGravityOn)
        {
            rb.AddForce(fixGravityDir * force * forceMultiplier * Time.deltaTime, ForceMode2D.Force);
            return;
        }


        if (nearestGround == null)
            return;


        if (charGravity)
        {

            Debug.DrawLine(transform.position, nearestPoint, Color.red);

            //Vector2 grav = (nearestGround.position - transform.position).normalized;
            Vector2 grav = (nearestPoint - (Vector2)transform.position).normalized;
            rb.AddForce(grav * force * forceMultiplier * Time.deltaTime, ForceMode2D.Force);
            return;
        }

        float totalGrav = 0f ;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            Planet planet = gravityPlanets[i].GetComponent<Planet>();
            
            float planetRadius = planet.planetRadius;   //�༺�� ������
            float gravRadius = planet.gravityRadius;   //�༺�� �߷� ������
            
            Vector2 targetVec = gravityPlanets[i].position - this.transform.position;   //�߷��� ����
            
            if(targetVec.magnitude < planetRadius)  // �༺ ������ ���
            {
                totalGrav = 1f;
            }
            else
            {
                //float upVec = Vector2.SignedAngle(transform.right, targetVec) < 0 ? 1 : -1;   //�ش� �߷��� ������ ��(+)�ΰ� ��(-)�ΰ�
                float upVec = gravityPlanets[i] == nearestGround ? 1f : -1f;
                //float percent = Mathf.Round( targetVec.magnitude - planetRadius) / (gravRadius - planetRadius);
                float percent = Mathf.Clamp01((targetVec.magnitude - planetRadius) / (gravRadius - planetRadius)); 
                float tempGrav = upVec * (1f - percent);

                totalGrav += tempGrav;
            }

        }

        Vector2 gravity = (nearestGround.position - this.transform.position).normalized;    //nearestGround�������� ���ϴ� ����
        rb.AddForce(gravity * totalGrav * force * forceMultiplier * Time.deltaTime, ForceMode2D.Force);

    }

    Transform GetGroundPlanet()
    {
        Transform groundPlanet = null;
        float minDist = 1000f;

        for (int i = 0; i < gravityPlanets.Count; i++)
        {
            if (gravityPlanets[i] == null)
                continue;

            Vector2 targetVec = gravityPlanets[i].position - this.transform.position;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, targetVec.normalized, 100f, planetLayer);
            if(hit.collider != null)
            {
                if (hit.distance < minDist)
                {
                    minDist = hit.distance;
                    groundPlanet = gravityPlanets[i];
                }
            }
            else //�浹�Ǵ� ���� ������, �׳� transform�� ���� �������� �������.
            {
                if (targetVec.magnitude < minDist)
                {
                    minDist = targetVec.magnitude;
                    groundPlanet = gravityPlanets[i];
                }
            }

        }


        return groundPlanet;
    }

    public void FixedGravity(Vector2 dir)
    {
        if (!fixGravityOn)
        {
            fixGravityOn = true;
            fixGravityDir = dir;
        }
        else
        {
            fixGravityOn = false;
            fixGravityDir = Vector2.zero;
        }
        
    }

    void GetNearestPoint()
    {
        Collider2D coll = nearestGround.GetComponentInChildren<EdgeCollider2D>();
        nearestPoint = coll.ClosestPoint(transform.position);
    }

}
