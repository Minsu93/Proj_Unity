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
    //public bool isGrabProjectile = false;   //총알을 끌어당기나요?

    public float planetRadius = 2f;
    public float gravityRadius = 4f;
    public float planetFOV = 90f;

    //스크립트
    public Transform gravityViewer;
    public PolygonCollider2D polyCollider;

    CircleCollider2D circleColl;


    [Header("Planet Properties")]
   
    //public List<EnemyBrain> enemyList = new List<EnemyBrain>();
    //public List<Vector3> enemyStartPos = new List<Vector3>();
    public List<PlanetBridge> linkedPlanetList = new List<PlanetBridge>();
    public event System.Action PlanetWakeUpEvent;


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

        int targetLayer = 1 << LayerMask.NameToLayer("Enemy");
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, gravityRadius, Vector2.right, 0f, targetLayer);
        if(hits.Length > 0)
        {
            foreach(RaycastHit2D hit in hits)
            {
                hit.transform.GetComponentInParent<Gravity>().AddToGravityList(this);
            }
        }

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

    private void Update()
    {
        //행성이 화면 안에 들어오는지 확인한다. 
        //화면 안에 들어오면 행성을 깨운다. 
        if (activate) return;   //행성 활성화 여부 체크

        if (PlanetBoundIsInScreen())
        {
            if (PlanetWakeUpEvent != null) PlanetWakeUpEvent();
            activate = true;
        }
    }

    #region Gravity

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (activate)
    //        return;

    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        WakeUpPlanet();
    //    }
    //}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent<Gravity>(out Gravity gravity))
        {
            gravity.AddToGravityList(this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent<Gravity>(out Gravity gravity))
        {
            gravity.RemoveFromGravityList(this);
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

    #endregion

    #region Planet Events

    bool PlanetBoundIsInScreen()
    {
        Bounds bounds = polyCollider.bounds;

        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(bounds.min.x, bounds.min.y, 0); // Bottom-left
        corners[1] = new Vector3(bounds.max.x, bounds.min.y, 0); // Bottom-right
        corners[2] = new Vector3(bounds.max.x, bounds.max.y, 0); // Top-right
        corners[3] = new Vector3(bounds.min.x, bounds.max.y, 0); // Top-left

        foreach (var corner in corners)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(corner);

            if (screenPoint.x > 0 && screenPoint.x < Screen.width && screenPoint.y > 0 && screenPoint.y < Screen.height)
            {
                return true; // One of the corners is outside the screen
            }
        }

        return false; // All corners are inside the screen
    }

    void WakeUpPlanet()
    {
        if (activate) return;     
        activate = true;

        //적 움직임 시작 이벤트 실행
        if (PlanetWakeUpEvent != null) PlanetWakeUpEvent();
        //WakeUpEnemies();

        //StartPlanetEvent();

        //링크 된 행성들의 LinkedWakeUp을 실행시킨다.
        //if (linkedPlanetList.Count > 0)
        //{
        //    foreach (PlanetBridge linkedPlanet in linkedPlanetList)
        //    {
        //        linkedPlanet.planet.LinkedWakeUp();
        //    }
        //}
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
        //if (enemyList.Count > 0)
        //{
        //    foreach (EnemyBrain brain in enemyList)
        //    {
        //        brain.WakeUp();
        //    }
        //}
    }


    //행성에서 벌어지는 특수 이벤트들. 운석이 떨어진다거나, 적이 소환된다거나... 
    public virtual void StartPlanetEvent()
    {

    }

    #endregion

    #region Get JumpPoints

    //플레이어 추격 시, 특정 행성 방향으로 가는 출구 index를 구한다. 
    public bool GetjumpPoint(Planet planet, out PlanetBridge _bridge)
    {
        bool hasBridge = false;

        _bridge = new PlanetBridge();

        if (linkedPlanetList.Count == 0) return hasBridge;

        foreach(PlanetBridge bridge in linkedPlanetList)
        {
            if(bridge.planet == planet)
            {
                hasBridge = true;
                _bridge = bridge; 
                break;
            }
        }

        //연결된 행성이 없으면
        if(_bridge.planet == null)
        {
            foreach(PlanetBridge bridge in linkedPlanetList)
            {
                if (bridge.planet.GetLinkedJumpPoint(planet, this))
                {
                    hasBridge = true;
                    _bridge = bridge; 
                    break;
                }
            }
        }

        return hasBridge;
    }

    //연결된 행성에서 JumpPoints를 가져온다
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

    #endregion

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

    //게임 시작 시 계산
    Vector2[] GetPointsWorldPositions(Vector2[] points)
    {
        Vector3[] ppoints = new Vector3[points.Length];
        ppoints = points.toVector3Array();
        for (int i = 0;  i < ppoints.Length; i++)
        {
            ppoints[i] = transform.TransformPoint(ppoints[i]);
        }
        //transform.TransformPoints(ppoints);
        Vector2[] v2 = ppoints.toVector2Array();
        return v2;
    }
    //게임 시작 시 계산
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

    //포인트 가져오기
    public Vector2[] GetPoints(float height)
    {
        Vector2[] v2 = new Vector2[worldPoints.Length];
        for(int i = 0; i < v2.Length; i++)
        {
            v2[i] = worldPoints[i] + (pointsNormal[i] * height);
        }
        return v2;
    }

    //pos에서 가장 가까운 포인트 Index를 구한다.
    public int GetClosestIndex(Vector2 pos)
    {
        int index;
        int approxIndex = 0;
        int secondIndex;
        float closestDistance = float.MaxValue;
        Vector2 pointPosition;
        float distance;

        // 1. 월드 포인트를 10으로 나눠서 가장 가까운 포인트를 구한다. 
        for (int i = 0; i < worldPoints.Length; i++)
        {
            if (i % 10 != 0) continue;

            pointPosition = worldPoints[i];
            distance = Vector2.Distance(pos, pointPosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                approxIndex = i;
            }
        }
        index = approxIndex;
        //closestDistance = float.MaxValue;

        // 2. approxIndex의 앞뒤로 -5 ~ +4 사이 점들에서 비교한다.(10개) 
        for (int j = -5; j < 5; j++)
        {
            secondIndex = (approxIndex + j + worldPoints.Length) % worldPoints.Length;
            pointPosition = worldPoints[secondIndex];
            distance = Vector2.Distance(pos, pointPosition);

            if(distance < closestDistance)
            {
                closestDistance = distance;
                index = secondIndex;
            }

            Debug.DrawLine(pos, pointPosition, Color.cyan,1f);
        }
        return index;
    }
    #endregion

    [ContextMenu("Clear Linked Planets")]
    void ClearLinkedPlanets()
    {
        linkedPlanetList.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, planetRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, gravityRadius);
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



