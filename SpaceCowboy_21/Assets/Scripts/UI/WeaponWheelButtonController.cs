using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelButtonController : MonoBehaviour
{
    [SerializeField] int ID;    //현재 슬롯의 번호
    [SerializeField] string itemName;   //현재 슬롯의 아이템 이름
    [SerializeField] WeaponData weaponData;

    private Animator animator;
    private WeaponWheelController weaponWheelController;


    private void Awake()
    {
        animator = transform.GetComponent<Animator>();
        weaponWheelController = transform.GetComponentInParent<WeaponWheelController>();  
    }

    //아이템 장착
    public void Selected()
    {
        weaponWheelController.ActivateItem(ID, weaponData);
    }

    public void HoverEnter()
    {
        animator.SetBool("Hover", true);
        weaponWheelController.ShowItemNameText(itemName);
    }

    public void HoverExit()
    {
        animator.SetBool("Hover", false);
        weaponWheelController.ShowItemNameText(" ");
    }
}
