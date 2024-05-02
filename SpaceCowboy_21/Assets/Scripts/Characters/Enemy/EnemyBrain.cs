using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;


    [SelectionBase]

    public abstract class EnemyBrain : MonoBehaviour , IHitable
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

            if (health != null) health.ResetHealth();    //체력 초기화
        }


        protected void Start()
        {
            GameManager.Instance.PlayerDeadEvent += PlayerIsDead;   //플레이어가 죽은 경우 실행하는 이벤트 
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
        public void ResetEnemyBrain(EnemyState eState)
        {
            activate = true;
            enemyState = eState;

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
        #endregion

        #region Hit and Damage
        public virtual void DamageEvent(float damage, Vector2 hitVec)
        {
            if (enemyState == EnemyState.Die) return;

            if (health.AnyDamage(damage))
            {
                //맞는 효과 
                if(action != null) action.HitView();
                if (hitEffect != null) ParticleHolder.instance.GetParticle(hitEffect, transform.position, transform.rotation);

                if (health.IsDead())
                {
                    StopAllCoroutines();
                    activate = false;
                    enemyState = EnemyState.Die;
                    
                    if (deadEffect != null) ParticleHolder.instance.GetParticle(deadEffect, transform.position, transform.rotation);

                    WhenDieEvent();
                }
            }
        }

        protected virtual void WhenDieEvent()
        {

            GameManager.Instance.ChargeFireworkEnergy();

        }

        #endregion

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


        //깨어나기, 모든 Brain 들은 Planet에서 신호를 받아서 깨어난다. 
        public virtual void WakeUp()
        {
            if (enemyState == EnemyState.Die) return;
            
            ChangeState(EnemyState.Chase, 0f);
            action.WakeUpEvent();
        }

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
        Wait, 
        Strike
    }



