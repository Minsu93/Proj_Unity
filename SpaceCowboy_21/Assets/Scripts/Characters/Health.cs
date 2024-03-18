using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Health : MonoBehaviour
{
    [Header("Health Properties")]
    public float maxHealth = 10;
    public float currHealth { get; set; }    //확인용

    public float InvincibleTime;
    float _invincibleTimer;
   

    //데미지 공식이 복잡해질 수 있다. 
    //방어력 관련, 속성 관련

    public void ResetHealth()
    {
        currHealth = maxHealth;
    }

    public bool AnyDamage(float dmg)
    {   //맞아서 데미지를 입었는지 검사
        bool isHit = false;

        if (_invincibleTimer > 0)
        {
            return isHit;
        }
            

        //무적 시간 초기화
        _invincibleTimer = InvincibleTime;

        //피해를 입는다
        currHealth -= dmg;
        isHit = true;


        if (currHealth <= 0)
        {
            currHealth = 0;
        }

        return isHit;
    }

    public bool IsDead()
    {   //죽었는지 검사
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
