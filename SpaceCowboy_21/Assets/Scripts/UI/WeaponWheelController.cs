using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponWheelController : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Image selectedItem;
    [SerializeField] Sprite noImage;
    //public static int weaponID;
    [SerializeField] TextMeshProUGUI itemText;    //�ؽ�Ʈ ǥ�õǴ°�
    private bool wheelActivate;


    public void ToggleWheel(bool activate)
    {
        //WheelActivate �� false�̰�  activate �� true �϶�  >> ����
        if(!wheelActivate && activate)
        {
            wheelActivate = true;
            animator.SetBool("OpenWeaponWheel", true);
        }

        //WheelActivate ��  true �̰� activate �� false�϶� >> ����
        if(wheelActivate && !activate)
        {
            wheelActivate = false;
            animator.SetBool("OpenWeaponWheel", false);
            ShowItemNameText("");
        }

    }

    //������ Ȱ��ȭ
    public void ActivateItem(int ID, WeaponData wData)
    {
        GameManager.Instance.playerManager.playerBehavior.TryChangeWeapon(wData);
        selectedItem.sprite = wData.Icon;

        ToggleWheel(false);

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
}
