using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Spine.Unity;
using Spine;
using System.Net.Mail;


public class TechDocument : MonoBehaviour
{
    public static ItemData iData;
    TechMission techMission;
    public TechDictionary techDictionary { get; private set; }
    TechLevel techLevel;

    private void Awake()
    {
        techMission = GetComponent<TechMission>();
        techDictionary = GetComponent<TechDictionary>();
        techLevel = GetComponent<TechLevel>();

        LoadDocument();
    }

    //��� ���� �ʱ�ȭ
    public void ResetDocument()
    {
        //iData.techLevel = 0;
        //iData.techCurrExp = 0;
        List<Item> items = iData.itemList;
        for (int i = 0; i < items.Count; i++)
        {
            //items[i].currExp = 0;
            items[i].itemState = (int)ItemStateName.Locked; //"�̹߰�" ���·� ����(��ü �ʱ�ȭ)
        }
    }

    //���� ���� �����ϱ�
    public void SaveDocument()
    {
        string path = Path.Combine(Application.dataPath, "Data/ItemData", "itemData.json");
        string str = JsonUtility.ToJson(iData, true);
        File.WriteAllText(path, str);
    }

    //���� ���� �ҷ�����
    public void LoadDocument()
    {
        string path = Path.Combine(Application.dataPath, "Data/ItemData", "itemData.json");
        string loadJson = File.ReadAllText(path);
        iData = JsonUtility.FromJson<ItemData>(loadJson);

        //�ڼյ� ����
        techDictionary.SetTechDictionary();
    }


    //������ ���� üũ. ����� ����, ������ ����Ҷ��� ��� üũ�� �� ������ ���¸� �ҷ����� ����. 
    /// <summary>
    /// return 0 = "Locked", 1 = "Unlocked & Unavailable", 2 = "Available", 3 = "Master"
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetItemState(int id)
    {
        Item item = GetItem(id);
        int state = item.itemState;

        //state �� Unavailable�϶� ���� �̼� �޼� ���θ� üũ�غ���.
        //if(state == (int)ItemStateName.Unavailable)
        //{
        //    state = techMission.CheckMission(id, item);
        //}

        if (state == (int)ItemStateName.Locked)
        {
            state = (int)ItemStateName.Unlocked;
        }

        return state;
    }

    //�������� ���� ����. ������ �߽߰�, ������ �ر� �� ����Ѵ�. 
    public void SetItemState(int id, ItemStateName state)
    {
        Item item = GetItem(id);
        if (item != null)
        {
            item.itemState = (int)state;
        }
        
    }

    ////������ ������� ����ġ ȹ��
    //public void ItemExpGain(int id, int count)
    //{
    //    List<Item> items = iData.itemList;
    //    Item targetItem = null;

    //    //�˻�
    //    for(int i = 0; i < items.Count; i++)
    //    {
    //        if(items[i].id == id)
    //        {
    //            targetItem = items[i];
    //            break;
    //        }
    //    }

    //    //targetItem �� "Available"�� ���.
    //    if(targetItem != null && targetItem.itemState == (int)ItemStateName.Available)
    //    {
    //        targetItem.currExp += targetItem.xpPerEvent * count;

    //        //Item ����ġ�� ��� ä���� ���
    //        if(targetItem.currExp >= targetItem.maxExp)
    //        {
    //            targetItem.itemState = (int)ItemStateName.Master;   //"Master"���� ���� 
    //            targetItem.currExp = targetItem.maxExp;
    //        }
    //    }

    //    // iData.itemList�� �������̹Ƿ�, targetItem�� ���� ������ �ڵ����� �ݿ���
    //}



    //����Ʈ���� ������Item ��������
    public Item GetItem(int id)
    {
        //List<Item> items = iData.itemList;
        //Item targetItem = null;

        //for (int i = 0; i < items.Count; i++)
        //{
        //    if (items[i].id == id)
        //    {
        //        targetItem = items[i];
        //        break;
        //    }
        //}
        techDictionary.itemDictionary.TryGetValue(id, out Item targetItem);

        return targetItem;
    }


    //[SerializeField] List<Sprite> gunSprites = new List<Sprite>();
    //public Sprite GetItemImage(int id)
    //{
    //    foreach(Sprite gunSpr in gunSprites)
    //    {
    //        if(gunSpr.name == id.ToString())
    //        {
    //            return gunSpr;
    //        }
    //    }

    //    return null;
    //}

    public GameObject GetPrefab(int id)
    {
        techDictionary.itemPrefabDictionary.TryGetValue(id, out var prefab);
        return prefab;
    }

}

/// <summary>
///  ������ ������ Exp. ������ �ʿ� exp �� LevelUp ���� �ر��� TechLevel���� ����. 
/// </summary>
[Serializable]
public class ItemData
{
    //public int techLevel;
    //public float techCurrExp;
    public List<Item> itemList = new List<Item>();
}

[Serializable]
public class Item
{
    public int id;
    public string sort; //default �⺻ ����(���x),drop ��� ������(����), object ������Ʈ
    public string name; // drop Prefab Ȥ�� Object �� �̸�
    public int itemState;   // 0 �̸� Unlocked(�̹߰�), 1�̸� Unavailable(�߰�& ���Ұ�), 2�̸� Available(���� �޼� �� ��� ����), 3�̸� Master(�ɼ�)
    //public float maxExp;
    //public float currExp;
    //public float xpPerEvent;
}


/// 0 = "Locked", 1 = "Unlocked & Unavailable", 2 = "Available", 3 = "Master"
public enum ItemStateName { Locked, Unlocked/*, Available, Master*/}