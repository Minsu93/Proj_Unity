using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    bool startWave;

    [SerializeField]
    FlagPoints[] flagPoints;

    [SerializeField]
    CinemachineVirtualCamera[] virtualCams;

    [SerializeField]
    MapBorder[] mapBorders;

    [SerializeField]
    WaveManager waveManager;

    [ContextMenu("Get All Vcams")]
    void GetAllVirtualCameras()
    {
        virtualCams = transform.GetComponentsInChildren<CinemachineVirtualCamera>();    
    }

    [ContextMenu("Get All MapBorder Transforms")]
    void GetAllMapBorderTransforms()
    {
        mapBorders = FindObjectsOfType<MapBorder>();
    }


    int currCamIndex = 0;
    int preStageIndex = 0;
    int currStageIndex = 0;
    public int CurrStageIndex {  get { return currStageIndex; } }
    [SerializeField] int maxStage = 5;
    public int MaxStage { get { return maxStage; } }


    private void Start()
    {
        StartCoroutine(StartChapter());
    }

    IEnumerator StartChapter()
    {
        //1. ī�޶� ���� ��ġ��Ų��.
        GameManager.Instance.cameraManager.InitCam();
        GameManager.Instance.cameraManager.ChangeCamera(null, virtualCams[0]);

        yield return new WaitForSeconds(0.5f);
        //2. ȭ�� ���� �������� ĳ���͸� ���� 
        Vector2 point = waveManager.GetPointFromOutsideScreen(flagPoints[0].startPoint.position - flagPoints[0].flagPoint.position);
        GameManager.Instance.SpawnPlayer(point, Quaternion.identity);

        //3. ĳ���Ͱ� ���Ƽ� �����ϰ� �Ѵ�. 
        GameManager.Instance.playerManager.SuperBoost(point, flagPoints[0].flagPoint, true, StartWave);

        //4. �� ������Ʈ
        GameManager.Instance.MapSize = new Vector2(mapBorders[0].width / 2, mapBorders[0].height / 2);
        GameManager.Instance.MapCenter = mapBorders[0].transform.position;
        waveManager.AddStagePlanets(mapBorders[0].transform);

        //MapBorder Ȱ��ȭ
        mapBorders[0].ActivateBorder = true;

    }

    /// <summary>
    /// ���������� ������ ����.
    /// </summary>
    public void FinishStage()
    {
        //1. ĳ���� ���� ����
        GameManager.Instance.playerManager.playerBehavior.DeactivatePlayer(false);
        
        //2. ī�޶� ���� ����.
        GameManager.Instance.cameraManager.StopCameraFollow();

        //3. ���� �������� MapBorder ��Ȱ��ȭ
        mapBorders[0].ActivateBorder = false;

        //���� �������� ����
        if (currStageIndex +1 < maxStage)
        {
            mapBorders[currStageIndex + 1].gameObject.SetActive(true);
            MoveAStarNavMesh(mapBorders[currStageIndex+1].transform.position);
        }

        //4. ĳ���Ͱ� ȭ�� ������ ���ư��� �ִϸ��̼� ����.
        Vector2 point = waveManager.GetPointFromOutsideScreen(flagPoints[currStageIndex].endPoint.position - flagPoints[currStageIndex].flagPoint.position);
        GameManager.Instance.playerManager.SuperBoost(point, GameManager.Instance.player, false, MoveStartToNextStage);

    }


    /// <summary>
    /// Del �� SuperBoost�� ������ ����� �̺�Ʈ.
    /// </summary>
    void StartWave()
    {
        // StartCoroutine();
        GameManager.Instance.cameraManager.FollowCamera();
        //MapBorder Ȱ��ȭ
        mapBorders[currStageIndex].ActivateBorder = true;

        if (startWave) waveManager.WaveStart(CurrStageIndex);
    }

    /// <summary>
    /// Del �� FinishStage���� ����� SuperBoost�� ȭ�� ������ ������ ����.
    /// </summary>
    void MoveStartToNextStage()
    {
        preStageIndex = currStageIndex;

        //1. �������� �ε��� ���
        currStageIndex++;

        //2. ī�޶� �ʱ�ȭ.
        GameManager.Instance.cameraManager.ChangeCamera(virtualCams[preStageIndex], virtualCams[currStageIndex]);

        //3.�� ũ�� ������Ʈ
        GameManager.Instance.MapSize = new Vector2(mapBorders[currStageIndex].width / 2, mapBorders[currStageIndex].height / 2);
        GameManager.Instance.MapCenter = mapBorders[currStageIndex].transform.position;

        //4. ���� �� ����
        mapBorders[preStageIndex].gameObject.SetActive(true);

        StartCoroutine(MoveComplete());
    }

    IEnumerator MoveComplete()
    {
        yield return new WaitForSeconds(2.0f);

        //ĳ���� ����(�̵�)
        Vector2 point = waveManager.GetPointFromOutsideScreen(flagPoints[currStageIndex].startPoint.position - flagPoints[currStageIndex].flagPoint.position);
        GameManager.Instance.playerManager.SuperBoost(point, flagPoints[currStageIndex].flagPoint, true, StartWave);

        //���̺� Ȱ��ȭ
        waveManager.AddStagePlanets(mapBorders[currStageIndex].transform);
    }


    public bool IsNextStageAvailable()
    {
        if (currStageIndex + 1 < maxStage)
        {
            return true;
        }
        else return false;
    }


    void MoveAStarNavMesh(Vector3 newPos)
    {
        AstarPath.active.AddWorkItem(() => {
            // Move the graph to the origin, with no rotation, and with a node size of 2.0
            var gg = AstarPath.active.data.gridGraph;
            gg.RelocateNodes(center: newPos, rotation: Quaternion.Euler(gg.rotation), nodeSize: 2.0f);
        });
        AstarPath.active.FlushWorkItems();
        // Scan the graph at the new position
        AstarPath.active.Scan();
    }

}

[Serializable]
public class FlagPoints
{
    public Transform startPoint;
    public Transform endPoint;
    public Transform flagPoint;


}