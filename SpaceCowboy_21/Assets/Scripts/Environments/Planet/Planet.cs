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
    public bool isGrabProjectile = false;   //�Ѿ��� �����⳪��?

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


    //�ѿ� �°ų� �÷��̾ �༺ ���� �ö󼭸� �ش� �༺�� Idle, Ambush ���� ������ ��� �����. �༺ �̺�Ʈ�� �����Ѵ�. 
    public void WakeUpPlanet()
    {
        if (activate)
            return;


        activate = true;

        WakeUpEnemies();

        StartPlanetEvent();

        //��ũ �� �༺���� LinkedWakeUp�� �����Ų��.
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
        if (enemyList.Count > 0)
        {
            foreach (EnemyBrain brain in enemyList)
            {
                brain.WakeUp();
            }
        }
    }


    //�༺���� �������� Ư�� �̺�Ʈ��. ��� �������ٰų�, ���� ��ȯ�ȴٰų�... 
    public virtual void StartPlanetEvent()
    {

    }



    //Ư�� �༺ �������� ���� �ⱸ index�� ���Ѵ�. 
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

        //����� �༺�� ������
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

    //linkedPlanet���� �����
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



