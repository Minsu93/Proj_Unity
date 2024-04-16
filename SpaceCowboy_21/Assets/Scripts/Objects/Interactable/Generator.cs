using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : InteractableOBJ
{

    public List<GameObject> switchList = new List<GameObject>();
    public Sprite turnOnSprite;


    public override void InteractAction()
    {
        if (!interactOn) return;

        //�ڽ�Ʈ�� �ִ��� �˻��ϰ�
        if (ResourceManager.Instance.PayMoney(interactCost))
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
        foreach (GameObject obj in switchList)
        {
            ISwitchable switchObj = obj.GetComponent<ISwitchable>();
            switchObj.ActivateObject();
        }

        if (turnOnSprite != null) spr.sprite = turnOnSprite;
    }


}
