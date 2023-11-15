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
        //�������� ����
        health.AnyDamage(dmg);

        if (health.IsDead())
        {
            //true �� ��� ü���� 0 ���Ϸ� �������ٴ� ��.
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
        //�ӽ÷� Ȱ��ȭ�� �����Ѵ�.
        gameObject.SetActive(false);
    }



}
