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

    ///���� �Ŵ������� �������� Ŭ���� �ߴٰ� �����Ѵ�. 
    ///���� ���۵Ǹ� ���� Ŭ���� ���¸� �ε��Ѵ�. 
    ///��� Ŭ������ ���� ���� ī�޶� �̵��Ѵ�(ó�� ���̰� UI�� �̵��Ѵ�)
    ///Ŭ���ϸ� �ش� ���� �ε��Ѵ�. 

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