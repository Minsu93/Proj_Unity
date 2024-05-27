using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Projectile_Bomb : Projectile
{
    //��ź�� ���� ���ϴ� ��������, ������ �Ÿ� ��ŭ ���ư���. �ϴ��� ������ ������ �; ������ �Ÿ���ŭ ���ư���. 
    //��ź�� ������ ������ ������ ������ ��, ���𰡿� �ε����� ���̴�. 
    //��ź�� ������ ���� Ư���� ȿ���� �����ϴ� ��ź���� �ִ�. 

    [SerializeField] BombType type = BombType.Instant;
    [SerializeField] GameObject usingBomb; //�÷��̾ ����� ��ô ���� >> orb

    PM_P_Bomb bombMovement;

    protected override void Awake()
    {
        base.Awake();
        bombMovement = GetComponent<PM_P_Bomb>();
    }

    public override void Init(float damage, float speed, float lifeTime, float distance, int penetrate, int reflect, int guide)
    {
        this.damage = damage;
        this.speed = speed;
        this.lifeTime = lifeTime;
        this.distance = distance;
        this.penetrateCount = penetrate;
        this.reflectCount = reflect;
        this.guideAmount = guide;

        if (lifeTime > 0) { lifeLimitProj = true; }
        else { lifeLimitProj = false; }

        ResetProjectile();
        bombMovement.StartMovement(speed,distance);
        bombMovement.BombExplodeEvent -= Explode;
        bombMovement.BombExplodeEvent += Explode;
    }


    //��ź�� ������ ȿ�� 
    public virtual void Explode(Vector2 pos)
    {
        switch (type)
        {
            case BombType.Instant:
                //���� ����
                Debug.Log("Bomb!");
                break;

            case BombType.Bubble:
                //���� �ڸ��� ���� ����.
                GameObject newOrb = GameManager.Instance.poolManager.GetItem(usingBomb);
                newOrb.transform.position = pos;
                newOrb.transform.rotation = Quaternion.identity;
                break;
        }

    }


    protected override void HitEvent(ITarget target, IHitable hitable)
    {

        Explode(transform.position);

        AfterHitEvent();
    }

    protected override void NonHitEvent(ITarget target)
    {
        Explode(transform.position);

        AfterHitEvent();
    }

    protected override void LifeOver()
    {
        Explode(transform.position);

        AfterHitEvent();
    }

}

public enum BombType
{
    Instant,
    Bubble
}