using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObjective : MonoBehaviour
{
    /// <summary>
    /// 선택한 맵의 유닛들이 죽으면 이벤트 발생. 
    /// </summary>
    /// 

    [Tooltip("잡아야 하는 목표물, 3개까지")]
    public GameObject[] targets = new GameObject[3];
    public Image[] fillImages = new Image[3];
    bool[] getStars = new bool[3];

    private void Start()
    {
        foreach(var img in fillImages)
        {
            img.enabled = false;
        }
    }


    public void GetStar(GameObject target)
    {
        int order = 0;

        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] == target) order = i;
        }

        getStars[order] = true;
        fillImages[order].enabled = true;
    }
}
