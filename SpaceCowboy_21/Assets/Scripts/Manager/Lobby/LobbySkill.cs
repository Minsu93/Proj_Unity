using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] Image curUsedEnergyImage;
    public float maxEnergy = 20.0f; //일시적인 착용 최대 에너지 량. 추가 업그레이드를 통해서 증축 가능. 

    private void Start()
    {
        SkillTotalData totalSkill = GameManager.Instance.skillDictionary.skillTotal;
        SetSkill_Inventory(totalSkill);

        equippedSkillList = GameManager.Instance.skillDictionary.LoadEquippedSkill();
        UpdateEquippedSkill(equippedSkillList);
    }

    [SerializeField] List<LobbyWeaponButton> skill_InventoryButtons = new List<LobbyWeaponButton>();
    List<string> unlockedSkilNames = new List<string>();

    //해금된 스킬들을 불러와서 리스트로 인벤토리를 채운다.
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

        //사용 에너지 초기화
        usedEnergy = 0f;

        for (int i = 0; i < equippedList.Count; i++)
        {
            //1. equipped 버튼을 활성화.
            Sprite icon = GameManager.Instance.skillDictionary.GetSkillIcon(equippedList[i]);
            skillEquipmentButtons[i].SetSkillDisarmButton(icon, equippedList[i], this);
            skillEquipmentButtons[i].SetInteractableButton(true);

            //2.인벤토리 버튼 비활성화
            int invenIndex = unlockedSkilNames.FindIndex(x => x.Equals(equippedList[i]));
            skill_InventoryButtons[invenIndex].SetInteractableButton(false);

            //3. 사용중인 에너지 체크하기
            usedEnergy += GameManager.Instance.skillDictionary.GetSkillData(equippedList[i]).energyUse;
        }

        curUsedEnergyImage.fillAmount = usedEnergy / maxEnergy;
    }

    public void EquipSkill(string name)
    {
        //착용할 자리가 남아 있으면
        if(equippedSkillList.Count < skillEquipmentButtons.Count)
        {
            //에너지 요구 총량이 MaxEnergy를 넘지 않으면
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

        //인벤토리 버튼 활성화
        int invenIndex = unlockedSkilNames.FindIndex(x => x.Equals(name));
        skill_InventoryButtons[invenIndex].SetInteractableButton(true);
    }

    void SaveEquippedSkill()
    {
        GameManager.Instance.skillDictionary.SaveEquippedSkill(equippedSkillList);
    }

}
