using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechLevel : MonoBehaviour
{
    //[SerializeField] private List<float> expNeedPerLevel = new List<float>();
    ///// <summary>
    ///// 레벨업시 해금되는 Item 들의 ID 리스트.
    ///// </summary>
    //[SerializeField] private List<IDs> unlockedItemIdPerLevel = new List<IDs>();

    ////Tech 경험치 획득 
    //public void TechExpGain(int count)
    //{
    //    //필요한 정보를 불러온다. 
    //    int lv = TechDocument.iData.techLevel;
    //    float exp = TechDocument.iData.techCurrExp;

    //    //tech 경험치 획득
    //    exp = exp + (10 * count);

    //    //경험치가 필요한 이상 쌓이면 레벨업
    //    if(exp >= expNeedPerLevel[lv])
    //    {
    //        exp -= expNeedPerLevel[lv];

    //        lv++;
    //        TechDocument.iData.techLevel = lv;
    //        UnlockItemPerLevel(lv);
    //    }

    //    //해당 레벨에 해금되는 아이템을 해금한다. 
    //    TechDocument.iData.techCurrExp = exp;

        
    //}

    ////레벨별 해금되는 아이템 리스트.
    //void UnlockItemPerLevel(int level)
    //{
    //    List<int> unlockedIDs = unlockedItemIdPerLevel[level].id;
    //    for(int i = 0; i < unlockedIDs.Count; i++)
    //    {
    //        GameManager.Instance.techDocument.SetItemState(unlockedIDs[i], ItemStateName.Unlocked);
    //    }
        
    //}
}

[Serializable]
public class IDs
{
    public List<int> id = new List<int>();
}
