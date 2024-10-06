using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class ArrowManager : MonoBehaviour
{
    [SerializeField] GameObject arrowCanvas;
    [SerializeField] GameObject[] arrowPrefab;

    Stack<GameObject>[] sleepArrowStacks;
    Dictionary<GameObject, GameObject> objArrowPair = new Dictionary<GameObject, GameObject>();
    
    float aspect;               //화면 비율
    Vector2 rectSizeDelta;  //awake에서 화면 sizeDelta넣기.
    Vector2 centerScreenPos;    //rectTr 화면중앙 위치
    GameObject canvas;


    private void Awake()
    {
        sleepArrowStacks = new Stack<GameObject>[arrowPrefab.Length];
        for(int i = 0; i< sleepArrowStacks.Length; i++)
        {
            sleepArrowStacks[i] = new Stack<GameObject>();
        }

        canvas =  Instantiate(arrowCanvas, this.transform);

        rectSizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
        centerScreenPos = new Vector2(rectSizeDelta.x / 2, rectSizeDelta.y / 2);
        aspect = rectSizeDelta.y / rectSizeDelta.x;
    }

    private void LateUpdate()
    {
        /// Dictionary에 있는 오브젝트들을 그린다.
        /// 
        if(objArrowPair.Count > 0)
        {
            foreach(KeyValuePair<GameObject, GameObject> pair in objArrowPair) 
            {
                GameObject obj = pair.Key;
                GameObject arrow = pair.Value;

                Vector2 targetScreenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
                if (IsInsideCameraScreen(targetScreenPos))
                {
                    //화면 안
                    arrow.SetActive(false);
                }
                else
                {
                    //화면 밖
                    arrow.SetActive(true);
                }

                if (arrow.activeSelf)
                {
                    DrawArrow(arrow, targetScreenPos);
                }
            }

        }
    }

    public void CreateArrow(GameObject obj, int index)
    {
        ///오브젝트를 dictionary에 추가하려고 한다. 
        ///index에 맞는(최대 = arrowPrefab.count) 쉬고있는 arrow를 가져온다. 
        ///쉬고있는 arrow가 없으면 새로운 arrow를 생성해서 가져온다
        ///추가를 완료한다. 
        ///
        if(!objArrowPair.ContainsKey(obj))
            objArrowPair.Add(obj, GetSleepArrow(index));
        else
        {
            Debug.LogWarning("ArrowManager : Already contained Object");
        }
    }

    public void RemoveArrow(GameObject obj, int index)
    {
        ///오브젝트를 dictionary에서 찾는다.
        ///arrow를 다시 쉬는곳으로 보낸다. 
        ///오브젝트를 dictionary에서 제거한다. 
        ///
        if(objArrowPair.ContainsKey(obj))
        {
            objArrowPair.TryGetValue(obj, out GameObject arrow);

            index = Mathf.Clamp(index, 0, arrowPrefab.Length);
            sleepArrowStacks[index].Push(arrow);
            arrow.SetActive(false);

            objArrowPair.Remove(obj);
        }
    }

    GameObject GetSleepArrow(int index)
    {
        index = Mathf.Clamp(index,0, arrowPrefab.Length);

        if (sleepArrowStacks[index].Count > 0)
        {
            //스택에 쉬는게 있을 때
            GameObject obj = sleepArrowStacks[index].Pop();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            //스택에 하나도 없을 때
            GameObject obj = Instantiate(arrowPrefab[index], canvas.transform);
            return obj;
        }
    }

    public void ResetArrows()
    {
        for(int i = 0; i < arrowPrefab.Length; i++)
        {
            sleepArrowStacks[i].Clear();
        }
        objArrowPair.Clear();
    }


    #region 화면에 배치하는 로직



    bool IsInsideCameraScreen(Vector2 screenPos)
    {

        bool x = screenPos.x > 0 && screenPos.x < rectSizeDelta.x;
        bool y = screenPos.y > 0 && screenPos.y < rectSizeDelta.y;
        return x && y;
    }
    void DrawArrow(GameObject arrow, Vector2 screenPos)
    {
        Vector2 fromCenterToTarget = new Vector2(screenPos.x - centerScreenPos.x, screenPos.y - centerScreenPos.y);
        Vector2 normalizedDirection = fromCenterToTarget.normalized;

        //기울에 따라 화면 가장자리 어디에 위치할지 지정.
        float slope = (normalizedDirection.y / normalizedDirection.x);

        Vector2 edgePoint = Vector2.zero;
        if (Mathf.Abs(slope) < aspect)
        {
            edgePoint.x = Mathf.Sign(normalizedDirection.x) * rectSizeDelta.x / 2;
            edgePoint.y = edgePoint.x * slope;
        }
        else
        {
            edgePoint.y = Mathf.Sign(normalizedDirection.y) * rectSizeDelta.y / 2;
            edgePoint.x = edgePoint.y / slope;
        }

        RectTransform arrowRect = arrow.GetComponent<RectTransform>();
        arrowRect.anchoredPosition = edgePoint;

        // 화살표 방향 설정
        float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
        arrowRect.rotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion
}
