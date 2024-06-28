using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDrop : AutoCollectable
{

    protected override void ConsumeEffect()
    {
        GameManager.Instance.materialManager.AddMoney("gold", amount);
    }
}
