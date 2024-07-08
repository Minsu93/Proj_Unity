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
    [SerializeField] TextMeshProUGUI itemText;    //�ؽ�Ʈ ǥ�õǴ°�
    [SerializeField] WeaponWheelButtonController[] buttons = new WeaponWheelButtonController[4];
    [SerializeField] Image[] miniWheels = new Image[4]; //ȭ�� ���� �ϴܺκп� �ִ� �̴� ���� ��.
    [SerializeField] Sprite[] onSprite = new Sprite[4]; //Ȱ��ȭ�� �̹���
    [SerializeField] Sprite[] offSprite = new Sprite[4];    //��Ȱ��ȭ�� �̹���
    [SerializeField] RectTransform selectedWeaponSquare;    //Ȱ��ȭ�� ���� ǥ�� Image

    private bool wheelActivate;

    //private float[] button_Gauges = new float[4];
    

    private void Awake()
    {
        buttons = GetComponentsInChildren<WeaponWheelButtonController>();
    }



    public void ToggleWheel(bool activate)
    {
        //WheelActivate �� false�̰�  activate �� true �϶�  >> ����
        if(!wheelActivate && activate)
        {
            wheelActivate = true;
            animator.SetBool("OpenWeaponWheel", true);
            GameManager.Instance.playerManager.DisablePlayerShoot();

        }

        //WheelActivate ��  true �̰� activate �� false�϶� >> ����
        if (wheelActivate && !activate)
        {
            wheelActivate = false;
            animator.SetBool("OpenWeaponWheel", false);
            ShowItemNameText("");
            GameManager.Instance.playerManager.EnablePlayerShoot();

        }

    }   

    //������ Ȱ��ȭ
    public void ActivateItem(int ID, WeaponData wData)
    {
        //GameManager.Instance.playerManager.ChangeWeapon(ID);
        //selectedItem.sprite = wData.Icon;

        //ToggleWheel(false);

    }

    //������ ������ ��Ȱ��ȭ
    public void CancelItem()
    {
        selectedItem.sprite = noImage;
    }

    //������ �̸� 
    public void ShowItemNameText(string str)
    {
        itemText.text = str;
    }

    //���� ǥ�� ��ġ ����
    public void MoveSelectedSquarePosition(int index)
    {
        selectedWeaponSquare.anchoredPosition = miniWheels[index].rectTransform.anchoredPosition;
    }

    //ù ��ư ���� ��
    //public void SetItemData(WeaponInventory[] w_Inventory)
    //{
    //    //��ư Ȱ��ȭ ����
    //    for(int i = 0; i < w_Inventory.Length; i++)
    //    {
    //        buttons[i].SetData(w_Inventory[i].weaponData, w_Inventory[i].maxWeaponEnergy);


    //        //�̴��� ����
    //        onSprite[i] = w_Inventory[i].weaponData.WeaponEnableSprite;
    //        offSprite[i] = w_Inventory[i].weaponData.WeaponDisableSprite;
    //        miniWheels[i].sprite = offSprite[i];

    //        //Ȱ��ȭ ���ο� ���� UI ��ü
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

    //���� ������ ���� ������
    //public void UpdateWeaponGauge(WeaponInventory[] w_Inventory)
    //{
    //    float curEng;
    //    float maxEng;
    //    float value;
    //    //string valueName = "";

    //    for (int i = 0; i < w_Inventory.Length; i++)
    //    {
    //        //���� ��ư Ȱ��ȭ, ��Ȱ��ȭ
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
            
    //        //���� percent ������
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
