using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    /// <summary>
    /// ��ֹ� - ���߿� �� �ִ� ���, Ȥ�� �༺�� �پ��ִ� �ϼ���.
    /// �Ѵ� �ѿ� �¾Ƽ� ü���� ��� �μ�����. 
    /// ��� ���߿��� �����ϸ�, �༺�̳� ���� ���� �ε����� �ݴ�������� �з�����. 
    /// �Ѿ˿� ������ ��¦ ������ �ٲ��. �ӵ��� maxġ�� ���� �ʴ´�. 
    /// </summary>
    /// 

    public Type type = Type.Asteroid;

    public float moveSpeed = 1.0f;
    public float rotationSpeed = 1.0f;

    Health health;
    protected Rigidbody2D rb;
    protected Collider2D coll;

    private void Awake()
    {
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    protected void Start()
    {
        // �������ڸ��� ���� �������� �ӵ�, ȸ���� �ش�. 
        health.ResetHealth();
        
        //������ 1�� ���� ����
        int degree = Random.Range(0, 360);
        float x = Mathf.Cos(degree * Mathf.Deg2Rad);
        float y = Mathf.Sin(degree * Mathf.Deg2Rad);    
        Vector2 UnitVec = new Vector2(x, y);

        //-2 ~ -1, 1~2 ������ ���� ȸ����
        float torque;
        float random = Random.Range(0, 100);
        if(random > 50)
        {
            torque = Random.Range(-2.0f, -1.0f);
        }
        else
        {
            torque = Random.Range(1.0f, 2.0f);
        }

        //rb.AddForce(UnitVec * moveSpeed, ForceMode2D.Impulse);
        rb.AddTorque(torque * rotationSpeed, ForceMode2D.Impulse);
    }


    //������ �¾��� �� 
    public void AnyDamage(float dmg, Vector2 velocity)
    {
        if (health.AnyDamage(dmg))
        {
            if (health.IsDead())
            {
                //�׾����� ����? 
                DestroyEvent();
            }
            else
            {
                //�������� �Ծ����� Hit ����Ʈ
                //�з������ұ�...?
                HitEvent();

                rb.AddForce(velocity * dmg * moveSpeed, ForceMode2D.Impulse);
            }
        }

    }

    //�������� �Ծ��� �� �̺�Ʈ
    protected abstract void HitEvent();

    //�ı��Ǿ��� �� �̺�Ʈ
    protected abstract void DestroyEvent();





    [ContextMenu("����ȭ ����")]
    public void SizeRandomize()
    {
        //�������� ����ȭ
        float scale = Random.Range(0.3f, 1f);
        transform.localScale = Vector3.one * scale;

        //Asteroid �� ��� ������ ����ȭ
        if(type == Type.Asteroid)
        {
            int degree = Random.Range(0, 360);
            transform.rotation = Quaternion.Euler(0, 0, degree);
        }
    }
}

public enum Type { Asteroid, Rock}
