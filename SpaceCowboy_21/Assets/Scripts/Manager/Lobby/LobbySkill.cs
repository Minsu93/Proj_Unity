using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] Image curUsedEnergyImage;
    public float maxEnergy = 20.0f; //�Ͻ����� ���� �ִ� ������ ��. �߰� ���׷��̵带 ���ؼ� ���� ����. 

    private void Start()
    {
        SkillTotalData totalSkill = GameManager.Instance.skillDictionary.skillTotal;
        SetSkill_Inventory(totalSkill);

        equippedSkillList = GameManager.Instance.skillDictionary.LoadEquippedSkill();
        UpdateEquippedSkill(equippedSkillList);
    }

    [SerializeField] List<LobbyWeaponButton> skill_InventoryButtons = new List<LobbyWeaponButton>();
    List<string> unlockedSkilNames = new List<string>();

    //�رݵ� ��ų���� �ҷ��ͼ� ����Ʈ�� �κ��丮�� ä���.
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
    float usedEnergy = 0f;

    void UpdateEquippedSkill(List<string> equippedList)
    {
        foreach (var button in skillEquipmentButtons)
        {
            button.ClearButton();
        }

        //��� ������ �ʱ�ȭ
        usedEnergy = 0f;

        for (int i = 0; i < equippedList.Count; i++)
        {
            //1. equipped ��ư�� Ȱ��ȭ.
            Sprite icon = GameManager.Instance.skillDictionary.GetSkillIcon(equippedList[i]);
            skillEquipmentButtons[i].SetSkillDisarmButton(icon, equippedList[i], this);
            skillEquipmentButtons[i].SetInteractableButton(true);

            //2.�κ��丮 ��ư ��Ȱ��ȭ
            int invenIndex = unlockedSkilNames.FindIndex(x => x.Equals(equippedList[i]));
            skill_InventoryButtons[invenIndex].SetInteractableButton(false);

            //3. ������� ������ üũ�ϱ�
            usedEnergy += GameManager.Instance.skillDictionary.GetSkillData(equippedList[i]).energyUse;
        }

        curUsedEnergyImage.fillAmount = usedEnergy / maxEnergy;
    }

    public void EquipSkill(string name)
    {
        //������ �ڸ��� ���� ������
        if(equippedSkillList.Count < skillEquipmentButtons.Count)
        {
            //������ �䱸 �ѷ��� MaxEnergy�� ���� ������
            float neededEnergy = GameManager.Instance.skillDictionary.GetSkillData(name).energyUse;
            if (usedEnergy + neededEnergy <= maxEnergy)
            {
                equippedSkillList.Add(name);
                UpdateEquippedSkill(equippedSkillList);
                SaveEquippedSkill();
            }
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
