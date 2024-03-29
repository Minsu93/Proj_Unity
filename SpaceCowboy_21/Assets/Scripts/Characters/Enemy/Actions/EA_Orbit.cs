using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class EA_Orbit : EnemyAction
{

    [Header("Orbit")]
    public bool moveRight;          //ȸ�� ����
    public bool pauseOnAttackMode;  //���ݽ� ����.
    public float pauseTimer = 0.1f; //�ǰ� �� ���� �ð�

    [Header("Orbit Attack")]
    public int burstNumber = 3;
    public float burstDelay = 0.5f;
    
    //����
    float moveSpeedMultiplier = 1f; //�� �߰� �� ������ ������ 
    float moveSpd = 0f;
    float pTime;
    protected int direction;
    bool moveUpdate = true;
    Vector3 center;
    
    //��ƼŬ
    public ParticleSystem boosterParticle;

    //��ũ��Ʈ
    AttachToPlanet attachToPlanet;
    EV_OrbitCannon enemyview_s;
    
    //�ӽ�, ȸ�� �뵵
    public GameObject ViewObj;

    protected override void Awake()
    {
        base.Awake();

        enemyview_s = GetComponentInChildren<EV_OrbitCannon>();
        attachToPlanet = GetComponent<AttachToPlanet>();
    }

    protected void Start()
    {
        center = attachToPlanet.coll.transform.position;
    }

    private void FixedUpdate()
    {
        //���� �ð�
        if(pTime > 0)
        {
            pTime -= Time.deltaTime;
            return;
        }

        //����
        if (moveUpdate)
        {
            //�ӵ��� 1���� ���� ��� �ӵ��� 1�� ���δ�
            if (moveSpd < 1)
            {
                moveSpd += Time.deltaTime * 10f;
                if (moveSpd >= 1) moveSpd = 1;
            }
        }
        else
        {
            //�ӵ��� 0���� Ŭ ��� �ӵ��� 0���� �����
            if (moveSpd > 0)
            {
                moveSpd -= Time.deltaTime * 10f;
                if(moveSpd <= 0) { moveSpd = 0; }
            }
        }

        if(moveSpd > 0)
            transform.RotateAround(center, Vector3.forward, direction * moveSpeed * moveSpeedMultiplier * Time.deltaTime);
    }


    protected override void OnChaseAction()
    {
        return;
    }

    protected override void OnAttackAction()
    {
        attackCool = true;
        StartCoroutine(AttackCoroutine());
    }

    protected virtual IEnumerator AttackCoroutine()
    {
        yield return StartCoroutine(DelayRoutine(preAttackDelay));

        int burst = burstNumber;
        //if (enemyview_s != null)
        //{
        //    var guntip = enemyview_s.GetGunTipPos();
        //    ShootAction(guntip.Item1, guntip.Item2, 0);
        //}

        while (burst > 0)
        {
            burst--;

            Vector2 pos = transform.position;
            Vector2 vec = Quaternion.Euler(0, 0, 90) * brain.playerDirection;
            Quaternion rot = Quaternion.LookRotation(Vector3.forward, vec);

            ShootAction(pos, rot, 0);

            yield return new WaitForSeconds(burstDelay);
        }
    }




    public override void HitView()
    {
        //�ִϸ��̼� �̺�Ʈ
        base.HitView();

        //�ణ ����? 
        pTime = pauseTimer;
    }
    


    public override void WakeUpEvent()
    {
        base.WakeUpEvent();

        //�����̱� ���� 
        moveUpdate = true;
        if (boosterParticle != null) boosterParticle.Play();
        ChangeDirectionToRight(moveRight);
    }

    protected override void OnDieAction()
    {
        base.OnDieAction();

        moveUpdate = false;
        if (boosterParticle != null) boosterParticle.Stop();

    }

    public virtual void ChangeDirectionToRight(bool right)
    {

        faceRight = right ? true : false;
        FlipToDirectionView();

        direction = right ? -1 : 1;


    }

}
