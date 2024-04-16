using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyChase : MonoBehaviour
{
    /// 
    /// 추격 속도 관련 변수들. 
    /// 추격 실행 함수
    ///

    [Header("Move Property")]
    public float moveSpeed = 5f;
    public bool faceRight { get; set; }  //캐릭터가 오른쪽을 보고 있습니까? 


    //스크립트
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
