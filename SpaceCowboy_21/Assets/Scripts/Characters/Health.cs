using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Health : MonoBehaviour
{
    [Header("Health Properties")]
    public float maxHealth = 10;
    public float currHealth { get; set; }    //Ȯ�ο�

    public float InvincibleTime;
    float _invincibleTimer;
   

    //������ ������ �������� �� �ִ�. 
    //���� ����, �Ӽ� ����

    public void ResetHealth()
    {
        currHealth = maxHealth;
    }

    public bool AnyDamage(float dmg)
    {   //�¾Ƽ� �������� �Ծ����� �˻�
        bool isHit = false;

        if (_invincibleTimer > 0)
        {
            return isHit;
        }
            

        //���� �ð� �ʱ�ȭ
        _invincibleTimer = InvincibleTime;

        //���ظ� �Դ´�
        currHealth -= dmg;
        isHit = true;


        if (currHealth <= 0)
        {
            currHealth = 0;
        }

        return isHit;
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
        if(_invincibleTimer > 0)
        {
            _invincibleTimer -= Time.deltaTime;
        }
    }

}
