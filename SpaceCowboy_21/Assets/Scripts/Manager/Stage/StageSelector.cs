using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelector : MonoBehaviour
{
    //private void Start()
    //{
    //    StageTotalClearState clearState = new StageTotalClearState();
    //    clearState.lastClearStageIndex = -1;
    //    StageState newStageState = new StageState();
    //    newStageState.stageIndex = 0;
    //    newStageState.isCleared = false;
    //    clearState.stageStates.Add(newStageState);
    //    LoadManager.Save<StageTotalClearState>(clearState, "StageData.json");
    //}

    ///게임 매니저에서 스테이지 클리어 했다고 전달한다. 
    ///씬이 시작되면 현재 클리어 상태를 로드한다. 
    ///방금 클리어한 씬에 따라 카메라를 이동한다(처럼 보이게 UI를 이동한다)
    ///클릭하면 해당 맵을 로드한다. 

    public void SelectStage(string name)
    {
        GameManager.Instance.LoadsceneByName(name);
    }
}

[Serializable]
public class StageTotalClearState
{
    public int lastClearStageIndex;
    public List<StageState> stageStates = new List<StageState>();
}

[Serializable]
public class StageState
{
    public int stageIndex;
    public bool isCleared;
}