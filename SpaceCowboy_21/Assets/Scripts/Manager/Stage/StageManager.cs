using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{

    private static StageManager instance;
    public static StageManager Instance { get { return instance; }}


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
    int preStageIndex = -1;
    int currStageIndex = 0;
    public int CurrStageIndex {  get { return currStageIndex; } }
    [SerializeField] int maxStage = 5;
    public int MaxStage { get { return maxStage; } }

    public event System.Action StageClearEvent;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(StartChapter());
    }

    IEnumerator StartChapter()
    {
        //1. 카메라를 먼저 위치시킨다.
        GameManager.Instance.cameraManager.InitCam();
        GameManager.Instance.cameraManager.ChangeCamera(null, virtualCams[0]);

        yield return new WaitForSeconds(0.5f);
        //2. 화면 밖의 지점에서 캐릭터를 생성 
        Vector2 point = waveManager.GetPointFromOutsideScreen(flagPoints[0].startPoint.position - flagPoints[0].flagPoint.position);
        GameManager.Instance.SpawnPlayer(point, Quaternion.identity);

        //3. 캐릭터가 날아서 도착하게 한다. 
        GameManager.Instance.playerManager.SuperBoost(point, flagPoints[0].flagPoint, true, StartWave);

        //4. 맵 업데이트
        GameManager.Instance.MapSize = new Vector2(mapBorders[0].width / 2, mapBorders[0].height / 2);
        GameManager.Instance.MapCenter = mapBorders[0].transform.position;
        waveManager.AddStagePlanets(mapBorders[0].transform);

        //MapBorder 활성화
        mapBorders[0].ActivateBorder = true;

    }

    /// <summary>
    /// 스테이지가 끝나면 실행.
    /// </summary>
    public void FinishStage()
    {
        //1. 캐릭터 조작 정지
        GameManager.Instance.playerManager.playerBehavior.DeactivatePlayer(false);
        
        //2. 카메라 추적 정지.
        GameManager.Instance.cameraManager.StopCameraFollow();

        //3. 현재 스테이지 MapBorder 비활성화
        mapBorders[currStageIndex].ActivateBorder = false;

        //다음 스테이지 생성
        if (currStageIndex +1 < maxStage)
        {
            mapBorders[currStageIndex + 1].gameObject.SetActive(true);

            MoveAStarNavMesh(mapBorders[currStageIndex+1].transform.position);
        }

        //4. 캐릭터가 화면 밖으로 날아가는 애니메이션 시작.
        Vector2 point = waveManager.GetPointFromOutsideScreen(flagPoints[currStageIndex].endPoint.position - flagPoints[currStageIndex].flagPoint.position);
        GameManager.Instance.playerManager.SuperBoost(point, GameManager.Instance.player, false, MoveStartToNextStage);

    }


    /// <summary>
    /// Del 로 SuperBoost가 끝나면 실행될 이벤트.
    /// </summary>
    void StartWave()
    {
        GameManager.Instance.cameraManager.FollowCamera();
        //MapBorder 활성화
        mapBorders[currStageIndex].ActivateBorder = true;

        if (startWave) waveManager.WaveStart(CurrStageIndex);

        //이전 웨이브 오브젝트 및 레벨 제거
        if(preStageIndex >= 0)
            mapBorders[preStageIndex].gameObject.SetActive(false);

        if (CurrStageIndex > 0 && StageClearEvent != null) StageClearEvent();
    }

    /// <summary>
    /// Del 로 FinishStage에서 실행된 SuperBoost가 화면 밖으로 나가면 실행.
    /// </summary>
    void MoveStartToNextStage()
    {
        preStageIndex = currStageIndex;

        //1. 스테이지 인덱스 상승
        currStageIndex++;

        //2. 카메라 초기화.
        GameManager.Instance.cameraManager.ChangeCamera(virtualCams[preStageIndex], virtualCams[currStageIndex]);

        //3.맵 크기 업데이트
        GameManager.Instance.MapSize = new Vector2(mapBorders[currStageIndex].width / 2, mapBorders[currStageIndex].height / 2);
        GameManager.Instance.MapCenter = mapBorders[currStageIndex].transform.position;

        //4. 다음 맵 활성화
        //mapBorders[CurrStageIndex].ActivateBorder = true;



        StartCoroutine(MoveComplete());
    }

    IEnumerator MoveComplete()
    {
        yield return new WaitForSeconds(2.0f);

        //캐릭터 생성(이동)
        Vector2 point = waveManager.GetPointFromOutsideScreen(flagPoints[currStageIndex].startPoint.position - flagPoints[currStageIndex].flagPoint.position);
        GameManager.Instance.playerManager.SuperBoost(point, flagPoints[currStageIndex].flagPoint, true, StartWave);

        //행성 활성화 
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