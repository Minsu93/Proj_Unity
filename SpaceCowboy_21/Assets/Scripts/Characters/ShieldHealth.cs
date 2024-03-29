using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHealth : Health
{
    /// <summary>
    /// ���� ������ �����ľ� �ϴ� ������ Health
    /// </summary>
    /// 

    public float maxShield;
    public float currShield { get; private set; }
    public override void ResetHealth()
    {
        base.ResetHealth();
        currShield = maxShield;
    }




}
