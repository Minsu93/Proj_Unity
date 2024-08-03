using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proj_Charged : Projectile_Player
{
    /// ������ ����ü�� ���� ������ ���� ����ü�� ���,�׸��� ����� �ٲ��. 
    /// 

    //�ܰ迡 ���� ������ ����
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
    /// ���� ������ ���� �ٸ� �߻縦 �Ѵ�. �⺻�����δ� ���� ����, vieObj ũ�� ����, ������ ����. 
    /// </summary>
    /// <param name="power"></param>
    public void InitCharge(float power)
    {
        if (circleColl == null) return;
        //power�� 1~3���� ������������ ������ ������ ���� -1�� ����.
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
