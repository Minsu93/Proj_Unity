using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Obstacle : MonoBehaviour
{
    /// <summary>
    /// 장애물 - 공중에 떠 있는 운석형, 혹은 행성에 붙어있는 암석형.
    /// 둘다 총에 맞아서 체력이 닳고 부서진다. 
    /// 운석은 공중에서 유영하며, 행성이나 우주 벽에 부딪히면 반대방향으로 밀려난다. 
    /// 총알에 맞으면 살짝 방향이 바뀐다. 속도는 max치를 넘지 않는다. 
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
        // 시작하자마자 랜덤 방향으로 속도, 회전을 준다. 
        health.ResetHealth();
        
        //반지름 1의 랜덤 벡터
        int degree = Random.Range(0, 360);
        float x = Mathf.Cos(degree * Mathf.Deg2Rad);
        float y = Mathf.Sin(degree * Mathf.Deg2Rad);    
        Vector2 UnitVec = new Vector2(x, y);

        //-2 ~ -1, 1~2 사이의 랜덤 회전값
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


    //뭔가를 맞았을 때 
    public void AnyDamage(float dmg, Vector2 velocity)
    {
        if (health.AnyDamage(dmg))
        {
            if (health.IsDead())
            {
                //죽었으면 폭발? 
                DestroyEvent();
            }
            else
            {
                //데미지를 입었으면 Hit 이펙트
                //밀려나야할까...?
                HitEvent();

                rb.AddForce(velocity * dmg * moveSpeed, ForceMode2D.Impulse);
            }
        }

    }

    //데미지를 입었을 때 이벤트
    protected abstract void HitEvent();

    //파괴되었을 때 이벤트
    protected abstract void DestroyEvent();





    [ContextMenu("랜덤화 실행")]
    public void SizeRandomize()
    {
        //스케일을 랜덤화
        float scale = Random.Range(0.3f, 1f);
        transform.localScale = Vector3.one * scale;

        //Asteroid 인 경우 방향을 랜덤화
        if(type == Type.Asteroid)
        {
            int degree = Random.Range(0, 360);
            transform.rotation = Quaternion.Euler(0, 0, degree);
        }
    }
}

public enum Type { Asteroid, Rock}
