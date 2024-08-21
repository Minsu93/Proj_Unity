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


    //버튼 생성 시 초기화
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

    //버튼 클릭 시 장착을 시도
    void EquipEvent(WeaponState state, LobbyWeapon lobby)
    {
        if (lobby.EquipItem(state))
        {
            //장착 성공 시 
            SetInteractableButton(false);
        }
        else
        {
            //장착 실패 시 아무일도 일어나지 않음.
        }
    }

    //버튼 클릭 시 구매를 시도 
    void PurchaseEvent(WeaponState state, LobbyWeapon lobby)
    {
        //돈 있는지 검사 

        //돈이 있는 경우에만 다음으로 작동 

        lobby.BuyItem(state);
    }

    void EquipmentDisarmEvent(WeaponState state, LobbyWeapon lobby)
    {
        //해당하는 버튼의 아이템을 장착 해제헌다. 
        lobby.DisarmItem(state);
        image.sprite = null;
        if (button.interactable)
        {
            SetInteractableButton(false);
        }
        
    }

    public void SetInteractableButton(bool enable)
    {
        button.interactable = enable;
    }



}
