using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ȭ�� �����ڸ� ���� ȭ��ǥ ǥ�� ���. ������ ǥ�� ���.
/// </summary>
/// 
public class ArrowIndicatorManager : MonoBehaviour
{
    List<GameObject> monsterList = new List<GameObject>();
    List<GameObject> arrowList = new List<GameObject>();
    Dictionary<GameObject, GameObject> arrowDictionaries = new Dictionary<GameObject, GameObject>();

    RectTransform rectTransform;
    Vector2 centerScreenPos;    //rectTr ȭ���߾� ��ġ
    float aspect;               //ȭ�� ����
    [SerializeField] GameObject arrowPrefab;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        centerScreenPos = new Vector2(rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.y / 2);
        aspect = rectTransform.sizeDelta.y / rectTransform.sizeDelta.x;
    }


    private void LateUpdate()
    {
        //���� ����Ʈ���� �� ���͵��� ȭ�� ���ο� �ִ��� �ܺο� �ִ��� Ȯ���Ѵ�. 
        //Ȯ���� �� �ܺο� �ִ� ���͵��� arrowDictionaries�� ����ְ� arrow�� �׸��� �����Ѵ�. 

        for(int i = 0; i < monsterList.Count; i++)
        {
            GameObject obj = monsterList[i];
            Vector2 targetScreenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
            if (IsInsideCameraScreen(targetScreenPos, rectTransform.sizeDelta))
            {
                //ȭ�� ���̸� arrowDictionaries���� �����Ѵ�.
                HideArrow(obj);
            }
            else
            {
                //ȭ�� ���̸� arrowDictionaries�� �߰��Ѵ�. 
                ShowArrow(obj);
            }
        }

        //dictionary�� �ִ� ��� ���͵��� arrow ��ġ�� �����Ѵ�. 
        foreach(KeyValuePair<GameObject, GameObject> item in arrowDictionaries)
        {
            DrawArrow(item.Key, item.Value);
        }
    }

    //���� �߰�
    public void AddMonster(GameObject monsterObj)
    {
        monsterList.Add(monsterObj);
    }
    //���� ����
    public void RemoveMonster(GameObject monsterObj)
    {
        //�ش� ���Ͱ� ���� ǥ�õǰ� �ִ� (arrowDictionaries�� ���Ե�) ������ ���
        if (arrowDictionaries.ContainsKey(monsterObj))
        {
            //1. arrow�� disable�Ѵ�
            GameObject arrow = arrowDictionaries[monsterObj];
            //2. �ش� dictionary���� �����Ѵ� 
            arrowDictionaries.Remove(monsterObj);
        }
        //3. ���� ����Ʈ���� �����Ѵ�. 
        monsterList.Remove(monsterObj);
    }

    //ȭ��ǥ ���̱�
    void ShowArrow(GameObject obj)
    {
        //arrowDictionaries�� ��ϵǾ� ���� ������
        if (!arrowDictionaries.ContainsKey(obj))
        {
            //1. arrowList���� ��Ȱ��ȭ�� arrow�� �����´�. 
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
            //2.��Ȱ��ȭ�� arrow�� ���ٸ� arrow�� �߰��Ͽ� arrowList�� �ִ´�. 
            if (arrow == null)
            {
                arrow = Instantiate(arrowPrefab, this.transform);
                arrowList.Add(arrow);
            }
            //2. dictionary�� �ִ´�. 
            arrowDictionaries.Add(obj, arrow);
        }
       
    }
    //ȭ��ǥ �����
    void HideArrow(GameObject obj)
    {
        //arrowDictionaries�� ��ϵǾ� ������
        if (arrowDictionaries.ContainsKey(obj))
        {
            //1. arrow�� ��Ȱ��ȭ �Ѵ�
            arrowDictionaries[obj].SetActive(false);
            //2. dictionary���� �����Ѵ�
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

        //��￡ ���� ȭ�� �����ڸ� ��� ��ġ���� ����.
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

        // ȭ��ǥ ���� ����
        float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
        arrowRect.rotation = Quaternion.Euler(0, 0, angle);
    }
}
