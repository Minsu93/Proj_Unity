using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechDictionary : MonoBehaviour
{
    //몬스터 이름 string , 그리고 값 prefab이 들어가있는 Dictonary이다. 

    public Dictionary<int, Item> itemDictionary = new Dictionary<int, Item>();
    public Dictionary<int, GameObject> itemPrefabDictionary = new Dictionary<int, GameObject>();


    //아이템 데이터를 받아서 Dictionary에 집어넣는다. itemData에 추가하면 모두 여기에 등록된다. ID로 원하는 Prefab을 찾을 수 있다. 
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
