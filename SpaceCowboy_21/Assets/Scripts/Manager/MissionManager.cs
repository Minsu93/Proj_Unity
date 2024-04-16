using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    /// <summary>
    /// 현재 스테이지에 있는 미션 관련.
    /// 1. Total Objective : 미니맵에 표시될 목표가 되는 행성(or 발전기) 표시를 위해
    /// 2. 전체 미션의 수, 달성한 미션의 수 : 진행도 표시를 위해 
    /// 3. 남아있는 상자의 수 : 히든 미션 정보를 위해(보류)
    /// </summary>
    /// 
    public static MissionManager instance;
    public GameObject[] totalObjectives;
    public List<GameObject> unCompletedObjectives = new List<GameObject>();
    public List<GameObject> completedObjectives = new List<GameObject>();

    public MissionUI missionUI;
    public MinimapIndicator minimapIndicator;

    private void Awake()
    {
        instance = this; 
    }

    private void Start()
    {
        foreach (GameObject obj in totalObjectives) { unCompletedObjectives.Add(obj); }
    }

    //스테이지 시작 시 실행
    public void UpdateTotalObjectives(GameObject[] objectives)
    {
        //totalObjectives = new GameObject[objectives.Length];
        totalObjectives = objectives;

        completedObjectives.Clear() ;
        unCompletedObjectives.Clear() ;
        foreach (GameObject obj in totalObjectives) { unCompletedObjectives.Add(obj); }

        missionUI.UpdateMissionUI(totalObjectives.Length, 0);
        minimapIndicator.SetMissionObjectives(totalObjectives);
    }

    //오브젝트 완료 시 실행 
    public void CompleteMissionObjectives(GameObject completedObj)
    {
        //오브젝트 완료 표시 
        unCompletedObjectives.Remove(completedObj);
        completedObjectives.Add(completedObj);
        missionUI.UpdateMissionUI(totalObjectives.Length, completedObjectives.Count);
        //오브젝트를 모두 완료했으면 GameManager에서 Win
    }
}
