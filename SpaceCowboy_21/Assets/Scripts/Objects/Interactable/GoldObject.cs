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
            Debug.Log("오브젝트 활성화");
            ObjectActivate();
        }
    }

    /// <summary>
    /// 오브젝트에 가격을 지불했을 때 벌어지는 것들.
    /// 1. 발차기 시 물품 생성
    /// 2. 발차기 시 함정 작동
    /// 3. 발차기 시 ...
    /// </summary>
    /// 
    protected abstract void ObjectActivate();

    protected abstract void ObjectDeactivate();
}
