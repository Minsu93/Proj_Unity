using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIndicator : MonoBehaviour
{
    bool arrowOn;
    float minimapAspect;

    public Camera minimapCam;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] int arrowNumber = 5;
    List<RectTransform> arrowList = new List<RectTransform>();

    RectTransform minimapRect; // 미니맵 RectTransform
    List<GameObject> objectiveList = new List<GameObject>();   //미니맵에 표시될 타겟 objectives
    Vector2 centerScreenPos;

    private void Awake()
    {
        minimapRect = GetComponent<RectTransform>();
        centerScreenPos = new Vector2(minimapRect.sizeDelta.x / 2, minimapRect.sizeDelta.y / 2);

        // 화살표가 미니맵 가장자리에 위치하도록 설정
        minimapAspect = minimapRect.sizeDelta.y / minimapRect.sizeDelta.x;

        // 화살표를 원하는 개수만큼 생성
        AddArrows(arrowNumber);
    }

    //public void SetMissionObjectives(GameObject[] objs)
    //{

    //    objectives = objs;
        
    //    for(int i = 0; i < arrow.Length; i++)
    //    {
    //        if(i < objectives.Length) arrow[i].gameObject.SetActive(true);
    //        else arrow[i].gameObject.SetActive(false);
    //    }
    //}

    void AddArrows(int number)
    {
        for(int i  = 0; i < arrowNumber; i++)
        {
            GameObject arrow = Instantiate(arrowPrefab, this.transform);
            arrowList.Add(arrow.GetComponent<RectTransform>());
            arrow.SetActive(false);

        }
    }

    public void AddMissionObjectives(GameObject obj)
    {
        objectiveList.Add(obj);
        if (objectiveList.Count > 0) arrowOn = true;
    }
    public void RemoveMissionObjectives(GameObject obj)
    {
        objectiveList.Remove(obj);
        if(objectiveList.Count == 0) arrowOn = false;
    }

    void LateUpdate()
    {
        if (!arrowOn) return;

        int arrowIndex = 0;
        foreach(GameObject target in objectiveList)
        {
            //objectives의 수가 arrow보다 많으면 그만둔다.
            if (arrowIndex > arrowList.Count) break;

            //타겟 위치가 화면 내부라면 화살표 사라짐 
            Vector2 targetPos = target.transform.position;
            Vector2 targetScreenPos = minimapCam.WorldToScreenPoint(targetPos);
            if(IsInsideCameraScreen(targetScreenPos, minimapRect.sizeDelta))
            {
                arrowList[arrowIndex].gameObject.SetActive(false);
                arrowIndex++;
                break;
            }
            else
            {
                arrowList[arrowIndex].gameObject.SetActive(true);
            }

            Vector2 fromPlayerToTarget = new Vector2(targetScreenPos.x - centerScreenPos.x, targetScreenPos.y - centerScreenPos.y);
            Vector2 normalizedDirection = fromPlayerToTarget.normalized;

            //기울기
            float slope = (normalizedDirection.y / normalizedDirection.x);

            Vector2 edgePoint = Vector2.zero;
            if (Mathf.Abs(slope) < minimapAspect)
            {
                edgePoint.x = Mathf.Sign(normalizedDirection.x) * minimapRect.sizeDelta.x / 2;
                edgePoint.y = edgePoint.x * slope;
            }
            else
            {
                edgePoint.y = Mathf.Sign(normalizedDirection.y) * minimapRect.sizeDelta.y / 2;
                edgePoint.x = edgePoint.y / slope;
            }

            arrowList[arrowIndex].anchoredPosition = edgePoint;

            // 화살표 방향 설정
            float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
            arrowList[arrowIndex].rotation = Quaternion.Euler(0, 0, angle );
            arrowIndex++;
        }
    }

    bool IsInsideCameraScreen(Vector2 screenPos, Vector2 rectSizeDelta)
    {
        bool x = screenPos.x > 0 && screenPos.x < rectSizeDelta.x;
        bool y = screenPos.y > 0 && screenPos.y < rectSizeDelta.y;
        return x && y;
    }
}

