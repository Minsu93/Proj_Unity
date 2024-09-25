using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopperManager : MonoBehaviour
{

    //발사 사이 간격
    [SerializeField] GameObject popperPrefab;
    [SerializeField] private float gapBetweenOrbs = 3.0f;
    //[SerializeField] private float launchHeight = 5.0f;
    //[SerializeField] private float launchRadius = 5.0f;
    [SerializeField] private float launchInterval = 0.3f;   //발사 사이 간격 

    //드롭 확률 조절 관련
    List<EquippedItem> equippedWeapons = new List<EquippedItem>(); // 드롭 가능한 아이템 목록
    List<EquippedItem> equippedDrones = new List<EquippedItem>();

    [SerializeField] float dropChanceIncrement = 0.1f; // 드롭 실패 시 확률 증가량
    [SerializeField] float dropChanceDecrement = 0.05f; // 드롭 성공 시 확률 감소량
    [SerializeField] float totalDropChance = 0.1f;  // Bubble 드롭 확률 
    float currentDropChance;
    [SerializeField] float dropCoolTime = 5f;   //드롭 된 이후 다음 드롭까지 제한 최소 시간
    bool dropReady = true;
    float timer = 0f;

    private void Update()
    {
        //드랍 쿨타임
        if (!dropReady)
        {
            timer += Time.deltaTime;
            if(timer > dropCoolTime)
            {
                dropReady = true;
                timer = 0;
            }
        }
    }

    #region 초기화

    public void PopperReady()
    {
        List<WeaponData> unlockedList = GameManager.Instance.weaponDictionary.unlockedDataList;
        for (int i = 0; i < unlockedList.Count; i++)
        {
            equippedWeapons.Add(new EquippedItem(unlockedList[i].name, 0.1f, unlockedList[i]));
        }

        List<GameObject> unlockedDroneList = GameManager.Instance.weaponDictionary.unlockedDroneList;
        for (int j = 0; j < unlockedDroneList.Count; j++)
        {
            equippedDrones.Add(new EquippedItem(unlockedDroneList[j].name, 0.1f, unlockedDroneList[j]));
        }

        ResizeDropChance(equippedWeapons);
        ResizeDropChance(equippedDrones);

        currentDropChance = totalDropChance;
    }

    #endregion

    #region popper 방식
    public void CreatePopper(Transform targetTr)
    {
        if (!dropReady) return;
        //createPopper 요청이 오면 totalDropChance 확률로 아이템을 스폰한다. 
        float random = UnityEngine.Random.Range(0, 1.0f);
        if (random < currentDropChance)
        {
            // 폭죽 프리팹을 랜덤한 위치에 생성합니다.
            GameObject firework = GameManager.Instance.poolManager.GetPoolObj(popperPrefab, 2);
            firework.transform.position = targetTr.position;
            firework.transform.rotation = targetTr.rotation;
            
            //Weapon 대 Dronem 의 비는 6:4 이다.
            float WeaponRatio = 0.6f;
            float randomF = UnityEngine.Random.Range(0, 1.0f);
            if(randomF < WeaponRatio)
            {
                WeaponData data = TryDropItem(equippedWeapons).item as WeaponData;
                if(data != null)
                {
                    Vector2 targetPoint = GetEmptySpace(targetTr);
                    StartCoroutine(firework.GetComponent<Popper>().CreateWeaponBubble(targetTr.position, targetPoint, data));
                }
                else Debug.LogWarning("WeaponData cast failed. The item is not of type WeaponData.");

            }
            else
            {
                GameObject prefab = TryDropItem(equippedDrones).item as GameObject;
                if(prefab != null)
                {
                    Vector2 targetPoint = GetEmptySpace(targetTr);
                    StartCoroutine(firework.GetComponent<Popper>().CreateDroneBubble(targetTr.position, targetPoint, prefab));
                }
                else Debug.LogWarning("DroneObject cast failed. The item is not of type DroneObject.");
            }

            currentDropChance = totalDropChance;
            dropReady = false;
        }
        else
        {
            //드랍이 안될 경우 1.1배씩 계속 증가.
            currentDropChance *= 1.1f;
        }
    }
    

    //타겟 윗부분 주변의 포인트 가져옴. 행성이 없는 장소. 
    Vector3 GetEmptySpace(Transform targetTr)
    {
        int maxAttempts = 100;
        Vector3 emptyPos = new Vector3();

        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = UnityEngine.Random.Range(0, Mathf.PI * 2); 
            float distance = UnityEngine.Random.Range(3f, 5f); 
            Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // 이 각도와 거리를 사용하여 원 내부의 랜덤한 포인트를 계산합니다.
            randomPoint += (Vector2)targetTr.position;

            Vector2 mapSize = GameManager.Instance.MapSize;
            //화면 밖이 아닌지 검사한다
            if(randomPoint.x < -mapSize.x || randomPoint.x > mapSize.x || randomPoint.y < -mapSize.y || randomPoint.y > mapSize.y)
            {
                continue;
            }

            //가장 가까운 (걸을 수 있는)노드를 가져온다
            var constraint = NNConstraint.None;

            // Constrain the search to walkable nodes only
            constraint.constrainWalkability = true;
            constraint.walkable = true;

            GraphNode node = AstarPath.active.GetNearest((Vector3)randomPoint, constraint).node;
            if(node != null)
            {
                emptyPos = (Vector3)node.position;
                Debug.Log("Try count : " + i.ToString() + "/ Pos is : " + emptyPos.ToString());
                break;
            }
        }

        return emptyPos;
    }

    #endregion

    //#region 플레이어 다이렉트 드랍 방식
    //public void GiveWeaponToPlayer(Transform fromTr)
    //{
    //    EquippedWeapon equip = ForceDropItem();
    //    GameObject firework = GameManager.Instance.poolManager.GetPoolObj(popperPrefab, 1);
    //    firework.transform.position = fromTr.position;
    //    firework.transform.rotation = fromTr.rotation;

    //    StartCoroutine(MoveToPlayerRoutine(firework, fromTr.position, equip.weaponData));

    //}

    //IEnumerator MoveToPlayerRoutine(GameObject firework, Vector2 startPos, WeaponData w_Data)
    //{
    //    float time = 0f;

    //    // 폭죽이 목표 위치에 도달할 때까지 이동합니다.
    //    while (time <= launchTimer)
    //    {
    //        Vector2 targetPos = GameManager.Instance.player.position;
    //        time += Time.deltaTime;

    //        Vector2 pos = Vector2.Lerp(startPos, targetPos, fireworkCurve.Evaluate(time / launchTimer));
    //        firework.transform.position = pos;
    //        yield return null;
    //    }

    //    // 오브 생성.
    //    GameObject newOrb = GameManager.Instance.poolManager.GetPoolObj(weaponBubble, 2);
    //    newOrb.transform.position = GameManager.Instance.player.position;
    //    newOrb.transform.rotation = Quaternion.identity;
    //    Bubble_Weapon bubble = newOrb.GetComponent<Bubble_Weapon>();
    //    bubble.SetBubble(w_Data);

    //    firework.SetActive(false);
    //}
    //#endregion

    #region ItemDrop

    EquippedItem TryDropItem(List<EquippedItem> itemList)
    {
        //드랍 종류 선택 
        Debug.Log("Item dropped");
        float roll = UnityEngine.Random.Range(0, 1.0f);
        float cumulativeChance = 0.0f;

        foreach (var item in itemList)
        {
            cumulativeChance += item.dropChance;

            if (roll <= cumulativeChance)
            {
                AdjustDropChances(item, itemList);
                return item;
            }
        }

        //드랍되지 못한 경우
        Debug.Log("No item dropped");
        return null;

    }

    //각 아이템들의 드롭 찬스 변경. 이미 나온 아이템이 나올 확률은 점점 줄어들고, 다른 아이템이 나올 확률은 높아진다.
    void AdjustDropChances(EquippedItem droppedItem, List<EquippedItem> itemList)
    {
        foreach (var item in itemList)
        {
            if (item == droppedItem)
            {
                item.dropChance = Mathf.Max(item.dropChance - dropChanceDecrement, 0.01f); // 최소 확률 제한
            }
            else
            {
                item.dropChance = Mathf.Min(item.dropChance + dropChanceIncrement, 1.0f); // 최대 확률 제한
            }
        }

        //정규화
        ResizeDropChance(itemList);
    }

    // 드롭 확률을 다시 정규화 (total = 1로 확률을 재분배)
    void ResizeDropChance(List<EquippedItem> itemList)
    {
        float totalChance = 0.0f;
        foreach (var item in itemList)
        {
            totalChance += item.dropChance;
        }

        for (int i = 0; i < itemList.Count; i++)
        {
            itemList[i].dropChance /= totalChance;
        }
    }
    #endregion
}


[Serializable]
public class EquippedItem
{
    //public WeaponData weaponData;
    public string name;
    public float dropChance;
    public object item;

    public EquippedItem(string name, float chance, object item)
    {
        //weaponData = bubble;
        this.name = name;
        dropChance = chance;
        this.item = item;
    }
}

