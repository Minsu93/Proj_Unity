using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;


[SelectionBase]

public class EnemyBrain : MonoBehaviour
{
    [SerializeField] bool doAttackRangeCheck = true; //���� �Ÿ��� üũ�մϱ�?
    [SerializeField] bool doVisionCheck = true;      //�þ߸� üũ�մϱ�? 
    [SerializeField] bool doPlanetCheck = true;      //�༺�� üũ�մϱ�? 

    [SerializeField] float timeBetweenChecks = 0.5f;  //�÷��̾� ���� �ð� ����(����)
    [SerializeField] float attackRange = 10f;       //���� �Ÿ�

    //���ú���
    float lastCheckTime;

    //��ũ��Ʈ
    protected EnemyAction action;
    protected CharacterGravity gravity;


    //��������
    public float playerDistance { get; private set; }     //�÷��̾���� �Ÿ��� ����ؼ� �ٸ� �ൿ�� �� �� �ֵ���.
    public Vector2 playerDirection { get; private set; }     //�÷��̾� ����

    public bool inAttackRange { get; private set; }   //���� ���� �ȿ� �ִ�. 
    public bool isVisible { get; private set; }    //�÷��̾ ���� ���̴°�
    public bool inOtherPlanet { get; private set; } //�ٸ� �༺���� �߰��Ұ��ΰ�?
    public bool playerIsRight { get; private set; }      //�÷��̾ �����ʿ� �ֳ�?



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





