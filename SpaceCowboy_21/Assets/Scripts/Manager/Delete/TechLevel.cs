using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechLevel : MonoBehaviour
{
    //[SerializeField] private List<float> expNeedPerLevel = new List<float>();
    ///// <summary>
    ///// �������� �رݵǴ� Item ���� ID ����Ʈ.
    ///// </summary>
    //[SerializeField] private List<IDs> unlockedItemIdPerLevel = new List<IDs>();

    ////Tech ����ġ ȹ�� 
    //public void TechExpGain(int count)
    //{
    //    //�ʿ��� ������ �ҷ��´�. 
    //    int lv = TechDocument.iData.techLevel;
    //    float exp = TechDocument.iData.techCurrExp;

    //    //tech ����ġ ȹ��
    //    exp = exp + (10 * count);

    //    //����ġ�� �ʿ��� �̻� ���̸� ������
    //    if(exp >= expNeedPerLevel[lv])
    //    {
    //        exp -= expNeedPerLevel[lv];

    //        lv++;
    //        TechDocument.iData.techLevel = lv;
    //        UnlockItemPerLevel(lv);
    //    }

    //    //�ش� ������ �رݵǴ� �������� �ر��Ѵ�. 
    //    TechDocument.iData.techCurrExp = exp;

        
    //}

    ////������ �رݵǴ� ������ ����Ʈ.
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
