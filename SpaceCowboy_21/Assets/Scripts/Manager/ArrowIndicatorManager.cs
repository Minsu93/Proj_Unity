using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 화면 가장자리 몬스터 화살표 표시 담당. 목적지 표시 담당.
/// </summary>
/// 
public class ArrowIndicatorManager : MonoBehaviour
{
    List<GameObject> monsterList = new List<GameObject>();
    List<GameObject> arrowList = new List<GameObject>();
    Dictionary<GameObject, GameObject> arrowDictionaries = new Dictionary<GameObject, GameObject>();

    RectTransform rectTransform;
    Vector2 centerScreenPos;    //rectTr 화면중앙 위치
    float aspect;               //화면 비율
    [SerializeField] GameObject arrowPrefab;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        centerScreenPos = new Vector2(rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.y / 2);
        aspect = rectTransform.sizeDelta.y / rectTransform.sizeDelta.x;
    }


    private void LateUpdate()
    {
        //몬스터 리스트에서 각 몬스터들이 화면 내부에 있는지 외부에 있는지 확인한다. 
        //확인한 후 외부에 있는 몬스터들은 arrowDictionaries에 집어넣고 arrow를 그리기 시작한다. 

        for(int i = 0; i < monsterList.Count; i++)
        {
            GameObject obj = monsterList[i];
            Vector2 targetScreenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
            if (IsInsideCameraScreen(targetScreenPos, rectTransform.sizeDelta))
            {
                //화면 안이면 arrowDictionaries에서 제거한다.
                HideArrow(obj);
            }
            else
            {
                //화면 밖이면 arrowDictionaries에 추가한다. 
                ShowArrow(obj);
            }
        }

        //dictionary에 있는 모든 몬스터들의 arrow 위치를 조절한다. 
        foreach(KeyValuePair<GameObject, GameObject> item in arrowDictionaries)
        {
            DrawArrow(item.Key, item.Value);
        }
    }

    //몬스터 추가
    public void AddMonster(GameObject monsterObj)
    {
        monsterList.Add(monsterObj);
    }
    //몬스터 제거
    public void RemoveMonster(GameObject monsterObj)
    {
        //해당 몬스터가 현재 표시되고 있는 (arrowDictionaries에 포함된) 몬스터인 경우
        if (arrowDictionaries.ContainsKey(monsterObj))
        {
            //1. arrow를 disable한다
            GameObject arrow = arrowDictionaries[monsterObj];
            //2. 해당 dictionary에서 제거한다 
            arrowDictionaries.Remove(monsterObj);
        }
        //3. 몬스터 리스트에서 제거한다. 
        monsterList.Remove(monsterObj);
    }

    //화살표 보이기
    void ShowArrow(GameObject obj)
    {
        //arrowDictionaries에 등록되어 있지 않으면
        if (!arrowDictionaries.ContainsKey(obj))
        {
            //1. arrowList에서 비활성화된 arrow를 가져온다. 
            GameObject arrow = null;
            for (int j = 0; j < arrowList.Count; j++)
            {
                if (!arrowList[j].activeSelf)
                {
                    arrow = arrowList[j];
                    arrow.SetActive(true);
                    break;
                }
            }
            //2.비활성화된 arrow가 없다면 arrow를 추가하여 arrowList에 넣는다. 
            if (arrow == null)
            {
                arrow = Instantiate(arrowPrefab, this.transform);
                arrowList.Add(arrow);
            }
            //2. dictionary에 넣는다. 
            arrowDictionaries.Add(obj, arrow);
        }
       
    }
    //화살표 숨기기
    void HideArrow(GameObject obj)
    {
        //arrowDictionaries에 등록되어 있으면
        if (arrowDictionaries.ContainsKey(obj))
        {
            //1. arrow를 비활성화 한다
            arrowDictionaries[obj].SetActive(false);
            //2. dictionary에서 제거한다
            arrowDictionaries.Remove(obj);
        }
    }

    bool IsInsideCameraScreen(Vector2 screenPos, Vector2 rectSizeDelta)
    {
        bool x = screenPos.x > 0 && screenPos.x < rectSizeDelta.x;
        bool y = screenPos.y > 0 && screenPos.y < rectSizeDelta.y;
        return x && y;
    }

    void DrawArrow(GameObject monster, GameObject arrow)
    {
        Vector2 targetScreenPos = Camera.main.WorldToScreenPoint(monster.transform.position);
        Vector2 fromPlayerToTarget = new Vector2(targetScreenPos.x - centerScreenPos.x, targetScreenPos.y - centerScreenPos.y);
        Vector2 normalizedDirection = fromPlayerToTarget.normalized;

        //기울에 따라 화면 가장자리 어디에 위치할지 지정.
        float slope = (normalizedDirection.y / normalizedDirection.x);

        Vector2 edgePoint = Vector2.zero;
        if (Mathf.Abs(slope) < aspect)
        {
            edgePoint.x = Mathf.Sign(normalizedDirection.x) * rectTransform.sizeDelta.x / 2;
            edgePoint.y = edgePoint.x * slope;
        }
        else
        {
            edgePoint.y = Mathf.Sign(normalizedDirection.y) * rectTransform.sizeDelta.y / 2;
            edgePoint.x = edgePoint.y / slope;
        }

        RectTransform arrowRect = arrow.GetComponent<RectTransform>();
        arrowRect.anchoredPosition = edgePoint;

        // 화살표 방향 설정
        float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
        arrowRect.rotation = Quaternion.Euler(0, 0, angle);
    }
}
