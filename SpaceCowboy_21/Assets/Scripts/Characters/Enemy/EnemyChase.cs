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
    public bool faceRight { get; set; }  //ĳ���Ͱ� �������� ���� �ֽ��ϱ�? 


    //��ũ��Ʈ
    protected Rigidbody2D rb;
    protected EnemyBrain brain;
    //protected EnemyAction action;
    protected CharacterGravity charGravity;
    protected Planet curPlanet;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        brain = GetComponent<EnemyBrain>();
        //action = GetComponent<EnemyAction>();
        charGravity = GetComponent<CharacterGravity>();
    }

    public abstract void OnChaseAction();

}
