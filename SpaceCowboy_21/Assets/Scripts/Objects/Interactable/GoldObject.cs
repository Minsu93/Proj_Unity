using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoldObject : InteractableOBJ
{
    [SerializeField] int price;

    public override void InteractAction()
    {
        if(GameManager.Instance.materialManager.PayMoney("gold", price))
        {
            ObjectActivate();
        }
    }

    /// <summary>
    /// 오브젝트에 가격을 지불했을 때 벌어지는 것들.
    /// 1. 발차기 시 물품 생성
    /// 2. 발차기 시 함정 작동
    /// 3. 발차기 시 ...
    /// </summary>
    protected abstract void ObjectActivate();
}
