using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlanetLinker : MonoBehaviour
{
    bool activeLinker = false;
    public Planet planet;
    PolygonCollider2D polyColl;
    public float maxLinkedDistance = 12f;


    private void OnEnable()
    {
        if (planet == null) planet = GetComponent<Planet>();
        if (polyColl== null) polyColl = GetComponent<PolygonCollider2D>();
        GetLinkedPlanets();
        activeLinker = true;
    }

    [ContextMenu("Get Linked Planets")]
    //이 행성과 Linked 되어있는 행성, 그리고 그 출입point를 구한다. 
    void GetLinkedPlanets()
    {
        //기존 리스트를 비운다
        planet.linkedPlanetList.Clear();

        //어느정도 가까이 있는 행성을 구한다 
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 30f, LayerMask.GetMask("Planet"));
        if (colls.Length > 0)
        {
            foreach (Collider2D overlapColl in colls)
            {
                //자기 자신은 제외한다
                if (overlapColl.transform == this.transform) continue;

                PlanetBridge bridge = new PlanetBridge();

                //행성에 가장 가까운 Index를 고른다. 
                Transform target = overlapColl.transform;
                Planet targetP = target.GetComponent<Planet>();
                PolygonCollider2D coll = overlapColl as PolygonCollider2D;

                float dist = float.MaxValue;
                int bridgeIndex = 0;
                Vector2 targetVector = Vector2.zero;
                int Counts = planet.polyCollider.points.Length - 1;

                for (int i = 0; i < Counts; i++)
                {
                    if (i % 2 != 0) continue;
                    Vector2 from = GetPointPos(i);
                    Vector2 to = coll.ClosestPoint(from);
                    float distan = Vector2.Distance(from, to);
                    if (distan < dist)
                    {
                        dist = distan;
                        targetVector = to;
                        bridgeIndex = i;
                    }
                }

                if (dist >= maxLinkedDistance) continue;

                bridge.planet = targetP;
                bridge.bridgeIndex = bridgeIndex;
                bridge.targetVector = targetVector;
                bridge.bridgeDistance = dist;
                bridge.planetDistance = Mathf.FloorToInt( Vector2.Distance(this.planet.transform.position, targetP.transform.position));

                planet.linkedPlanetList.Add(bridge);
            }
        }
        //RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 30f, Vector2.right, 0f, LayerMask.GetMask("Planet"));
        //if (hits.Length > 0)
        //{
        //    foreach (RaycastHit2D hit in hits)
        //    {
        //        //자기 자신은 제외한다
        //        if (hit.transform == this.transform) continue;

        //        PlanetBridge bridge = new PlanetBridge();

        //        //행성에 가장 가까운 Index를 고른다. 
        //        Transform target = hit.collider.transform;
        //        Planet targetP = target.GetComponent<Planet>();
        //        PolygonCollider2D coll = target.GetComponent<PolygonCollider2D>();
        //        float dist = float.MaxValue;
        //        int bridgeIndex = 0;
        //        Vector2 targetVector = Vector2.zero;
        //        int Counts = planet.polyCollider.points.Length - 1;

        //        for (int i = 0; i < Counts; i++)
        //        {
        //            if (i % 2 != 0) continue;
        //            Vector2 from = GetPointPos(i);
        //            Vector2 to = coll.ClosestPoint(from);
        //            float distan = Vector2.Distance(from, to);
        //            if (distan < dist)
        //            {
        //                dist = distan;
        //                targetVector = to;
        //                bridgeIndex = i;
        //            }
        //        }

        //        if (dist >= maxLinkedDistance) continue;

        //        bridge.planet = targetP;
        //        bridge.bridgeIndex = bridgeIndex;
        //        bridge.targetVector = targetVector;
        //        bridge.bridgeDistance = dist;

        //        planet.linkedPlanetList.Add(bridge);
        //    }
        //}
    }
    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.gray;
        //Gizmos.DrawWireSphere(this.transform.position, gravityRadius);
        //Gizmos.color = new Color(0, 1, 1, 0.3f);
        //Gizmos.DrawWireSphere(this.transform.position, planet.planetRadius);
        //Gizmos.color = new Color(0, 0, 1, 0.5f);
        //Gizmos.DrawWireSphere(this.transform.position, planet.gravityRadius);
        if (!activeLinker) return;


        if (planet.linkedPlanetList.Count > 0)
        {
            foreach (PlanetBridge pb in planet.linkedPlanetList)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawLine(GetPointPos(pb.bridgeIndex), pb.targetVector);
            }
        }
    }

    //collider상의 한 점(point)의 월드 위치를 반환한다
    Vector2 GetPointPos(int pointIndex)
    {
        Vector3 localPoint = planet.polyCollider.points[pointIndex];
        Vector2 pointPos = planet.polyCollider.transform.TransformPoint(localPoint);
        return pointPos;
    }
}

