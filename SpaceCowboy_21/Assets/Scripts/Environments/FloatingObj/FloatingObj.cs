using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObj : MonoBehaviour
{
    [SerializeField] bool isDamagable;
    [SerializeField] float mass;
    public bool IsDamagable { get { return isDamagable; }}

    Health health;
    Rigidbody2D rb;

    private void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void DamageEvent(float dmg)
    {
        //데미지를 적용
        health.AnyDamage(dmg);

        if (health.IsDead())
        {
            //true 인 경우 체력이 0 이하로 떨어졌다는 뜻.
            DeathEvent();
            return;
        }
    }

    public void KnockBackEvent(Vector2 force)
    {
        rb.AddForce(force / mass, ForceMode2D.Impulse);
    }

    void DeathEvent()
    {
        //임시로 활성화를 종료한다.
        gameObject.SetActive(false);
    }



}
