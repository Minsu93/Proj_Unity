using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

namespace SpaceEnemy
{
    [SelectionBase]

    public abstract class EnemyBrain : MonoBehaviour
    {
        public EnemyState enemyState = EnemyState.Sleep;
        public bool activate = true;    //�ֳʹ̰� Ȱ��ȭ �Ǿ����ϱ�?
        public bool doTotalRangeCheck;  //���� �����մϱ�? false�� ���� ��ü�� �ߴ�.
        public bool doAttackRangeCheck; //���� �Ÿ��� üũ�մϱ�?
        public bool doVisionCheck;      //�þ߸� üũ�մϱ�? 
        public bool doPlanetCheck;      //�༺�� üũ�մϱ�? 
        
        public float timeBetweenChecks = 0.5f;  //�÷��̾� ���� �ð� ����(����)
        public float limitRange;        //�Ѱ� �Ÿ�
        public float attackRange;       //���� �Ÿ�

        //����Ʈ
        public ParticleSystem hitEffect;    //�¾��� �� ȿ��
        public ParticleSystem deadEffect;   //�׾��� �� ȿ��


        //���ú���
        float lastCheckTime;
        Coroutine waitRoutine; //���� �ڷ�ƾ

        //��ũ��Ʈ
        protected Health health;
        protected EnemyAction action;
        protected CharacterGravity gravity;
        //protected CharacterGravity pGravity;
        protected DropItem dropitem;

        //����
        public float playerDistance { get; set; }     //�÷��̾���� �Ÿ��� ����ؼ� �ٸ� �ൿ�� �� �� �ֵ���.
        public Transform playerTr { get; set; }  //�÷��̾��� ��ġ
        public Vector2 playerDirection { get; set; }     //�÷��̾� ����
        
        //��������
        public bool inDetectRange;   //���� �Ÿ� �ȿ� �ִ�.
        public bool inAttackRange;  //���� ���� �ȿ� �ִ�. 
        public bool isVisible;    //�÷��̾ ���� ���̴°�
        public bool inOtherPlanet;     //�ٸ� �༺���� �߰��Ұ��ΰ�?
        public bool playerIsRight;      //�÷��̾ �����ʿ� �ֳ�?



        protected virtual void Awake()
        {
            health = GetComponent<Health>();
            action = GetComponent<EnemyAction>();
            gravity = GetComponent<CharacterGravity>();
            playerTr = GameManager.Instance.player;
            //pGravity = playerTr.GetComponent<CharacterGravity>();
            dropitem = GetComponent<DropItem>();


            if (health != null) health.ResetHealth();    //ü�� �ʱ�ȭ
        }


        protected void Start()
        {
            GameManager.Instance.PlayerDeadEvent += PlayerIsDead;   //�÷��̾ ���� ��� �����ϴ� �̺�Ʈ 
            
            Planet pp = gravity.nearestPlanet;
            if (pp != null)
            {
                pp.PlanetWakeUpEvent += WakeUp;
            }
        }


        protected virtual void Update()
        {
            if (!activate) return;

            PlayerPosUpdate();

            if (Time.time - lastCheckTime < timeBetweenChecks) return;
            lastCheckTime = Time.time;

            TotalRangeCheck();

            BrainStateChange();
        }

        //������ ��� 
        public void ResetEnemyBrain()
        {
            activate = true;
            enemyState = EnemyState.Sleep;

            health.ResetHealth();
            action.ResetAction();
        }

        //���� ������ ���� �ٸ��� ��Ȳ�� �����Ѵ�. 
        public abstract void BrainStateChange();

        #region Checks
        protected void PlayerPosUpdate()    
        {
            playerDistance = (playerTr.position - transform.position).magnitude;
            
            if (enemyState == EnemyState.Sleep) return;

            playerDirection = (playerTr.position - transform.position).normalized;
            playerIsRight = Vector2.SignedAngle(transform.up, playerDirection) <= 0;
        }

        protected void TotalRangeCheck()
        {
            if (doTotalRangeCheck)
            {
                if (playerDistance <= limitRange) inDetectRange = true;
                else inDetectRange = false;
            }

            if (enemyState == EnemyState.Sleep) return;

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

            //Vector2 playerVector = playerTr.position - transform.position;
            //Vector2 playerDirection = playerVector.normalized;
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, 0.3f, playerDirection, playerDistance, LayerMask.GetMask("Planet"));
            if (hit.collider != null) inVision = false;
            else inVision = true;

