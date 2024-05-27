using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeBox : InteractableOBJ, ISwitchable
{
    public Sprite turnOnSprite;
    public Sprite openSprite;

    DropItem dropItem;
    protected override void Awake()
    {
        base.Awake();
        dropItem = GetComponent<DropItem>();
    }

    void Start()
    {
        if (interactable) ActivateObject();
    }

    public override void InteractAction()
    {
        if (!interactOn) return;

        //�ڽ�Ʈ�� �ִ��� �˻��ϰ�
        if (GameManager.Instance.materialManager.PayMoney("gold", interactCost))
        {
            //TurnOn�� �����Ѵ�.
            turnOnStart = true;
            turnOnTimer = 0f;

            circleColl.radius = 5f;

            RemoveOutLine();
            objUI.TextOn(false);
            objUI.GaugeOn(true);
        }
    }

    public override void turnOnAction()
    {
        //�������� ����Ѵ�
        dropItem.GenerateItem();

        if (openSprite != null) spr.sprite = openSprite;
    }

    public void ActivateObject()
    {
        interactable = true;
        if (turnOnSprite != null) spr.sprite = turnOnSprite;
    }

    public void DeactivateObject()
    {
        interactable = false;
    }
}
