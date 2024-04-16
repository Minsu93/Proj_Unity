using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIndicator : MonoBehaviour
{
    bool arrowOn;
    float minimapAspect;

    public Camera minimapCam;
    public RectTransform[] arrow; // UI 화살표, 최대 10개이며 

    RectTransform minimapRect; // 미니맵 RectTransform
    GameObject[] objectives;    //미니맵에 표시될 타겟 objectives
    Vector2 centerScreenPos;

    private void Awake()
    {
        minimapRect = GetComponent<RectTransform>();
        centerScreenPos = new Vector2(minimapRect.sizeDelta.x / 2, minimapRect.sizeDelta.y / 2);
        // 화살표가 미니맵 가장자리에 위치하도록 설정
        minimapAspect = minimapRect.sizeDelta.y / minimapRect.sizeDelta.x;
    }

    public void SetMissionObjectives(GameObject[] objs)
    {
        if (objs.Length > 0) arrowOn = true;

        objectives = objs;
        
        for(int i = 0; i < arrow.Length; i++)
        {
            if(i < objectives.Length) arrow[i].gameObject.SetActive(true);
            else arrow[i].gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (MissionManager.instance == null) return;
        if (!arrowOn) return;

        int arrowIndex = 0;
        foreach(GameObject target in objectives)
        {
            //objectives의 수가 arrow보다 많으면 그만둔다.
            if (arrowIndex > arrow.Length) break;

            //타겟 위치가 화면 내부라면 화살표 사라짐 
            Vector2 targetPos = target.transform.position;
            Vector2 targetScreenPos = minimapCam.WorldToScreenPoint(targetPos);
            if(IsInsideCameraScreen(targetScreenPos, minimapRect.sizeDelta))
            {
                arrow[arrowIndex].gameObject.SetActive(false);
                arrowIndex++;
                break;
            }
            else
            {
                arrow[arrowIndex].gameObject.SetActive(true);
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

            arrow[arrowIndex].anchoredPosition = edgePoint;

            // 화살표 방향 설정
            float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
            arrow[arrowIndex].rotation = Quaternion.Euler(0, 0, angle );
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

