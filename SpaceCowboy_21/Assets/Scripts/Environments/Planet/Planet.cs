using System.Collections;

using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Planet : MonoBehaviour
{
    [Header("Gravity Properties")]
    public float planetRadius = 2f;
    public float gravityRadius = 4f;
    public float gravityForce = 300f;
    //public LayerMask targetLayer;
    CircleCollider2D circleColl;

    [Header("Planet Properties")]
    //�༺�� ������ �� ����Ʈ
    List<GameObject> enemyList = new List<GameObject>();
    //�༺�� ������ ������Ʈ ����Ʈ
    //�༺�� ������ Neutral Enemy ����Ʈ
    //�༺�� ������ Neutral Enemy ��
    //�༺�� ������ Neutral Enemy ������ �ð�


    private void Awake()
    {
        //CheckGravityAtStart();
        circleColl = GetComponentInChildren<CircleCollider2D>();
        circleColl.radius = gravityRadius;
    }

    /*
    void CheckGravityAtStart()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, GetComponent<Planet>().gravityRadius, Vector2.right, 0, targetLayer);
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.TryGetComponent<GravityFinder>(out GravityFinder finder))
                {
                    finder.gravityPlanets.Add(this.transform);
                }
            }
        }
    }
    */

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent<Gravity>(out Gravity gravity))
        {
            gravity.gravityPlanets.Add(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent<Gravity>(out Gravity gravity))
        {
            gravity.gravityPlanets.Remove(this);
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(this.transform.position, gravityRadius);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(this.transform.position, planetRadius);


    }
}
