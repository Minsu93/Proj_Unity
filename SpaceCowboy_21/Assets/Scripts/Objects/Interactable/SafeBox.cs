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

        //코스트가 있는지 검사하고
        if (GameManager.Instance.materialManager.PayMoney("gold", interactCost))
        {
            //TurnOn을 시작한다.
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
        //아이템을 드롭한다
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
