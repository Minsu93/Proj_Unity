using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class GoldObject : InteractableOBJ
{
    [SerializeField] int price;
    [SerializeField] protected TextMeshProUGUI goldText;
    [SerializeField] protected Transform goldTransform;
    [SerializeField] protected GameObject triggerObj;
    [SerializeField] protected Collider2D kickColl;

    protected virtual void Awake()
    {
        triggerObj.SetActive(true);
        kickColl.enabled = false;
        goldTransform.gameObject.SetActive(true);
        goldText.text = price.ToString();
    }

    public override void InteractAction()
    {
        if(GameManager.Instance.materialManager.PayMoney("gold", price))
        {
            Debug.Log("������Ʈ Ȱ��ȭ");
            ObjectActivate();
        }
    }

    /// <summary>
    /// ������Ʈ�� ������ �������� �� �������� �͵�.
    /// 1. ������ �� ��ǰ ����
    /// 2. ������ �� ���� �۵�
    /// 3. ������ �� ...
    /// </summary>
    /// 
    protected abstract void ObjectActivate();

    protected abstract void ObjectDeactivate();
}
