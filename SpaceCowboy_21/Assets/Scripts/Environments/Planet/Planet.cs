using SpaceEnemy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;


[SelectionBase]
public class Planet : MonoBehaviour
{
    public bool activate = false;   //행성이 활성화 되었는지

    [Header("Gravity Properties")]
    public float gravityMultiplier = 1f;    // 중력배수
    public bool isGrabProjectile = false;   //총알을 끌어당기나요?

    public float planetRadius = 2f;
    public float gravityRadius = 4f;
    public float planetFOV = 90f;

    public Transform gravityViewer;
    public PolygonCollider2D polyCollider;

    CircleCollider2D circleColl;


    [Header("Planet Properties")]
   
    public List<EnemyBrain> enemyList = new List<EnemyBrain>();
    public List<Vector3> enemyStartPos = new List<Vector3>();
    public List<PlanetBridge> linkedPlanetList = new List<PlanetBridge>();

    public Vector2[] worldPoints;   //행성Coll points 의 월드 값. 
    public Vector2[] pointsNormal;  //points의 노말방향 값들. 

    private void OnValidate()
    {
        if (polyCollider == null)
            polyCollider = GetComponent<PolygonCollider2D>();
    }

    private void Awake()
    {
        circleColl = GetComponentInChildren<CircleCollider2D>();
        circleColl.radius = gravityRadius;

        if (polyCollider == null)
            polyCollider = GetComponent<PolygonCollider2D>();

        SetViewerMaterial();

        //시작 시 Points 계산 (회전하지 않는다는 가정 하에)
        worldPoints = new Vector2[polyCollider.points.Length];
        pointsNormal = new Vector2[polyCollider.points.Length];
        worldPoints = GetPointsWorldPositions(polyCollider.points);
        pointsNormal = GetPointsNormals(worldPoints);

        
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

    //void SetEnemyStartPosition()
    //{
    //    if (enemyList.Count > 0)
    //    {
    //        for (int i = 0; i < enemyList.Count; i++)
    //        {
    //            enemyStartPos[i] = enemyList[i].transform.position;
    //        }
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (activate)
            return;

        if (collision.collider.CompareTag("Player"))
        {
            WakeUpPlanet();
        }
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


    //총에 맞거나 플레이어가 행성 위로 올라서면 해당 행성의 Idle, Ambush 상태 적들을 모두 깨운다. 행성 이벤트도 실행한다. 
    public void WakeUpPlanet()
    {
        if (activate)
            return;


        activate = true;

        WakeUpEnemies();

        StartPlanetEvent();

        //링크 된 행성들의 LinkedWakeUp을 실행시킨다.
        if (linkedPlanetList.Count > 0)
        {
            foreach (PlanetBridge linkedPlanet in linkedPlanetList)
            {
                linkedPlanet.planet.LinkedWakeUp();
            }
        }
    }

    public void LinkedWakeUp()
    {
        //링크드된 행성들을 깨운다.
        if (activate)
            return;

        activate = true;

        WakeUpEnemies();

        StartPlanetEvent();
    }

    //Idle, Ambush 적들을 깨운다. 
    void WakeUpEnemies()
    {
        if (enemyList.Count > 0)
        {
            foreach (EnemyBrain brain in enemyList)
            {
                brain.WakeUp();
            }
        }
    }


    //행성에서 벌어지는 특수 이벤트들. 운석이 떨어진다거나, 적이 소환된다거나... 
    public virtual void StartPlanetEvent()
    {

    }



    //특정 행성 방향으로 가는 출구 index를 구한다. 
    public PlanetBridge GetjumpPoint(Planet planet)
    {
        PlanetBridge pb = new PlanetBridge();

        foreach(PlanetBridge bridge in linkedPlanetList)
        {
            if(bridge.planet == planet)
            {
                pb = bridge; break;
            }
        }

        //연결된 행성이 없으면
        if(pb.planet == null)
        {
            foreach(PlanetBridge bridge in linkedPlanetList)
            {
                if(bridge.planet.GetLinkedJumpPoint(planet, this))
                {
                    pb = bridge; break;
                }
            }
        }
        return pb;
    }

    //linkedPlanet에서 실행됨
    public bool GetLinkedJumpPoint(Planet targetPlanet, Planet startPlanet)
    {
        if (linkedPlanetList.Count <= 1) return false;

        bool findPlayer = false;

        foreach (PlanetBridge bridge in linkedPlanetList)
        {
            //startPlanet 제외
            if (bridge.planet == startPlanet) continue;
            //targetPlanet찾으면 true 반환
            else if (bridge.planet == targetPlanet)
            {
                findPlayer = true; break;
            }
            //그 외의 경우 bridge planet에서 한번더 링크 검색
            else
            {
                //다리 수가 1 이하라면 리턴
                if (bridge.planet.linkedPlanetList.Count <= 1) return false;

                foreach (PlanetBridge bridge2 in bridge.planet.linkedPlanetList)
                {
                    //이전 StartPlanet 제외
                    if (bridge2.planet == bridge.planet) continue;
                    //타겟을 찾으면 ture 반환.
                    else if (bridge2.planet == targetPlanet)
                    {
                        findPlayer = true;
                        break;
                    }
                }
                if (findPlayer) break;
            }
            
        }



        return findPlayer;
    }


    //void ResetEnemy()
    //{
    //    if (enemyList.Count > 0)
    //    {
    //        for (int i = 0; i < enemyList.Count; i++)
    //        {
    //            if (!enemyList[i].gameObject.activeSelf)
    //                enemyList[i].gameObject.SetActive(true);
    //            enemyList[i].transform.position = enemyStartPos[i];
    //            enemyList[i].ResetEnemyBrain();
    //        }
    //    }
    //}

    #region Points Positions & Normals

    Vector2[] GetPointsWorldPositions(Vector2[] points)
    {
        Vector3[] ppoints = new Vector3[points.Length];
        ppoints = points.toVector3Array();
        transform.TransformPoints(ppoints);
        Vector2[] v2 = ppoints.toVector2Array();
        return v2;
    }

    Vector2[] GetPointsNormals(Vector2[] points)
    {
        Vector2[] v2 = new Vector2[points.Length];
        for(int i = 0; i < points.Length; i++)
        {
            int t = (i + 1) % points.Length;
            v2[i] = Vector2.Perpendicular(points[t] - points[i]).normalized;
        }
        return v2;
    }


    public Vector2[] GetPoints(float height)
    {
        Vector2[] v2 = new Vector2[worldPoints.Length];
        for(int i = 0; i < v2.Length; i++)
        {
            v2[i] = worldPoints[i] + (pointsNormal[i] * height);
        }
        return v2;
    }
    #endregion

    Vector2 GetPointPos(int pointIndex)
    {
        Vector3 localPoint = polyCollider.points[pointIndex];
        Vector2 pointPos = polyCollider.transform.TransformPoint(localPoint);
        return pointPos;
    }

}

[System.Serializable]
public struct PlanetBridge
{
    public Planet planet;
    public int bridgeIndex;
    public Vector2 targetVector;

    public PlanetBridge(Planet planet, int bridgeIndex, Vector2 targetVector)
    {
        this.planet = planet;
        this.bridgeIndex = bridgeIndex;
        this.targetVector = targetVector;
    }
}



