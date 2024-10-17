using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopperManager : MonoBehaviour
{

    //�߻� ���� ����
    [SerializeField] GameObject popperPrefab;

    //��� Ȯ�� ���� ����
    //List<EquippedItem> equippedWeapons = new List<EquippedItem>(); // ��� ������ ������ ���
    [SerializeField]
    List<EquippedItem>[] equippedWeapons;
    [SerializeField]
    List<EquippedItem>[] equippedDrones;

    [SerializeField] float dropChanceIncrement = 0.1f; // ��� ���� �� Ȯ�� ������
    [SerializeField] float dropChanceDecrement = 0.05f; // ��� ���� �� Ȯ�� ���ҷ�
    [SerializeField] float totalDropChance = 0.1f;  // Bubble ��� Ȯ�� 
    float currWeaponDropChance;
    float currDroneDropChance;
    [SerializeField] float dropCoolTime = 5f;   //��� �� ���� ���� ��ӱ��� ���� �ּ� �ð�
    bool dropReady = true;
    float timer = 0f;

    private void Update()
    {
        //��� ��Ÿ��
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

    #region �ʱ�ȭ

    public void ReadyWeaponPop(List<WeaponData>[] tierLists, List<GameObject>[] tierLists2)
    {
        //�ʱ�ȭ
        equippedWeapons = new List<EquippedItem>[tierLists.Length];
        for(int i =0; i < equippedWeapons.Length; i++)
        {
            equippedWeapons[i] = new List<EquippedItem>();
        }
        equippedDrones = new List<EquippedItem>[tierLists2.Length];
        for (int i = 0; i < equippedDrones.Length; i++)
        {
            equippedDrones[i] = new List<EquippedItem>();
        }

        //���� ����ֱ�
        for (int i = 0; i < tierLists.Length; i++)
        {
            for(int j = 0; j < tierLists[i].Count; j++)
            {
                equippedWeapons[i].Add(new EquippedItem(0.1f, tierLists[i][j]));
            }
            //Debug.Log("equippedWeapons" + i.ToString() + " count : " + equippedWeapons[i].Count);
            ResizeDropChance(equippedWeapons[i]);
        }
        for (int i = 0; i < tierLists2.Length; i++)
        {
            for (int j = 0; j < tierLists2[i].Count; j++)
            {
                equippedDrones[i].Add(new EquippedItem(0.1f, tierLists2[i][j]));
            }
            ResizeDropChance(equippedDrones[i]);
        }

        //��� Ȯ�� �ʱ�ȭ
        currWeaponDropChance = totalDropChance;
        currDroneDropChance = totalDropChance;
    }

    //public void PopperReady()
    //{
    //    equippedWeapons.Clear();
    //    equippedDrones.Clear();

    //    List<WeaponData> unlockedList = GameManager.Instance.weaponDictionary.unlockedDataList;
    //    for (int i = 0; i < unlockedList.Count; i++)
    //    {
    //        equippedWeapons.Add(new EquippedItem(unlockedList[i].name, 0.1f, unlockedList[i]));
    //    }

    //    List<GameObject> unlockedDroneList = GameManager.Instance.weaponDictionary.unlockedDroneList;
    //    for (int j = 0; j < unlockedDroneList.Count; j++)
    //    {
    //        equippedDrones.Add(new EquippedItem(unlockedDroneList[j].name, 0.1f, unlockedDroneList[j]));
    //    }

    //    ResizeDropChance(equippedWeapons);
    //    ResizeDropChance(equippedDrones);

    //    currWeaponDropChance = totalDropChance;
    //    currDroneDropChance = totalDropChance;
    //}




    #endregion

    #region popper ���
    public void CreatePopper(Transform targetTr)
    {
        WeaponDrop(targetTr);

        DroneDrop(targetTr);
    }
    
    void WeaponDrop(Transform targetTr)
    {
        float random = UnityEngine.Random.Range(0, 1.0f);

        //Weapon ����
        if (random < currWeaponDropChance)
        {
            // ���� �������� ������ ��ġ�� �����մϴ�.
            GameObject firework = GameManager.Instance.poolManager.GetPoolObj(popperPrefab, 2);
            firework.transform.position = targetTr.position;
            firework.transform.rotation = targetTr.rotation;

            int tier = PM_LuckLevel.itemTier;
            WeaponData data = TryDropItem(equippedWeapons[tier]).item as WeaponData;
            if (data != null)
            {
                Vector2 targetPoint = GetEmptySpace(targetTr);
                StartCoroutine(firework.GetComponent<Popper>().CreateWeaponBubble(targetTr.position, targetPoint, data));
            }
            else Debug.LogWarning("WeaponData cast failed. The item is not of type WeaponData.");

            currWeaponDropChance = totalDropChance;
            //dropReady = false;
        }
        else
        {
            //����� �ȵ� ��� 1.1�辿 ��� ����.
            currWeaponDropChance *= 1.1f;
        }
    }

    void DroneDrop(Transform targetTr)
    {
        if (!GameManager.Instance.playerManager.IsDroneDropPossible())
        {
            return;
        }

        float random = UnityEngine.Random.Range(0, 1.0f);

        //Drone ����
        if (random < currDroneDropChance)
        {
            // ���� �������� ������ ��ġ�� �����մϴ�.
            GameObject firework = GameManager.Instance.poolManager.GetPoolObj(popperPrefab, 2);
            firework.transform.position = targetTr.position;
            firework.transform.rotation = targetTr.rotation;

            int tier = PM_LuckLevel.itemTier;
            GameObject prefab = TryDropItem(equippedDrones[tier]).item as GameObject;
            if (prefab != null)
            {
                Vector2 targetPoint = GetEmptySpace(targetTr);
                StartCoroutine(firework.GetComponent<Popper>().CreateDroneBubble(targetTr.position, targetPoint, prefab));
            }
            else Debug.LogWarning("DroneObject cast failed. The item is not of type DroneObject.");

            currDroneDropChance = totalDropChance;
            //dropReady = false;
        }
        else
        {
            currDroneDropChance *= 1.1f;
        }
    }

    //Ÿ�� ���κ� �ֺ��� ����Ʈ ������. �༺�� ���� ���. 
    Vector3 GetEmptySpace(Transform targetTr)
    {
        int maxAttempts = 100;
        Vector3 emptyPos = new Vector3();

        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = UnityEngine.Random.Range(0, Mathf.PI * 2); 
            float distance = UnityEngine.Random.Range(3f, 5f); 
            Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // �� ������ �Ÿ��� ����Ͽ� �� ������ ������ ����Ʈ�� ����մϴ�.
            randomPoint += (Vector2)targetTr.position;

            //�� ���� �ƴ��� �˻��Ѵ�
            Vector2 mapSize = GameManager.Instance.MapSize;
            Vector2 center = GameManager.Instance.MapCenter;
            if(randomPoint.x < center.x - mapSize.x || randomPoint.x > center.x + mapSize.x || randomPoint.y < center.y - mapSize.y || randomPoint.y > center.y + mapSize.y)
            {
                continue;
            }

            //���� ����� (���� �� �ִ�)��带 �����´�
            var constraint = NNConstraint.None;

            // Constrain the search to walkable nodes only
            constraint.constrainWalkability = true;
            constraint.walkable = true;

            GraphNode node = AstarPath.active.GetNearest((Vector3)randomPoint, constraint).node;
            if(node != null)
            {
                emptyPos = (Vector3)node.position;
                break;
            }
        }

        return emptyPos;
    }

    #endregion

    //#region �÷��̾� ���̷�Ʈ ��� ���
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

    //    // ������ ��ǥ ��ġ�� ������ ������ �̵��մϴ�.
    //    while (time <= launchTimer)
    //    {
    //        Vector2 targetPos = GameManager.Instance.player.position;
    //        time += Time.deltaTime;

    //        Vector2 pos = Vector2.Lerp(startPos, targetPos, fireworkCurve.Evaluate(time / launchTimer));
    //        firework.transform.position = pos;
    //        yield return null;
    //    }

    //    // ���� ����.
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
        //��� ���� ���� 
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

        //������� ���� ���
        return null;

    }

    //�� �����۵��� ��� ���� ����. �̹� ���� �������� ���� Ȯ���� ���� �پ���, �ٸ� �������� ���� Ȯ���� ��������.
    void AdjustDropChances(EquippedItem droppedItem, List<EquippedItem> itemList)
    {
        foreach (var item in itemList)
        {
            if (item == droppedItem)
            {
                item.dropChance = Mathf.Max(item.dropChance - dropChanceDecrement, 0.01f); // �ּ� Ȯ�� ����
            }
            else
            {
                item.dropChance = Mathf.Min(item.dropChance + dropChanceIncrement, 1.0f); // �ִ� Ȯ�� ����
            }
        }

        //����ȭ
        ResizeDropChance(itemList);
    }

    // ��� Ȯ���� �ٽ� ����ȭ (total = 1�� Ȯ���� ��й�)
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
    //public string name;
    public float dropChance;
    public object item;

    public EquippedItem(float chance, object item)
    {
        //weaponData = bubble;
        //this.name = name;
        dropChance = chance;
        this.item = item;
    }
}

