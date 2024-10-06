using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    bool startWave;

    [SerializeField]
    StartPoint[] startPoints;

    [SerializeField]
    CinemachineVirtualCamera[] virtualCams;

    [SerializeField]
    MapBorder[] mapBorders;

    [SerializeField]
    WaveManager waveManager;


    [ContextMenu("Get All StartPoints")]
    void GetAllStartPoints()
    {
        startPoints = transform.GetComponentsInChildren<StartPoint>();
    }

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


    int currStageIndex = 0;
    public int CurrStageIndex {  get { return currStageIndex; } }
    [SerializeField] int maxStage = 5;
    public int MaxStage { get { return maxStage; } }

    int currCamIndex = 0;

    private void Start()
    {
        StartChapter();
    }

    void StartChapter()
    {
        //ĳ���� ���� 
        GameManager.Instance.SpawnPlayer(startPoints[0].transform.position, startPoints[0].transform.rotation);
        
        GameManager.Instance.cameraManager.InitCam();
        GameManager.Instance.cameraManager.ResetCam(virtualCams[0]);
        //�� ũ�� ������Ʈ
        GameManager.Instance.MapSize = new Vector2(mapBorders[0].width / 2, mapBorders[0].height / 2);
        GameManager.Instance.MapCenter = mapBorders[0].transform.position;
        //ĳ���� ���� �ִϸ��̼�
        startPoints[0].SpawnPlayer();
        //�⺻ ī�޶� �̵� 
        ChangeCameraPriority(0);
        //���̺� Ȱ��ȭ
        waveManager.AddStagePlanets(mapBorders[0].transform);
        if(startWave)
            waveManager.WaveStart(0);
    }

    public void FinishStage()
    {
        //ī�޶� ���� 
        GameManager.Instance.playerManager.playerBehavior.DeactivatePlayer(false);
        //ĳ���� ���� �ִϸ��̼� ����
        startPoints[currStageIndex].OpenPortal(NextStage);
        //��� ������ ���� ���

        //���� �������� ����
        if(currStageIndex +1 < maxStage)
        {
            mapBorders[currStageIndex +1].gameObject.SetActive(true);
            MoveAStarNavMesh(mapBorders[currStageIndex+1].transform.position);
        }

    }
    //NextStage�� StartPoint �ִϸ��̼����� �ڵ����� Call
    void NextStage()
    {
        //ĳ���� ���� �ִϸ��̼� �Ϸ� ��
        currStageIndex++;
        //ī�޶� �ʱ�ȭ
        GameManager.Instance.cameraManager.ResetCam(virtualCams[currStageIndex]);
        //�� ũ�� ������Ʈ
        GameManager.Instance.MapSize = new Vector2(mapBorders[currStageIndex].width / 2, mapBorders[currStageIndex].height / 2);
        GameManager.Instance.MapCenter = mapBorders[currStageIndex].transform.position;
        
        //ī�޶� �̵�
        ChangeCameraPriority(currStageIndex);
        GameManager.Instance.player.transform.position = startPoints[currStageIndex].transform.position;
        GameManager.Instance.cameraManager.MoveCameraPos(startPoints[currStageIndex].transform.position);

        //ĳ���� ����(�̵�)
        startPoints[currStageIndex].SpawnPlayer();

        //���̺� Ȱ��ȭ
        waveManager.AddStagePlanets(mapBorders[currStageIndex].transform);
        waveManager.WaveStart(currStageIndex);

        //���� �� ����
        Invoke("HidePriorMap", 2.0f);

    }


    public bool IsNextStageAvailable()
    {
        if (currStageIndex + 1 < maxStage)
        {
            return true;
        }
        else return false;
    }

    void ChangeCameraPriority(int index)
    {
        virtualCams[currCamIndex].Priority = 10;
        currCamIndex = index;
        virtualCams[currCamIndex].Priority = 20;
    }

    void HidePriorMap()
    {
        mapBorders[currStageIndex -1].gameObject.SetActive(false);

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
