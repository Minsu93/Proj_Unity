using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    ///
    /// ���������� ��ġ. 
    /// ������ ���۵Ǹ� MissionManager�� WaveManager�� ������Ʈ �Ѵ�. 
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
        //mission ������Ʈ
        MissionManager.instance.UpdateTotalObjectives(stageObjectives);

        //wave ������Ʈ. ���̺� ����. 
        //WaveManager.instance.UpdateWaveManager(invasionMinute, invasionTimeCurve, enemyTeams);

    }

    void StageStart()
    {
        WaveManager.instance.WaveStart(true);
    }
}
