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
        //캐릭터 생성 
        GameManager.Instance.SpawnPlayer(startPoints[0].transform.position, startPoints[0].transform.rotation);
        
        GameManager.Instance.cameraManager.InitCam();
        GameManager.Instance.cameraManager.ResetCam(virtualCams[0]);
        //맵 크기 업데이트
        GameManager.Instance.MapSize = new Vector2(mapBorders[0].width / 2, mapBorders[0].height / 2);
        GameManager.Instance.MapCenter = mapBorders[0].transform.position;
        //캐릭터 생성 애니메이션
        startPoints[0].SpawnPlayer();
        //기본 카메라 이동 
        ChangeCameraPriority(0);
        //웨이브 활성화
        waveManager.AddStagePlanets(mapBorders[0].transform);
        if(startWave)
            waveManager.WaveStart(0);
    }

    public void FinishStage()
    {
        //카메라 정지 
        GameManager.Instance.playerManager.playerBehavior.DeactivatePlayer(false);
        //캐릭터 제거 애니메이션 시작
        startPoints[currStageIndex].OpenPortal(NextStage);
        //드롭 아이템 전부 흡수

        //다음 스테이지 생성
        if(currStageIndex +1 < maxStage)
        {
            mapBorders[currStageIndex +1].gameObject.SetActive(true);
            MoveAStarNavMesh(mapBorders[currStageIndex+1].transform.position);
        }

    }
    //NextStage는 StartPoint 애니메이션으로 자동으로 Call
    void NextStage()
    {
        //캐릭터 제거 애니메이션 완료 후
        currStageIndex++;
        //카메라 초기화
        GameManager.Instance.cameraManager.ResetCam(virtualCams[currStageIndex]);
        //맵 크기 업데이트
        GameManager.Instance.MapSize = new Vector2(mapBorders[currStageIndex].width / 2, mapBorders[currStageIndex].height / 2);
        GameManager.Instance.MapCenter = mapBorders[currStageIndex].transform.position;
        
        //카메라 이동
        ChangeCameraPriority(currStageIndex);
        GameManager.Instance.player.transform.position = startPoints[currStageIndex].transform.position;
        GameManager.Instance.cameraManager.MoveCameraPos(startPoints[currStageIndex].transform.position);

        //캐릭터 생성(이동)
        startPoints[currStageIndex].SpawnPlayer();

        //웨이브 활성화
        waveManager.AddStagePlanets(mapBorders[currStageIndex].transform);
        waveManager.WaveStart(currStageIndex);

        //지난 맵 제거
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
