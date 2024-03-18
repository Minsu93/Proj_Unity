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
        public bool activate = true;    //애너미가 활성화 되었습니까?

        [Space]

        public float timeBetweenChecks = 0.5f;  //플레이어 감지 시간 간격(고정)
        public float checkRange;        //감지 거리
        public float attackRange;       //공격 거리
        public float minRange;          //최소 거리 

        float lastCheckTime;
        public float playerDistance { get; set; }     //플레이어와의 거리를 기록해서 다른 행동을 할 수 있도록.
        public Transform playerTr { get; set; }  //플레이어의 위치

        protected Health health;
        protected EnemyAction action;
        protected CharacterGravity gravity;

        //디버그 용

        public bool inChaseRange;   //추격 범위 안에 있다 (감지거리 ~  최소거리 사이)
        public bool inAttackRange;  //공격 범위 안에 있다. 
        public bool playerIsVisible;    //플레이어가 눈에 보이는가
        public bool playerIsInOtherPlanet;     //다른 행성으로 추격할것인가?

        Coroutine waitRoutine; //대기용 코루틴


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
            GameManager.Instance.PlayerDeadEvent += PlayerIsDead;   //플레이어가 죽은 경우 실행하는 이벤트 
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
            
            //실행한 시간 초기화
            lastCheckTime = Time.time;

            DetectSiutation();
        }

        //유닛 종류에 따라 다르게 상황을 감지한다. 

        public abstract void DetectSiutation();



        #region Checks
        //감지 범위 체크

        protected void TotalRangeCheck()
        {
            playerDistance = (playerTr.position - transform.position).magnitude;

            //거리가 감지 거리 밖이면 리턴한다.
            if (playerDistance > checkRange)
            {
                inChaseRange = false;
            }
            //거리가 검지거리 안 ~ 최소 거리 밖인가?
            else if (playerDistance <= checkRange && playerDistance > minRange)
            {
                inChaseRange = true;
            }
            //거리가 최소 거리 안이면 대기한다. 
            else if (playerDistance <= minRange)
            {
                inChaseRange = false;
            }

            //거리가 공격 거리 안 인가? 
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


        //시야에 보이는지 체크
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

        ////공격 범위 체크
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

        ////최소 거리 체크
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


        //같은 행성에 있는지 체크
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

        //현재 행성에서 플레이어를 공격할 수 있는 포인트가 있는지 찾기
        protected bool FindAttackablePoint()
        {
            bool findPoint = false;

            //먼저 플레이어가 보이는 Collider2D 상의 포인트들의 값을 알아낸다.
            List<int> visiblePoints = new List<int>();
            int pointCounts = gravity.nearestCollider.points.Length - 1;

            for (int i = 0; i < pointCounts; i++)
            {
                //절반만 하자. i가 짝수면 통과.
                if (i % 4 == 0)
                    continue;

                //포인트의 위치를 가져온다
                Vector2 pointVector = GetPointPos(i);
                Vector2 edgeDirection = gravity.nearestCollider.points[i + 1] - gravity.nearestCollider.points[i];
                Vector2 normal = Vector2.Perpendicular(edgeDirection).normalized;
                Vector2 playerPos = playerTr.position;

                pointVector = pointVector + (normal * action.enemyHeight);
                Vector2 dir = playerPos - pointVector;
                float dist = dir.magnitude;
                dir = dir.normalized;

                //사정거리 내부의 점들만 체크한다
                if (dist > attackRange)
                    continue;

                //적AI가 서 있는 행성의 Point 중에서 플레이어가 보이는 Point 들만 뽑아낸다. 
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



        //깨어나기, Planet에서 신호를 받는 용도
        public virtual void WakeUp()
        {
            if (enemyState == EnemyState.Die)
                return;

            //timeBetweenChecks = 0.5f;

            //잠복 종료
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

            //데미지를 적용
            if (health.AnyDamage(dmg))
            {
                //맞는 효과 
                action.HitView();

                //밟고 있는 행성이 비 활성화 상태라면
                if (!gravity.nearestPlanet.activate)
                {
                    //같은 행성의 적들을 깨우기 
                    gravity.nearestPlanet.WakeUpPlanet();
                }

                if (health.IsDead())
                {
                    StopAllCoroutines();
                    //죽은 경우 
                    enemyState = EnemyState.Die;
                }
            }
        }


        //딜레이 주기
        public void ChangeState(EnemyState state, float time)
        {
            if (enemyState == EnemyState.Die)
                return;
            
            //코루틴 중복을 막기 위해
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



        //플레이어가 죽은 경우 
        public void PlayerIsDead()
        {            
            activate = false;
            enemyState = EnemyState.Idle;
        }

        //리셋한 경우 
        public void ResetEnemyBrain()
        {
            health.ResetHealth();
            enemyState = EnemyState.Idle;
            activate = true;

            //action에도 전달해야한다.
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


