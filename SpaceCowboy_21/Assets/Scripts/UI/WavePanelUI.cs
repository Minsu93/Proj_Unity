using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePanelUI : MonoBehaviour
{
    //int stageCount;
    [SerializeField] float playerIconYUp = 50f;

    [SerializeField] GameObject stageicon;
    [SerializeField] GameObject bossicon;
    [SerializeField] Transform iconStorage;
    [SerializeField] RectTransform playerIconTr;
    [SerializeField] RectTransform lineRect;

    List<RectTransform> iconList = new List<RectTransform>();

    //스테이지 수에 따라 UI 배치하기
    public void SetStageIcons(int stageCount)
    {
        //this.stageCount = stageCount;
        //stageicon
        for(int i  = 0; i < stageCount; i++)
        {
            GameObject prefab;
            if (i == stageCount - 1)
            {
                prefab = bossicon;
            }
            else
            {
                prefab = stageicon;
            }
            GameObject obj = Instantiate(prefab, iconStorage);
            iconList.Add(obj.GetComponent<RectTransform>());

        }

        Canvas.ForceUpdateCanvases();

        //line 길이 조절
        float width = iconList[0].localPosition.x - iconList[stageCount -1].localPosition.x;
        width = Mathf.Abs(width);

        Vector2 size = lineRect.sizeDelta;
        size.x = width;
        lineRect.sizeDelta = size;

    }

    //스테이지 이동시 플레이어 아이콘 이동
    public void MovePlayericon(int stageIndex)
    {
        playerIconTr.anchoredPosition = iconList[stageIndex].localPosition + (Vector3.up * playerIconYUp);
        //Debug.Log(iconList[stageIndex].localPosition);
        //Debug.Log(iconList[stageIndex].anchoredPosition);


    }
}
