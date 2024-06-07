using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WeaponWheelController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Image selectedItem;
    [SerializeField] Sprite noImage;
    //public static int weaponID;
    [SerializeField] TextMeshProUGUI itemText;    //텍스트 표시되는곳
    private bool wheelActivate;


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
        GameManager.Instance.playerManager.playerBehavior.TryChangeWeapon(wData);
        selectedItem.sprite = wData.Icon;

        ToggleWheel(false);

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
}
