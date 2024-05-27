using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;


[SelectionBase]

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] bool doAttackRangeCheck = true; //공격 거리를 체크합니까?
    [SerializeField] bool doVisionCheck = true;      //시야를 체크합니까? 
    [SerializeField] bool doPlanetCheck = true;      //행성을 체크합니까? 

    [SerializeField] float timeBetweenChecks = 0.5f;  //플레이어 감지 시간 간격(고정)
    [SerializeField] float attackRange = 10f;       //공격 거리

    //로컬변수
    float lastCheckTime;

    //스크립트
    protected EnemyAction action;
    protected CharacterGravity gravity;


    //로컬참조
    public float playerDistance { get; private set; }     //플레이어와의 거리를 기록해서 다른 행동을 할 수 있도록.
    public Vector2 playerDirection { get; private set; }     //플레이어 방향

    public bool inAttackRange { get; private set; }   //공격 범위 안에 있다. 
    public bool isVisible { get; private set; }    //플레이어가 눈에 보이는가
    public bool inOtherPlanet { get; private set; } //다른 행성으로 추격할것인가?
    public bool playerIsRight { get; private set; }      //플레이어가 오른쪽에 있나?



    protected virtual void Awake()
    {
        action = GetComponent<EnemyAction>();
        gravity = GetComponent<CharacterGravity>();

    }


    #region Checks

    public void TotalCheck()
    {
        if (Time.time - lastCheckTime < timeBetweenChecks) return;
        lastCheckTime = Time.time;

        Transform playerTr = GameManager.Instance.player;
        playerDistance = (playerTr.position - transform.position).magnitude;
        playerDirection = (playerTr.position - transform.position).normalized;
        playerIsRight = Vector2.SignedAngle(transform.up, playerDirection) <= 0;


        if (doAttackRangeCheck)
        {
            if (playerDistance <= attackRange) inAttackRange = true;
            else inAttackRange = false;
        }

        if (doVisionCheck)
        {
            if (VisionCheck()) isVisible = true;
            else isVisible = false;
        }

        if (doPlanetCheck)
        {
            OnOtherPlanetCheck();
        }
    }

    protected bool VisionCheck()
    {
        bool inVision;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.3f, playerDirection, playerDistance, LayerMask.GetMask("Planet"));
        if (hit.collider != null) inVision = false;
        else inVision = true;

        return inVision;
    }


    public bool OnOtherPlanetCheck()
    {
        bool inOtherP = false;

        if (GameManager.Instance.playerManager.playerNearestPlanet != gravity.nearestPlanet) inOtherP = true;
        inOtherPlanet = inOtherP;

        return inOtherP;
    }
    #endregion





    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

}





