using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyWeaponButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;


    //��ư ���� �� �ʱ�ȭ
    public void SetEquipButton(WeaponState state, LobbyWeapon lobby)
    {
        image.sprite = state.weaponData.Icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => EquipEvent(state,lobby));
    }

    
    public void SetShopButton(WeaponState state, LobbyWeapon lobby)
    {
        image.sprite = state.weaponData.Icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => PurchaseEvent(state, lobby));
        if (text != null) text.text = "Cost : " + state.price.ToString();
    }

    public void SetDisarmButton(WeaponState state, LobbyWeapon lobby)
    {
        image.sprite = state.weaponData.Icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => EquipmentDisarmEvent(state, lobby));
    }

    public void SetSkillEquipButton(Sprite icon, string name, LobbySkill lobby)
    {
        image.sprite = icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SkillEquipEvent(name, lobby));

    }
    public void SetSkillDisarmButton(Sprite icon, string name, LobbySkill lobby)
    {
        image.sprite = icon;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => SkillDisarmEvent(name, lobby));

    }

    //��ư Ŭ�� �� ������ �õ�
    void EquipEvent(WeaponState state, LobbyWeapon lobby)
    {
        if (lobby.EquipItem(state))
        {
            //���� ���� �� 
            SetInteractableButton(false);
        }
        else
        {
            //���� ���� �� �ƹ��ϵ� �Ͼ�� ����.
        }
    }

    //��ư Ŭ�� �� ���Ÿ� �õ� 
    void PurchaseEvent(WeaponState state, LobbyWeapon lobby)
    {
        //�� �ִ��� �˻� 

        //���� �ִ� ��쿡�� �������� �۵� 

        lobby.BuyItem(state);
    }

    void EquipmentDisarmEvent(WeaponState state, LobbyWeapon lobby)
    {
        //�ش��ϴ� ��ư�� �������� ���� �������. 
        lobby.DisarmItem(state);
    }
    void SkillEquipEvent(string name, LobbySkill lobby)
    {
        lobby.EquipSkill(name);
    }

    void SkillDisarmEvent(string name, LobbySkill lobby)
    {
        lobby.DisarmSkill(name);
    }

    public void SetInteractableButton(bool enable)
    {
        button.interactable = enable;
    }

    public void ClearButton()
    {
        image.sprite = Resources.Load<Sprite>("UI/Empty");
        if(text!= null) text.text = "";
        button.onClick.RemoveAllListeners();
        SetInteractableButton(false);

    }

}
