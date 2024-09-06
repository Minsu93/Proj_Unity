using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyChase : MonoBehaviour
{
    /// 
    /// �߰� �ӵ� ���� ������. 
    /// �߰� ���� �Լ�
    ///

    [Header("Move Property")]
    public float moveSpeed = 5f;

    //��ũ��Ʈ
    protected Rigidbody2D rb;
    protected Planet curPlanet;
    protected CharacterGravity charGravity;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        charGravity = GetComponent<CharacterGravity>();
    }

    public abstract void OnChaseAction();


}
