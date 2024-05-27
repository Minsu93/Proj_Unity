using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponWheelButtonController : MonoBehaviour
{
    [SerializeField] int ID;    //���� ������ ��ȣ
    [SerializeField] string itemName;   //���� ������ ������ �̸�
    [SerializeField] WeaponData weaponData;

    private Animator animator;
    private WeaponWheelController weaponWheelController;


    private void Awake()
    {
        animator = transform.GetComponent<Animator>();
        weaponWheelController = transform.GetComponentInParent<WeaponWheelController>();  
    }

    //������ ����
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
