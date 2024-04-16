using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    public TextMeshProUGUI waveIndexText;
    public Image waveImg;
    float curGauge;
    float preGauge;
    float maxGauge;


    private void LateUpdate()
    {
        curGauge = WaveManager.instance.gameTime;

        waveImg.fillAmount = (curGauge - preGauge) / (maxGauge - preGauge);
    }

    public void UpdateProgressBar(float waveTime, float gameTime, int waveLevel)
    {
        maxGauge = waveTime;
        preGauge = gameTime;
        waveIndexText.text = waveLevel.ToString();
    }
}
