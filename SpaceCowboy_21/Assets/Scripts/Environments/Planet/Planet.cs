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
    public bool activate = false;   //�༺�� Ȱ��ȭ �Ǿ�����

    [Header("Gravity Properties")]
    public float gravityMultiplier = 1f;    // �߷¹��
    //public bool isGrabProjectile = false;   //�Ѿ��� �����⳪��?

    public float planetRadius = 2f;
    public float gravityRadius = 4f;
    public float planetFOV = 90f;

    //��ũ��Ʈ
    public Transform gravityViewer;
    public PolygonCollider2D polyCollider;

    CircleCollider2D circleColl;


    [Header("Planet Properties")]
   
    //public List<EnemyBrain> enemyList = new List<EnemyBrain>();
    //public List<Vector3> enemyStartPos = new List<Vector3>();
    public List<PlanetBridge> linkedPlanetList = new List<PlanetBridge>();
    public event System.Action PlanetWakeUpEvent;


    public Vector2[] worldPoints;   //�༺Coll points �� ���� ��. 
    public Vector2[] pointsNormal;  //points�� �븻���� ����. 


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

        //���� �� Points ��� (ȸ������ �ʴ´ٴ� ���� �Ͽ�)
        worldPoints = new Vector2[polyCollider.points.Length];
        pointsNormal = new Vector2[polyCollider.points.Length];
        worldPoints = GetPointsWorldPositions(polyCollider.points);
        pointsNormal = GetPointsNormals(worldPoints);
    }

    void SetViewerMaterial()
    {
        //viewer�� �������� ����
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
        //�༺�� ȭ�� �ȿ� �������� Ȯ���Ѵ�. 
        //ȭ�� �ȿ� ������ �༺�� �����. 
        if (activate) return;   //�༺ Ȱ��ȭ ���� üũ

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

        //�� ������ ���� �̺�Ʈ ����
        if (PlanetWakeUpEvent != null) PlanetWakeUpEvent();
        //WakeUpEnemies();

        //StartPlanetEvent();

        //��ũ �� �༺���� LinkedWakeUp�� �����Ų��.
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
        //��ũ��� �༺���� �����.
        if (activate)
            return;

        activate = true;

        WakeUpEnemies();

        StartPlanetEvent();
    }

    //Idle, Ambush ������ �����. 
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


    //�༺���� �������� Ư�� �̺�Ʈ��. ��� �������ٰų�, ���� ��ȯ�ȴٰų�... 
    public virtual void StartPlanetEvent()
    {

    }

    #endregion

    #region Get JumpPoints

    //�÷��̾� �߰� ��, Ư�� �༺ �������� ���� �ⱸ index�� ���Ѵ�. 
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

        //����� �༺�� ������
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

    //����� �༺���� JumpPoints�� �����´�
    public bool GetLinkedJumpPoint(Planet targetPlanet, Planet startPlanet)
    {
        if (linkedPlanetList.Count <= 1) return false;

        bool findPlayer = false;

        foreach (PlanetBridge bridge in linkedPlanetList)
        {
            //startPlanet ����
            if (bridge.planet == startPlanet) continue;
            //targetPlanetã���� true ��ȯ
            else if (bridge.planet == targetPlanet)
            {
                findPlayer = true; break;
            }
            //�� ���� ��� bridge planet���� �ѹ��� ��ũ �˻�
            else
            {
                //�ٸ� ���� 1 ���϶�� ����
                if (bridge.planet.linkedPlanetList.Count <= 1) return false;

                foreach (PlanetBridge bridge2 in bridge.planet.linkedPlanetList)
                {
                    //���� StartPlanet ����
                    if (bridge2.planet == bridge.planet) continue;
                    //Ÿ���� ã���� ture ��ȯ.
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

    //���� ���� �� ���
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
    //���� ���� �� ���
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

    //����Ʈ ��������
    public Vector2[] GetPoints(float height)
    {
        Vector2[] v2 = new Vector2[worldPoints.Length];
        for(int i = 0; i < v2.Length; i++)
        {
            v2[i] = worldPoints[i] + (pointsNormal[i] * height);
        }
        return v2;
    }

    //pos���� ���� ����� ����Ʈ Index�� ���Ѵ�.
    public int GetClosestIndex(Vector2 pos)
    {
        int index;
        int approxIndex = 0;
        int secondIndex;
        float closestDistance = float.MaxValue;
        Vector2 pointPosition;
        float distance;

        // 1. ���� ����Ʈ�� 10���� ������ ���� ����� ����Ʈ�� ���Ѵ�. 
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

        // 2. approxIndex�� �յڷ� -5 ~ +4 ���� ���鿡�� ���Ѵ�.(10��) 
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



