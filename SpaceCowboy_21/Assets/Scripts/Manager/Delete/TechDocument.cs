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

    //모든 상태 초기화
    public void ResetDocument()
    {
        //iData.techLevel = 0;
        //iData.techCurrExp = 0;
        List<Item> items = iData.itemList;
        for (int i = 0; i < items.Count; i++)
        {
            //items[i].currExp = 0;
            items[i].itemState = (int)ItemStateName.Locked; //"미발견" 상태로 변경(전체 초기화)
        }
    }

    //현재 상태 저장하기
    public void SaveDocument()
    {
        string path = Path.Combine(Application.dataPath, "Data/ItemData", "itemData.json");
        string str = JsonUtility.ToJson(iData, true);
        File.WriteAllText(path, str);
    }

    //현재 상태 불러오기
    public void LoadDocument()
    {
        string path = Path.Combine(Application.dataPath, "Data/ItemData", "itemData.json");
        string loadJson = File.ReadAllText(path);
        iData = JsonUtility.FromJson<ItemData>(loadJson);

        //자손들 세팅
        techDictionary.SetTechDictionary();
    }


    //아이템 상태 체크. 드롭할 때도, 아이템 사용할때도 요걸 체크한 후 아이템 상태를 불러오면 좋다. 
    /// <summary>
    /// return 0 = "Locked", 1 = "Unlocked & Unavailable", 2 = "Available", 3 = "Master"
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public int GetItemState(int id)
    {
        Item item = GetItem(id);
        int state = item.itemState;

        //state 가 Unavailable일때 선행 미션 달성 여부를 체크해본다.
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

    //아이템의 상태 변경. 아이템 발견시, 아이템 해금 시 사용한다. 
    public void SetItemState(int id, ItemStateName state)
    {
        Item item = GetItem(id);
        if (item != null)
        {
            item.itemState = (int)state;
        }
        
    }

    ////아이템 사용으로 경험치 획득
    //public void ItemExpGain(int id, int count)
    //{
    //    List<Item> items = iData.itemList;
    //    Item targetItem = null;

    //    //검색
    //    for(int i = 0; i < items.Count; i++)
    //    {
    //        if(items[i].id == id)
    //        {
    //            targetItem = items[i];
    //            break;
    //        }
    //    }

    //    //targetItem 이 "Available"인 경우.
    //    if(targetItem != null && targetItem.itemState == (int)ItemStateName.Available)
    //    {
    //        targetItem.currExp += targetItem.xpPerEvent * count;

    //        //Item 경험치를 모두 채웠을 경우
    //        if(targetItem.currExp >= targetItem.maxExp)
    //        {
    //            targetItem.itemState = (int)ItemStateName.Master;   //"Master"으로 변경 
    //            targetItem.currExp = targetItem.maxExp;
    //        }
    //    }

    //    // iData.itemList는 참조형이므로, targetItem의 변경 사항이 자동으로 반영됨
    //}



    //리스트에서 아이템Item 가져오기
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
///  아이템 레벨과 Exp. 레벨별 필요 exp 와 LevelUp 관련 해금은 TechLevel에서 실행. 
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
    public string sort; //default 기본 무기(드롭x),drop 드롭 아이템(무기), object 오브젝트
    public string name; // drop Prefab 혹은 Object 의 이름
    public int itemState;   // 0 이면 Unlocked(미발견), 1이면 Unavailable(발견& 사용불가), 2이면 Available(조건 달성 후 사용 가능), 3이면 Master(능숙)
    //public float maxExp;
    //public float currExp;
    //public float xpPerEvent;
}


/// 0 = "Locked", 1 = "Unlocked & Unavailable", 2 = "Available", 3 = "Master"
public enum ItemStateName { Locked, Unlocked/*, Available, Master*/}