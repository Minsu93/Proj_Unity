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

    //스크립트
    protected Rigidbody2D rb;
    protected Planet curPlanet;


    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public abstract void OnChaseAction();


}
