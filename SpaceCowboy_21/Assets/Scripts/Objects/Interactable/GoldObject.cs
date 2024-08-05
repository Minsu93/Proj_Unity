using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GoldObject : InteractableOBJ
{
    [SerializeField] int price;
    [SerializeField] protected GameObject collObj;



    public override void InteractAction()
    {
        if(GameManager.Instance.materialManager.PayMoney("gold", price))
        {
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
