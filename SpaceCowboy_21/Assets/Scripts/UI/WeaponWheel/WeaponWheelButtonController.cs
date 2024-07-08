using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class WeaponWheelButtonController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] int ID;    //���� ������ ��ȣ
    
    public WeaponData weaponData;
    string itemName;   //���� ������ ������ �̸�
    private Animator animator;
    private WeaponWheelController weaponWheelController;
    private Button button;
    //private bool activate = false;
    [SerializeField] Image gaugeImage;
    [SerializeField] Image icon;
    Material gaugeMaterial;

    private void Awake()
    {
        animator = transform.GetComponent<Animator>();
        weaponWheelController = transform.GetComponentInParent<WeaponWheelController>();  
        button = transform.GetComponent<Button>();
        gaugeMaterial = gaugeImage.material;
        
    }

    public void SetData(WeaponData data, int energyMax)
    {
        this.weaponData = data;
        itemName = data.name;
        gaugeMaterial.SetFloat("_SegmentCount", energyMax);
        icon.sprite = data.WeaponEnableSprite;

    }

    public void SetInteractable(bool interactable)
    {
        button.interactable = interactable;
        
    }

    public void SetPercentGauge(float value)
    {
        gaugeMaterial.SetFloat("_Percent", value);
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
