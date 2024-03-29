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
    float maxGauge;
    int curWave;


    private void LateUpdate()
    {
        curGauge = WaveManager.instance.gameTime;
        maxGauge = WaveManager.instance.waveTime;

        waveImg.fillAmount = curGauge / maxGauge;

        int waveIndex = WaveManager.instance.waveIndex;
        if(waveIndex != curWave)
        {
            curWave = waveIndex;
            waveIndexText.text = waveIndex.ToString();
        }
    }
}