            return inVision;
        }
                

        public bool OnOtherPlanetCheck()
        {
            bool inOtherP = false;

            if (GameManager.Instance.playerNearestPlanet != gravity.nearestPlanet) inOtherP = true;
            inOtherPlanet = inOtherP;
            
            return inOtherP;
        }

        //protected bool FindAttackablePoint()
        //{
        //    bool findPoint = false;

        //    //���� �÷��̾ ���̴� Collider2D ���� ����Ʈ���� ���� �˾Ƴ���.
        //    List<int> visiblePoints = new List<int>();
        //    int pointCounts = gravity.nearestCollider.points.Length - 1;

        //    for (int i = 0; i < pointCounts; i++)
        //    {
        //        //���ݸ� ����. i�� ¦���� ���.
        //        if (i % 4 == 0)
        //            continue;

        //        //����Ʈ�� ��ġ�� �����´�
        //        Vector2 pointVector = GetPointPos(i);
        //        Vector2 edgeDirection = gravity.nearestCollider.points[i + 1] - gravity.nearestCollider.points[i];
        //        Vector2 normal = Vector2.Perpendicular(edgeDirection).normalized;
        //        Vector2 playerPos = playerTr.position;

        //        pointVector = pointVector + (normal * action.enemyHeight);
        //        Vector2 dir = playerPos - pointVector;
        //        float dist = dir.magnitude;
        //        dir = dir.normalized;

        //        //�����Ÿ� ������ ���鸸 üũ�Ѵ�
        //        if (dist > attackRange)
        //            continue;

        //        //��AI�� �� �ִ� �༺�� Point �߿��� �÷��̾ ���̴� Point �鸸 �̾Ƴ���. 
        //        RaycastHit2D hit = Physics2D.Raycast(pointVector, dir, dist, LayerMask.GetMask("Planet"));
        //        if (hit.collider == null)
        //        {
        //            findPoint = true;
        //            break;
        //        }
        //    }

        //    return findPoint;
        //}

        //Vector2 GetPointPos(int pointIndex)
        //{
        //    Vector3 localPoint = gravity.nearestCollider.points[pointIndex];
        //    Vector2 pointPos = gravity.nearestCollider.transform.TransformPoint(localPoint);
        //    return pointPos;
        //}

        #endregion

        public virtual void DamageEvent(float dmg)
        {
            if (enemyState == EnemyState.Die) return;

            if (health.AnyDamage(dmg))
            {
                //�´� ȿ�� 
                if(action != null) action.HitView();
                if (hitEffect != null) ParticleHolder.instance.GetParticle(hitEffect, transform.position, transform.rotation);

                AfterHitEvent();

                if (health.IsDead())
                {
                    StopAllCoroutines();
                    activate = false;
                    enemyState = EnemyState.Die;
                    
                    //if (dropitem != null) dropitem.GenerateItem();
                    //Debug.Log("Generate item!");
                    if (deadEffect != null) ParticleHolder.instance.GetParticle(deadEffect, transform.position, transform.rotation);

                    WhenDieEvent();
                }
            }
        }

        protected abstract void AfterHitEvent();
        protected abstract void WhenDieEvent();

        //�����, ��� Brain ���� Planet���� ��ȣ�� �޾Ƽ� �����. 
        public virtual void WakeUp()
        {
            if(enemyState == EnemyState.Sleep)
            {
                ChangeState(EnemyState.Chase, 0f);
                action.WakeUpEvent();
            }
        }

        #region Delaied State Change
        //������ �ֱ�
        public void ChangeState(EnemyState state, float time)
        {
            if (enemyState == EnemyState.Die)
                return;
            
            //�ڷ�ƾ �ߺ��� ���� ����
            if (waitRoutine == null)
            {
                if(time == 0f)
                {
                    enemyState = state;
                    return;
                }

                enemyState = EnemyState.Wait;

                waitRoutine = StartCoroutine(StateChangeDelay(state, time));
            }
        }

        IEnumerator StateChangeDelay(EnemyState state, float time)
        {
            yield return new WaitForSeconds(time);

            waitRoutine = null;
            enemyState = state;
        }

        #endregion

        //�÷��̾ ���� ��� �ٽ� ����. 
        public void PlayerIsDead()
        {
            if (enemyState == EnemyState.Die) return;
            enemyState = EnemyState.Sleep;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, limitRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

    }


    public enum EnemyState
    {
        Sleep, 
        Chase,
        Attack, 
        Die,
        Wait
    }


}


