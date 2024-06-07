using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WeaponWheelButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("Hover", true);
        weaponWheelController.ShowItemNameText(itemName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("Hover", false);
        weaponWheelController.ShowItemNameText(" ");
    }
}
