using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySkill : MonoBehaviour
{
    ///�رݵ� ��ų�� ����Ʈ�� �߰��Ѵ�. 
    ///������ ��ų�� equip����Ʈ�� �߰��Ѵ�. 
    ///
    /// 1. ���� total skill �� �޾ƿ´�. 
    /// 2. �رݵ� ��ų�� �κ��丮 ����Ʈ�� �߰��Ѵ�. 
    /// 3. ������ ��ų�� Load.
    /// 4. ������ ��ų�� �ر� ����Ʈ���� ��Ȱ��ȭ�Ѵ�. 
    /// 5. ������ ��ų�� equip ����Ʈ�� �߰��Ѵ�. 
    /// 6. �رݵ� ��ų�� Ŭ���� ����. ������ ��ų�� Ŭ�� �� ���� ����.
    /// 7. ������ ����� ������ ���� ������ ���� Save.
    ///

    private void Start()
    {
        GameManager.Instance.skillDictionary.LoadSkillDictionary();
        SkillTotalData totalSkill = GameManager.Instance.skillDictionary.skillTotal;
        SetSkill_Inventory(totalSkill);

        equippedSkillList = GameManager.Instance.skillDictionary.LoadEquippedSkill();
        UpdateEquippedSkill(equippedSkillList);
    }

    [SerializeField] List<LobbyWeaponButton> skill_InventoryButtons = new List<LobbyWeaponButton>();
    List<string> unlockedSkilNames = new List<string>();

    void SetSkill_Inventory(SkillTotalData totalSkill)
    {

        for(int i = 0; i < totalSkill.skillDataList.Count; i++)
        {
            if (totalSkill.skillDataList[i].unlocked)
            {
                unlockedSkilNames.Add(totalSkill.skillDataList[i].name);
            }
        }

        for(int j = 0; j < skill_InventoryButtons.Count; j++)
        {
            if(j < unlockedSkilNames.Count)
            {
                Sprite icon = GameManager.Instance.skillDictionary.GetSkillIcon(unlockedSkilNames[j]);
                skill_InventoryButtons[j].SetSkillEquipButton(icon, unlockedSkilNames[j], this);
                skill_InventoryButtons[j].gameObject.SetActive(true);

            }
            else
            {
                skill_InventoryButtons[j].gameObject.SetActive(false);
            }
        }
    }

    List<string> equippedSkillList = new List<string>();
    [SerializeField] List<LobbyWeaponButton> skillEquipmentButtons = new List<LobbyWeaponButton>();

    void UpdateEquippedSkill(List<string> equippedList)
    {
        foreach (var button in skillEquipmentButtons)
        {
            button.ClearButton();
        }
        for (int i = 0; i < equippedList.Count; i++)
        {
            //1. equipped ��ư�� Ȱ��ȭ.
            Sprite icon = GameManager.Instance.skillDictionary.GetSkillIcon(equippedList[i]);
            skillEquipmentButtons[i].SetSkillDisarmButton(icon, equippedList[i], this);
            skillEquipmentButtons[i].SetInteractableButton(true);

            //�κ��丮 ��ư ��Ȱ��ȭ
            int invenIndex = unlockedSkilNames.FindIndex(x => x.Equals(equippedList[i]));
            skill_InventoryButtons[invenIndex].SetInteractableButton(false);
        }
    }

    public void EquipSkill(string name)
    {
        if(equippedSkillList.Count < skillEquipmentButtons.Count)
        {
            equippedSkillList.Add(name);
            UpdateEquippedSkill(equippedSkillList);
            SaveEquippedSkill();
        }

    }

    public void DisarmSkill(string name)
    {
        equippedSkillList.Remove(name);
        UpdateEquippedSkill(equippedSkillList);
        SaveEquippedSkill();

        //�κ��丮 ��ư Ȱ��ȭ
        int invenIndex = unlockedSkilNames.FindIndex(x => x.Equals(name));
        skill_InventoryButtons[invenIndex].SetInteractableButton(true);
    }

    void SaveEquippedSkill()
    {
        GameManager.Instance.skillDictionary.SaveEquippedSkill(equippedSkillList);
    }

}
