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
    
    float aspect;               //ȭ�� ����
    Vector2 rectSizeDelta;  //awake���� ȭ�� sizeDelta�ֱ�.
    Vector2 centerScreenPos;    //rectTr ȭ���߾� ��ġ
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
        /// Dictionary�� �ִ� ������Ʈ���� �׸���.
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
                    //ȭ�� ��
                    arrow.SetActive(false);
                }
                else
                {
                    //ȭ�� ��
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
        ///������Ʈ�� dictionary�� �߰��Ϸ��� �Ѵ�. 
        ///index�� �´�(�ִ� = arrowPrefab.count) �����ִ� arrow�� �����´�. 
        ///�����ִ� arrow�� ������ ���ο� arrow�� �����ؼ� �����´�
        ///�߰��� �Ϸ��Ѵ�. 
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
        ///������Ʈ�� dictionary���� ã�´�.
        ///arrow�� �ٽ� ���°����� ������. 
        ///������Ʈ�� dictionary���� �����Ѵ�. 
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
            //���ÿ� ���°� ���� ��
            GameObject obj = sleepArrowStacks[index].Pop();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            //���ÿ� �ϳ��� ���� ��
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


    #region ȭ�鿡 ��ġ�ϴ� ����



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

        //��￡ ���� ȭ�� �����ڸ� ��� ��ġ���� ����.
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

        // ȭ��ǥ ���� ����
        float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
        arrowRect.rotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion
}
