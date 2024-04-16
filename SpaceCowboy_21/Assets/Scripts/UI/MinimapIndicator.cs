using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIndicator : MonoBehaviour
{
    bool arrowOn;
    float minimapAspect;

    public Camera minimapCam;
    public RectTransform[] arrow; // UI ȭ��ǥ, �ִ� 10���̸� 

    RectTransform minimapRect; // �̴ϸ� RectTransform
    GameObject[] objectives;    //�̴ϸʿ� ǥ�õ� Ÿ�� objectives
    Vector2 centerScreenPos;

    private void Awake()
    {
        minimapRect = GetComponent<RectTransform>();
        centerScreenPos = new Vector2(minimapRect.sizeDelta.x / 2, minimapRect.sizeDelta.y / 2);
        // ȭ��ǥ�� �̴ϸ� �����ڸ��� ��ġ�ϵ��� ����
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
            //objectives�� ���� arrow���� ������ �׸��д�.
            if (arrowIndex > arrow.Length) break;

            //Ÿ�� ��ġ�� ȭ�� ���ζ�� ȭ��ǥ ����� 
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

            //����
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

            // ȭ��ǥ ���� ����
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

