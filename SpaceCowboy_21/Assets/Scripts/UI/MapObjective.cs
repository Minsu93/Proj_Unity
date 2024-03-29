using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObjective : MonoBehaviour
{
    /// <summary>
    /// ������ ���� ���ֵ��� ������ �̺�Ʈ �߻�. 
    /// </summary>
    /// 

    [Tooltip("��ƾ� �ϴ� ��ǥ��, 3������")]
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
