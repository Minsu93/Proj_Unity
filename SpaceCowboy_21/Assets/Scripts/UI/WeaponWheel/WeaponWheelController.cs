using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class WeaponWheelController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Image selectedItem;
    [SerializeField] Sprite noImage;
    //public static int weaponID;
    [SerializeField] TextMeshProUGUI itemText;    //텍스트 표시되는곳
    [SerializeField] WeaponWheelButtonController[] buttons = new WeaponWheelButtonController[4];
    [SerializeField] Image[] miniWheels = new Image[4]; //화면 우측 하단부분에 있는 미니 무기 휠.
    [SerializeField] Sprite[] onSprite = new Sprite[4]; //활성화된 이미지
    [SerializeField] Sprite[] offSprite = new Sprite[4];    //비활성화된 이미지
    [SerializeField] RectTransform selectedWeaponSquare;    //활성화된 무기 표시 Image

    private bool wheelActivate;

    //private float[] button_Gauges = new float[4];
    

    private void Awake()
    {
        buttons = GetComponentsInChildren<WeaponWheelButtonController>();
    }



    public void ToggleWheel(bool activate)
    {
        //WheelActivate 가 false이고  activate 가 true 일때  >> 오픈
        if(!wheelActivate && activate)
        {
            wheelActivate = true;
            animator.SetBool("OpenWeaponWheel", true);
            GameManager.Instance.playerManager.DisablePlayerShoot();

        }

        //WheelActivate 가  true 이고 activate 가 false일때 >> 종료
        if (wheelActivate && !activate)
        {
            wheelActivate = false;
            animator.SetBool("OpenWeaponWheel", false);
            ShowItemNameText("");
            GameManager.Instance.playerManager.EnablePlayerShoot();

        }

    }   

    //아이템 활성화
    public void ActivateItem(int ID, WeaponData wData)
    {
        //GameManager.Instance.playerManager.ChangeWeapon(ID);
        //selectedItem.sprite = wData.Icon;

        //ToggleWheel(false);

    }

    //아이템 아이콘 비활성화
    public void CancelItem()
    {
        selectedItem.sprite = noImage;
    }

    //아이템 이름 
    public void ShowItemNameText(string str)
    {
        itemText.text = str;
    }

    //선택 표식 위치 변경
    public void MoveSelectedSquarePosition(int index)
    {
        selectedWeaponSquare.anchoredPosition = miniWheels[index].rectTransform.anchoredPosition;
    }

    //첫 버튼 설정 시
    //public void SetItemData(WeaponInventory[] w_Inventory)
    //{
    //    //버튼 활성화 설정
    //    for(int i = 0; i < w_Inventory.Length; i++)
    //    {
    //        buttons[i].SetData(w_Inventory[i].weaponData, w_Inventory[i].maxWeaponEnergy);


    //        //미니휠 설정
    //        onSprite[i] = w_Inventory[i].weaponData.WeaponEnableSprite;
    //        offSprite[i] = w_Inventory[i].weaponData.WeaponDisableSprite;
    //        miniWheels[i].sprite = offSprite[i];

    //        //활성화 여부에 따라 UI 교체
    //        if (w_Inventory[i].activate)
    //        {
    //            buttons[i].SetInteractable(true);
    //            miniWheels[i].sprite = onSprite[i];
    //        }
    //        else
    //        {
    //            buttons[i].SetInteractable(false);
    //        }

    //    }
    //}

    //무기 게이지 변할 때마다
    //public void UpdateWeaponGauge(WeaponInventory[] w_Inventory)
    //{
    //    float curEng;
    //    float maxEng;
    //    float value;
    //    //string valueName = "";

    //    for (int i = 0; i < w_Inventory.Length; i++)
    //    {
    //        //무기 버튼 활성화, 비활성화
    //        if (w_Inventory[i].activate)
    //        {
    //            buttons[i].SetInteractable(true);
    //            miniWheels[i].sprite = onSprite[i];
    //        }
    //        else
    //        {
    //            buttons[i].SetInteractable(false);
    //            miniWheels[i].sprite = offSprite[i];
    //        }
            
    //        //무기 percent 게이지
    //        curEng = w_Inventory[i].curWeaponEnergy;
    //        maxEng = w_Inventory[i].maxWeaponEnergy;
    //        if (maxEng > 0f)
    //        {
    //            value = curEng / maxEng;
    //        }
    //        else
    //        {
    //            value = 1f;
    //        }
    //        buttons[i].SetPercentGauge(value);
    //    }

        
    //}

}
