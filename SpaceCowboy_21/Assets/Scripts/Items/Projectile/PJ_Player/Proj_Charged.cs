using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_Charged : Projectile_Player
{
    /// 충전식 투사체는 충전 정도에 따라 투사체의 모양,그리고 사이즈가 바뀐다. 
    /// 

    //단계에 따른 데미지 비율
    [SerializeField] private EnhancePerCharge[] enhancePerCharges = new EnhancePerCharge[3];

    CircleCollider2D circleColl;
    float defaultCollRadius;
    Vector3 defaultViewObjScale;

    protected override void Awake()
    {
        base.Awake();
        circleColl = coll as CircleCollider2D;
        if(circleColl != null) defaultCollRadius = circleColl.radius;
        defaultViewObjScale = ViewObj.transform.localScale;
    }


    /// <summary>
    /// 충전 정도에 따라 다른 발사를 한다. 기본적으로는 범위 증가, vieObj 크기 증가, 데미지 증가. 
    /// </summary>
    /// <param name="power"></param>
    public void InitCharge(float power)
    {
        if (circleColl == null) return;
        //power는 1~3까지 전달해져오기 때문에 순서를 위해 -1을 해줌.
        int pow = Mathf.FloorToInt(power) - 1 ;
        EnhancePerCharge epc = enhancePerCharges[pow];
        
        circleColl.radius = defaultCollRadius * epc.radius;
        ViewObj.transform.localScale = defaultViewObjScale * epc.scale;
        this.damage *= epc.damage;
    }
}

[Serializable]
public class EnhancePerCharge
{
    public float damage;
    public float radius;
    public float scale;
}
