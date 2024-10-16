using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LevelPanel : MonoBehaviour
{
    public TextMeshProUGUI[] texts;
    public Image[] gauges;
    public Animator[] animators;
    public void SetLevelPanel(int index, int level, float curExp, float maxExp, bool isGain)
    {
        texts[index].text = level.ToString();
        gauges[index].fillAmount = curExp / maxExp;
        if(isGain)
        {
            animators[index].SetTrigger("Gain");
        }
        else
        {
            animators[index].SetTrigger("Loss");
        }
    }
}
