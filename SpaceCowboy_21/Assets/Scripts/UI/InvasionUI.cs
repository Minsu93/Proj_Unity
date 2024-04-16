using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvasionUI : MonoBehaviour
{
    public TextMeshProUGUI invasionPercentageText;
    public Image invasionImg;
    float maxGauge;

    public void UpdateInvasionUI(float maxGauge)
    {
        this.maxGauge = maxGauge;
    }
    private void LateUpdate()
    {
        float curGauge = WaveManager.instance.gameTime;
        float percent = curGauge / maxGauge;
        invasionImg.fillAmount = percent;
        invasionPercentageText.text = Mathf.FloorToInt(percent * 100).ToString() + "%";
    }
}
