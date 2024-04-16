using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionUI : MonoBehaviour
{
    public TextMeshProUGUI missionText;
    public Image gaugeImage;

    ///
    /// 전체 진척도를 보여주는 UI. 
    /// MissionManager에서 이 UI의 정보를 업데이트 하도록 한다. 
    /// 미션이 달성 될 때마다, UI 진척도는 업데이트 된다. 
    ///

    public void UpdateMissionUI(int totalMissionCount, int completedMissionCount)
    {
        float percent = (float)totalMissionCount / completedMissionCount;
        gaugeImage.fillAmount = percent;
        missionText.text = Mathf.FloorToInt(percent * 100).ToString() + "%";
    }

}
