using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechMission : MonoBehaviour
{
    //아이템 선행 조건 검사
    //public int CheckMission(int id, Item targetItem)
    //{
    //    //달성해야 되는 선행조건들을 마스터 했는지 검사한 후 상태 변경

    //    //id에 따른 서로 다른 미션
    //    switch (id)
    //    {
    //        case 10001:
    //            //설정 없을 때 
    //            targetItem.itemState = (int)ItemStateName.Available;
    //            break;
    //        case 10002:
    //            //10001 과 10003이 "Master" 인 경우, targetItem 의 ItemState 를 2(Available) 으로 바꾼다. 
    //            if(CheckItemMaster(10001) && CheckItemMaster(10003))
    //            {
    //                targetItem.itemState = (int)ItemStateName.Available;
    //            }
                
    //            break;
    //    }

    //    return targetItem.itemState;

    //}

    ////리스트에서 아이템 가져오기
    //Item GetItem(int id)
    //{
    //    List<Item> items = TechDocument.iData.itemList;
    //    Item targetItem = null;

    //    for (int i = 0; i < items.Count; i++)
    //    {
    //        if (items[i].id == id)
    //        {
    //            targetItem = items[i];
    //            break;
    //        }
    //    }

    //    return targetItem;
    //}

    ////아이템의 상태가 "Master" 인지 검사
    //bool CheckItemMaster(int id)
    //{
    //    Item item = GetItem(id);

    //    if (item == null) return false;
    //    return GetItem(id).itemState == (int)ItemStateName.Master ? true : false;
    //}
}
