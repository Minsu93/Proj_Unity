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
        public bool activate = true;    //애너미가 활성화 되었습니까?
        public bool doTotalRangeCheck;  //적을 감지합니까? false시 감지 자체를 중단.
        public bool doAttackRangeCheck; //공격 거리를 체크합니까?
        public bool doVisionCheck;      //시야를 체크합니까? 
        public bool doPlanetCheck;      //행성을 체크합니까? 
        
        public float timeBetweenChecks = 0.5f;  //플레이어 감지 시간 간격(고정)
        public float limitRange;        //한계 거리
        public float attackRange;       //공격 거리

        //이펙트
        public ParticleSystem hitEffect;    //맞았을 때 효과
        public ParticleSystem deadEffect;   //죽었을 때 효과


        //로컬변수
        float lastCheckTime;
        Coroutine waitRoutine; //대기용 코루틴

        //스크립트
        protected Health health;
        protected EnemyAction action;
        protected CharacterGravity gravity;
        //protected CharacterGravity pGravity;
        protected DropItem dropitem;

        //참조
        public float playerDistance { get; set; }     //플레이어와의 거리를 기록해서 다른 행동을 할 수 있도록.
        public Transform playerTr { get; set; }  //플레이어의 위치
        public Vector2 playerDirection { get; set; }     //플레이어 방향
        
        //로컬참조
        public bool inDetectRange;   //감지 거리 안에 있다.
        public bool inAttackRange;  //공격 범위 안에 있다. 
        public bool isVisible;    //플레이어가 눈에 보이는가
        public bool inOtherPlanet;     //다른 행성으로 추격할것인가?
        public bool playerIsRight;      //플레이어가 오른쪽에 있나?



        protected virtual void Awake()
        {
            health = GetComponent<Health>();
            action = GetComponent<EnemyAction>();
            gravity = GetComponent<CharacterGravity>();
            playerTr = GameManager.Instance.player;
            //pGravity = playerTr.GetComponent<CharacterGravity>();
            dropitem = GetComponent<DropItem>();


            if (health != null) health.ResetHealth();    //체력 초기화
        }


        protected void Start()
        {
            GameManager.Instance.PlayerDeadEvent += PlayerIsDead;   //플레이어가 죽은 경우 실행하는 이벤트 
            
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

        //리셋한 경우 
        public void ResetEnemyBrain()
        {
            activate = true;
            enemyState = EnemyState.Sleep;

            health.ResetHealth();
            action.ResetAction();
        }

        //유닛 종류에 따라 다르게 상황을 감지한다. 
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

        //    //먼저 플레이어가 보이는 Collider2D 상의 포인트들의 값을 알아낸다.
        //    List<int> visiblePoints = new List<int>();
        //    int pointCounts = gravity.nearestCollider.points.Length - 1;

        //    for (int i = 0; i < pointCounts; i++)
        //    {
        //        //절반만 하자. i가 짝수면 통과.
        //        if (i % 4 == 0)
        //            continue;

        //        //포인트의 위치를 가져온다
        //        Vector2 pointVector = GetPointPos(i);
        //        Vector2 edgeDirection = gravity.nearestCollider.points[i + 1] - gravity.nearestCollider.points[i];
        //        Vector2 normal = Vector2.Perpendicular(edgeDirection).normalized;
        //        Vector2 playerPos = playerTr.position;

        //        pointVector = pointVector + (normal * action.enemyHeight);
        //        Vector2 dir = playerPos - pointVector;
        //        float dist = dir.magnitude;
        //        dir = dir.normalized;

        //        //사정거리 내부의 점들만 체크한다
        //        if (dist > attackRange)
        //            continue;

        //        //적AI가 서 있는 행성의 Point 중에서 플레이어가 보이는 Point 들만 뽑아낸다. 
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
                //맞는 효과 
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

        //깨어나기, 모든 Brain 들은 Planet에서 신호를 받아서 깨어난다. 
        public virtual void WakeUp()
        {
            if(enemyState == EnemyState.Sleep)
            {
                ChangeState(EnemyState.Chase, 0f);
                action.WakeUpEvent();
            }
        }

        #region Delaied State Change
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

        #endregion

        //플레이어가 죽은 경우 다시 잠든다. 
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


