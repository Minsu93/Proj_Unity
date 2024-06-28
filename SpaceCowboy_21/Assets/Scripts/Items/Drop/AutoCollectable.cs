using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class AutoCollectable : Collectable
{
    protected CircleCollider2D physicsCollider;

    public int amount = 1;
    public float timeToAutoCollect; //�ڵ� �������� �ɸ��� �ð�
    public float collectDuration;  //���� �ӵ�. 

    float _easeBaseSpeed = 1.5F; //initial acceleration

    float timer;
    float collectTimer;


    protected override void Awake()
    {
        base.Awake();
        physicsCollider = GetComponentsInChildren<CircleCollider2D>()[1];

    }

    protected override void OnEnable()
    {
        physicsCollider.enabled = true;
        gravity.activate = true;
        timer = 0f;
        collectTimer = 0f;
    }

    private void Update()
    {
        if(timer < timeToAutoCollect)
        {
            timer += Time.deltaTime;
            if(timer >= timeToAutoCollect)
            {
                physicsCollider.enabled = false;
                gravity.activate = false;
            }
        }
        else
        {
            AutoCollect();
        }
    }


    //�÷��̾� ��ġ�� �ڵ����� ���ư��� ����. 
    void AutoCollect()
    {
        float lerp = EaseOut(0, 1, ref collectTimer, collectDuration);
        lerp = Mathf.Clamp01(lerp);
        Vector2 _dest = GameManager.Instance.player.position;
        Vector2 _from = rb.position;
        Vector2 _pos = Vector2.Lerp(_from, _dest, lerp);
        
        rb.MovePosition(_pos);
    }

    float EaseOut(float _from, float _dest, ref float _value, float _duration)
    {
        float _result = _from + ((_dest - _from) * _value);
        float _accel = ((2F - (2F * _value)) + (((_value * 2F) - 1F) * (2F - _easeBaseSpeed)));

        _value += Time.deltaTime * ((Mathf.Abs(_dest - _from)) / _duration) * _accel;


        return _result;
    }

    float EaseIn(float _from, float _dest, ref float _value, float _duration)
    {

        float _result = _from + ((_dest - _from) * _value);
        float _accel = ((_value * 2F) + ((1F - (_value * 2F)) * (2F - _easeBaseSpeed)));

        _value += Time.deltaTime * ((Mathf.Abs(_dest - _from)) / _duration) * _accel;

        return _result;


    }

    protected override void ConsumeEffect()
    {
        throw new System.NotImplementedException();
    }
}
