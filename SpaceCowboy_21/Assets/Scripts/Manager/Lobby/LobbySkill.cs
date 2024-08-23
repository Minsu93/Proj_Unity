using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySkill : MonoBehaviour
{
    ///해금된 스킬을 리스트에 추가한다. 
    ///장착한 스킬을 equip리스트에 추가한다. 
    ///
    /// 1. 현재 total skill 을 받아온다. 
    /// 2. 해금된 스킬을 인벤토리 리스트에 추가한다. 
    /// 3. 장착된 스킬을 Load.
    /// 4. 장착된 스킬은 해금 리스트에서 비활성화한다. 
    /// 5. 장착된 스킬을 equip 리스트에 추가한다. 
    /// 6. 해금된 스킬을 클릭시 장착. 장착된 스킬을 클릭 시 장착 해제.
    /// 7. 장착이 변경될 때마다 장착 정보를 저장 Save.
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
            //1. equipped 버튼을 활성화.
            Sprite icon = GameManager.Instance.skillDictionary.GetSkillIcon(equippedList[i]);
            skillEquipmentButtons[i].SetSkillDisarmButton(icon, equippedList[i], this);
            skillEquipmentButtons[i].SetInteractableButton(true);

            //인벤토리 버튼 비활성화
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

        //인벤토리 버튼 활성화
        int invenIndex = unlockedSkilNames.FindIndex(x => x.Equals(name));
        skill_InventoryButtons[invenIndex].SetInteractableButton(true);
    }

    void SaveEquippedSkill()
    {
        GameManager.Instance.skillDictionary.SaveEquippedSkill(equippedSkillList);
    }

}
