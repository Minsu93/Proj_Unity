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
        public EnemyState enemyState = EnemyState.Idle;
        public bool activate = true;    //�ֳʹ̰� Ȱ��ȭ �Ǿ����ϱ�?

        [Space]

        public float timeBetweenChecks = 0.5f;  //�÷��̾� ���� �ð� ����(����)
        public float checkRange;        //���� �Ÿ�
        public float attackRange;       //���� �Ÿ�
        public float minRange;          //�ּ� �Ÿ� 

        float lastCheckTime;
        public float playerDistance { get; set; }     //�÷��̾���� �Ÿ��� ����ؼ� �ٸ� �ൿ�� �� �� �ֵ���.
        public Transform playerTr { get; set; }  //�÷��̾��� ��ġ

        protected Health health;
        protected EnemyAction action;
        protected CharacterGravity gravity;

        //����� ��

        public bool inChaseRange;   //�߰� ���� �ȿ� �ִ� (�����Ÿ� ~  �ּҰŸ� ����)
        public bool inAttackRange;  //���� ���� �ȿ� �ִ�. 
        public bool playerIsVisible;    //�÷��̾ ���� ���̴°�
        public bool playerIsInOtherPlanet;     //�ٸ� �༺���� �߰��Ұ��ΰ�?

        Coroutine waitRoutine; //���� �ڷ�ƾ


        private void Awake()
        {
            if (health == null)
                health = GetComponent<Health>();
            health.ResetHealth();

            if (action == null)
                action = GetComponent<EnemyAction>();

            gravity = GetComponent<CharacterGravity>();


            Initialize();
        }

        public virtual void Initialize()
        {

        }


        protected void Start()
        {
            playerTr = GameManager.Instance.player;
            GameManager.Instance.PlayerDeadEvent += PlayerIsDead;   //�÷��̾ ���� ��� �����ϴ� �̺�Ʈ 
        }


        protected void Update()
        {
            if (!activate)
                return;

            if (enemyState == EnemyState.Die)
                return;

            //if (enemyState == EnemyState.Attack)
            //    return;

            if (action != null)
            {
                if (action.attackOn || action.onAir)
                    return;
            }
            
            if (Time.time - lastCheckTime < timeBetweenChecks)
                return;
            
            //������ �ð� �ʱ�ȭ
            lastCheckTime = Time.time;

            DetectSiutation();
        }

        //���� ������ ���� �ٸ��� ��Ȳ�� �����Ѵ�. 

        public abstract void DetectSiutation();



        #region Checks
        //���� ���� üũ

        protected void TotalRangeCheck()
        {
            playerDistance = (playerTr.position - transform.position).magnitude;

            //�Ÿ��� ���� �Ÿ� ���̸� �����Ѵ�.
            if (playerDistance > checkRange)
            {
                inChaseRange = false;
            }
            //�Ÿ��� �����Ÿ� �� ~ �ּ� �Ÿ� ���ΰ�?
            else if (playerDistance <= checkRange && playerDistance > minRange)
            {
                inChaseRange = true;
            }
            //�Ÿ��� �ּ� �Ÿ� ���̸� ����Ѵ�. 
            else if (playerDistance <= minRange)
            {
                inChaseRange = false;
            }

            //�Ÿ��� ���� �Ÿ� �� �ΰ�? 
            if(playerDistance < attackRange)
            {
                inAttackRange = true;

                if (VisionCheck())
                {
                    playerIsVisible = true;
                }
                else
                {
                    playerIsVisible = false;
                }
            }
            else
            {
                inAttackRange = false;
            }
        }

        //protected bool RangeCheck()
        //{
        //    bool inRange = false;
        //    playerDistance = (playerTr.position - transform.position).magnitude;

        //    if (playerDistance <= checkRange)
        //    {
        //        inRange = true;
        //    }
        //    else 
        //    {
        //        inRange = false;
        //    }

        //    return inRange;

        //}


        //�þ߿� ���̴��� üũ
        protected bool VisionCheck()
        {
            bool inVision = false;

            Vector2 playerVector = playerTr.position - transform.position;
            Vector2 playerDirection = playerVector.normalized;
            playerDistance = (playerTr.position - transform.position).magnitude;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, playerDistance, LayerMask.GetMask("Planet"));
            if (hit.collider != null)
            {
                inVision = false;

            }
            else
            {
                inVision = true;
            }
            return inVision;
        }

        ////���� ���� üũ
        //protected bool AttackRangeCheck()
        //{
        //    bool inAttackRange = false;
        //    playerDistance = (playerTr.position - transform.position).magnitude;

        //    if (playerDistance <= attackRange)
        //    {
        //        inAttackRange = true;
        //    }
        //    else
        //    {
        //        inAttackRange = false;
        //    }

        //    return inAttackRange;
        //}

        ////�ּ� �Ÿ� üũ
        //protected bool MinRangeCheck()
        //{
        //    bool inMinRange = false;
        //    playerDistance = (playerTr.position - transform.position).magnitude;

        //    if (playerDistance <= minRange)
        //    {
        //        inMinRange = true;
        //    }
        //    else
        //    {
        //        inMinRange = false;
        //    }

        //    return inMinRange;
        //}


        //���� �༺�� �ִ��� üũ
        protected bool OnSamePlanetCheck()
        {
            bool inSamePlanet = false;

            if (playerTr.GetComponent<CharacterGravity>().nearestPlanet == gravity.nearestPlanet)
            {
                inSamePlanet = true;
            }
            playerIsInOtherPlanet = !inSamePlanet;
            return inSamePlanet;
        }

        //���� �༺���� �÷��̾ ������ �� �ִ� ����Ʈ�� �ִ��� ã��
        protected bool FindAttackablePoint()
        {
            bool findPoint = false;

            //���� �÷��̾ ���̴� Collider2D ���� ����Ʈ���� ���� �˾Ƴ���.
            List<int> visiblePoints = new List<int>();
            int pointCounts = gravity.nearestCollider.points.Length - 1;

            for (int i = 0; i < pointCounts; i++)
            {
                //���ݸ� ����. i�� ¦���� ���.
                if (i % 4 == 0)
                    continue;

                //����Ʈ�� ��ġ�� �����´�
                Vector2 pointVector = GetPointPos(i);
                Vector2 edgeDirection = gravity.nearestCollider.points[i + 1] - gravity.nearestCollider.points[i];
                Vector2 normal = Vector2.Perpendicular(edgeDirection).normalized;
                Vector2 playerPos = playerTr.position;

                pointVector = pointVector + (normal * action.enemyHeight);
                Vector2 dir = playerPos - pointVector;
                float dist = dir.magnitude;
                dir = dir.normalized;

                //�����Ÿ� ������ ���鸸 üũ�Ѵ�
                if (dist > attackRange)
                    continue;

                //��AI�� �� �ִ� �༺�� Point �߿��� �÷��̾ ���̴� Point �鸸 �̾Ƴ���. 
                RaycastHit2D hit = Physics2D.Raycast(pointVector, dir, dist, LayerMask.GetMask("Planet"));
                if (hit.collider == null)
                {
                    findPoint = true;
                    break;
                }
            }

            return findPoint;
        }

        Vector2 GetPointPos(int pointIndex)
        {
            Vector3 localPoint = gravity.nearestCollider.points[pointIndex];
            Vector2 pointPos = gravity.nearestCollider.transform.TransformPoint(localPoint);
            return pointPos;
        }

        #endregion



        //�����, Planet���� ��ȣ�� �޴� �뵵
        public virtual void WakeUp()
        {
            if (enemyState == EnemyState.Die)
                return;

            //timeBetweenChecks = 0.5f;

            //�ẹ ����
            if (enemyState == EnemyState.Ambush)
            {
                action.AmbushEndEvent();
            }

            ChangeState(EnemyState.Chase, 0.5f);
        }


        public virtual void DamageEvent(float dmg)
        {
            if (enemyState == EnemyState.Die)
                return;

            //�������� ����
            if (health.AnyDamage(dmg))
            {
                //�´� ȿ�� 
                action.HitView();

                //��� �ִ� �༺�� �� Ȱ��ȭ ���¶��
                if (!gravity.nearestPlanet.activate)
                {
                    //���� �༺�� ������ ����� 
                    gravity.nearestPlanet.WakeUpPlanet();
                }

                if (health.IsDead())
                {
                    StopAllCoroutines();
                    //���� ��� 
                    enemyState = EnemyState.Die;
                }
            }
        }


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



        //�÷��̾ ���� ��� 
        public void PlayerIsDead()
        {            
            activate = false;
            enemyState = EnemyState.Idle;
        }

        //������ ��� 
        public void ResetEnemyBrain()
        {
            health.ResetHealth();
            enemyState = EnemyState.Idle;
            activate = true;

            //action���� �����ؾ��Ѵ�.
            action.ResetAction();
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, checkRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

    }


    public enum EnemyState
    {
        Ambush, 
        Idle, 
        Chase,
        ToJump,
        Attack, 
        Die,
        Wait
    }


}


