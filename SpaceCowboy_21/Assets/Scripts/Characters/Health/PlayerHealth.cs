using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //public HealthState healthState = HealthState.Default;

    public float maxHealth = 10;
    public float currHealth {  get; private set; }

    //public float maxShield = 10;
    //public float currShield { get; private set; }
    //public float delayToRegeneration = 1.0f;
    //public float dTimer { get; private set; }
    //public float regenerateTime = 2.0f;
    //public float regenSpeed = 3.0f;

    //���� �ð�
    [SerializeField] float InvincibleTime;
    float _invincibleTimer;


    //������ ������ �������� �� �ִ�. 
    //���� ����, �Ӽ� ����

    public void ResetHealth()
    {
        currHealth = maxHealth;
        //currShield = maxShield;
        //healthState = HealthState.Default;
        GameManager.Instance.playerManager.UpdatePlayerStatusUI();

    }

    public bool AnyDamage(float dmg)
    {   //�¾Ƽ� �������� �Ծ����� �˻�
        bool isHit = false;

        if(currHealth ==0)
        {
            return isHit;
        }

        if (_invincibleTimer > 0)
        {
            return isHit;
        }


        //���� �ð� �ʱ�ȭ
        _invincibleTimer = InvincibleTime;

        //���ظ� �Դ´�
        //if(currShield > 0)
        //{
        //    currShield -= dmg;
        //    if (currShield < 0) currShield = 0;
        //}
        //else if (currHealth > 0) 
        //{
        //    currHealth -= dmg;
        //    if (currHealth < 0) currHealth = 0;
        //}
        if (currHealth > 0)
        {
            currHealth -= dmg;
            if (currHealth < 0) currHealth = 0;
        }

        isHit = true;

        //�÷��̾� Status UI �� ������Ʈ�Ѵ�.
        GameManager.Instance.playerManager.UpdatePlayerStatusUI();

        return isHit;
    }

    //public void ShieldUp(float amount)
    //{
    //    if (currHealth == 0)
    //    {
    //        return;
    //    }

    //    if (currShield < maxShield)
    //    {
    //        //�ǵ带 ȸ���Ѵ�
    //        currShield += amount;

    //        if (currShield >= maxShield) currShield = maxShield;
    //    }
    //}
    public bool HealthUp(float amount)
    {
        if (currHealth == 0)
            return false;

        if (currHealth < maxHealth)
        {
            currHealth += amount;
            if (currHealth >= maxHealth) currHealth = maxHealth;
            return true;
        }
        else return false;

    }

    public bool IsDead()
    {   //�׾����� �˻�
        bool isDead = false;


        if (currHealth <= 0)
        {
            isDead = true;
        }
        else
        {
            isDead = false;
        }

        return isDead;
    }

    protected virtual void Update()
    {
        if (_invincibleTimer > 0)
        {
            _invincibleTimer -= Time.deltaTime;
        }


       //switch(healthState)
       // {
       //     case HealthState.Default:
       //         if (currHealth < maxHealth) healthState = HealthState.OnHit;
       //         break;

       //     case HealthState.OnHit:

       //         //������ �����̵��� ��ٸ��� OnRegen���·� �Ѿ��
       //         dTimer += Time.deltaTime / delayToRegeneration;
       //         if (dTimer >= 1)
       //         {
       //             dTimer = 0;
       //             currShield = maxShield;
       //             healthState = HealthState.Default;
       //         }
       //         break;
       //     //case HealthState.OnRegen: 
       //     //    if(currShield < maxShield)
       //     //    {

       //     //        //�ǵ带 ȸ���Ѵ�
       //     //        currShield += regenSpeed * Time.deltaTime;

       //     //        //�ǵ尡 �� á���� �⺻ ���·� ���ư���
       //     //        if (currShield >= maxShield)
       //     //        {
       //     //            currShield = maxShield;
       //     //            healthState = HealthState.Default;
       //     //        }
       //     //    }
       //     //    break;
       //     case HealthState.Dead: break;
       // }
    }


    //public enum HealthState { Default, OnHit, OnRegen, Dead}

}
