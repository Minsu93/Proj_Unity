using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace PlanetSpace
{
    [SelectionBase]
    public class Planet : MonoBehaviour
    {
        public PlanetSize planetSize = PlanetSize.S; //행성 크기

        [Header("Gravity Properties")]
        public float planetRadius = 2f;
        public float gravityRadius = 4f;
        public float gravityForce = 300f;
        //public LayerMask targetLayer;
        public Transform gravityViewer;
        CircleCollider2D circleColl;
        public float oxygenAmount = 1f;
        public float lens = 8f;

 

        [Header("Planet Properties")]
        //행성에 등장할 적 리스트
        List<GameObject> enemyList = new List<GameObject>();
        //행성에 등장할 오브젝트 리스트
        //행성에 등장할 Neutral Enemy 리스트
        //행성에 등장할 Neutral Enemy 수
        //행성에 등장할 Neutral Enemy 리스폰 시간

        public EdgeCollider2D edgeColl { get; private set; }

        private void OnValidate()
        {
            edgeColl = GetComponent<EdgeCollider2D>();
            

        }

        private void Awake()
        {
            //CheckGravityAtStart();
            circleColl = GetComponentInChildren<CircleCollider2D>();
            circleColl.radius = gravityRadius;

            //gravityViewer = GetComponentsInChildren<Transform>(true)[2];
            SetViewerMaterial();


        }

        void SetViewerMaterial()
        {
            //viewer의 스케일을 조절
            gravityViewer.localScale = Vector3.one * gravityRadius * 2f;

            Material gravMat = gravityViewer.GetComponent<Renderer>().material;
            float lineWidth = gravMat.GetFloat("_LineWidth");
            float spacing = gravMat.GetFloat("_Segment_Spacing");
            float count = gravMat.GetFloat("_Segment_Count");
            gravMat.SetFloat("_LineWidth", lineWidth / gravityRadius);
            gravMat.SetFloat("_Segment_Spacing", spacing / gravityRadius);
            gravMat.SetFloat("_Segment_Count", count * gravityRadius);
            gravityViewer.gameObject.SetActive(false);
        }

        

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


        public void graviteyViewOn()
        {
            gravityViewer.gameObject.SetActive(true);
        }

        public void graviteyViewOff()
        {
            gravityViewer.gameObject.SetActive(false);
        }

       


        private void OnDrawGizmos()
        {
            //Gizmos.color = Color.gray;
            //Gizmos.DrawWireSphere(this.transform.position, gravityRadius);
            Gizmos.color = new Color(0, 1, 1, 0.3f);
            Gizmos.DrawWireSphere(this.transform.position, planetRadius + 3.1f);
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawWireSphere(this.transform.position, planetRadius + 4.7f);
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(this.transform.position, planetRadius);


        }
    }

    public enum PlanetSize { XS, S, SM, M, L, XL }

}


