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
    /// ��ü ��ô���� �����ִ� UI. 
    /// MissionManager���� �� UI�� ������ ������Ʈ �ϵ��� �Ѵ�. 
    /// �̼��� �޼� �� ������, UI ��ô���� ������Ʈ �ȴ�. 
    ///

    public void UpdateMissionUI(int totalMissionCount, int completedMissionCount)
    {
        float percent = (float)totalMissionCount / completedMissionCount;
        gaugeImage.fillAmount = percent;
        missionText.text = Mathf.FloorToInt(percent * 100).ToString() + "%";
    }

}
