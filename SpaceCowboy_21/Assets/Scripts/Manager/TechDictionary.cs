using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechDictionary : MonoBehaviour
{
    //���� �̸� string , �׸��� �� prefab�� ���ִ� Dictonary�̴�. 

    public Dictionary<int, Item> itemDictionary = new Dictionary<int, Item>();
    public Dictionary<int, GameObject> itemPrefabDictionary = new Dictionary<int, GameObject>();


    //������ �����͸� �޾Ƽ� Dictionary�� ����ִ´�. itemData�� �߰��ϸ� ��� ���⿡ ��ϵȴ�. ID�� ���ϴ� Prefab�� ã�� �� �ִ�. 
    public void SetTechDictionary()
    {
        List<Item> itemList = TechDocument.iData.itemList;
        for(int i = 0;i < itemList.Count;i++)
        {
            int id = itemList[i].id;
            GameObject obj = Resources.Load<GameObject>("Item/" + itemList[i].sort + "/" + itemList[i].name);
            itemPrefabDictionary.Add(id, obj);

            itemDictionary.Add(id, itemList[i]);
        }
    }



}

[Serializable]
public struct itemStruct
{
    public int itemID;
    public GameObject itemPrefab;

    public itemStruct(int itemID, GameObject itemPrefab)
    {
        this.itemID = itemID;
        this.itemPrefab = itemPrefab;
    }
}
