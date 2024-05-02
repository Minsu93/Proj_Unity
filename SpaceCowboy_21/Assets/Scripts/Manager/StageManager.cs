using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    ///
    /// 스테이지에 배치. 
    /// 게임이 시작되면 MissionManager와 WaveManager를 업데이트 한다. 
    ///

    public GameObject[] stageObjectives;
    public int invasionMinute = 1;
    public AnimationCurve invasionTimeCurve;

    private void Start()
    {
        UpdateManagers();
        StageStart();
    }

    void UpdateManagers()
    {
        //mission 업데이트
        MissionManager.instance.UpdateTotalObjectives(stageObjectives);

        //wave 업데이트. 웨이브 시작. 
        //WaveManager.instance.UpdateWaveManager(invasionMinute, invasionTimeCurve, enemyTeams);

    }

    void StageStart()
    {
        WaveManager.instance.WaveStart(true);
    }
}
