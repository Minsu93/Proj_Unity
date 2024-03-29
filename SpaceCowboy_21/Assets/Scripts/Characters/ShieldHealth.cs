using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHealth : Health
{
    /// <summary>
    /// 별의 힘으로 물리쳐야 하는 적들의 Health
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
