using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

namespace PlanetSpace
{
    [SelectionBase]
    public class Planet : MonoBehaviour
    {
        public PlanetType planetType = PlanetType.Blue; //�༺ Ÿ��

        [Header("Gravity Properties")]
        public float planetRadius = 2f;
        public float gravityRadius = 4f;
        public float gravityForce = 300f;
        //public LayerMask targetLayer;
        public GameObject gravityViewer;
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
            gravityViewer.transform.localScale = Vector3.one * gravityRadius * 2f;
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

            //�༺ ������ ������ ������ ���̰� �Ѵ�.
            if (collision.CompareTag("Player"))
            {
                gravityViewer.SetActive(true);

            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.transform.TryGetComponent<Gravity>(out Gravity gravity))
            {
                gravity.gravityPlanets.Remove(this);
            }

            //�༺ ������ ������ ������ ����.
            if (collision.CompareTag("Player"))
            {
                gravityViewer.SetActive(false);

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

    public enum PlanetType { Green, Blue, Red }

}


