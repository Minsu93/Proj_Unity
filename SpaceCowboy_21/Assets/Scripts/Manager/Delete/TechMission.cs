using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechMission : MonoBehaviour
{
    //������ ���� ���� �˻�
    //public int CheckMission(int id, Item targetItem)
    //{
    //    //�޼��ؾ� �Ǵ� �������ǵ��� ������ �ߴ��� �˻��� �� ���� ����

    //    //id�� ���� ���� �ٸ� �̼�
    //    switch (id)
    //    {
    //        case 10001:
    //            //���� ���� �� 
    //            targetItem.itemState = (int)ItemStateName.Available;
    //            break;
    //        case 10002:
    //            //10001 �� 10003�� "Master" �� ���, targetItem �� ItemState �� 2(Available) ���� �ٲ۴�. 
    //            if(CheckItemMaster(10001) && CheckItemMaster(10003))
    //            {
    //                targetItem.itemState = (int)ItemStateName.Available;
    //            }
                
    //            break;
    //    }

    //    return targetItem.itemState;

    //}

    ////����Ʈ���� ������ ��������
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

    ////�������� ���°� "Master" ���� �˻�
    //bool CheckItemMaster(int id)
    //{
    //    Item item = GetItem(id);

    //    if (item == null) return false;
    //    return GetItem(id).itemState == (int)ItemStateName.Master ? true : false;
    //}
}
