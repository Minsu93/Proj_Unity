using SpaceCowboy;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

namespace SpaceEnemy
{
    public abstract class EnemyAction : MonoBehaviour
    {
        /// <summary>
        /// 적에 따라 다른 행동을 한다. EnemyAction은 Ground, Orbit, Ship 모두에 따라 다른 행동을 하도록 만들어져 있다. 이들의 공통점은 Brain 의 EnemyState를 매턴 받아와, State가 바뀔 때 그에 맞는 행동을 하도록 바뀌는 것이다. 
        /// Brain의 State가 느리게 변하기 때문에, Action에서는 행동의 변화가 천천히 일어날 것으로 기대된다. 
        /// 각 유닛마다 다른 Action을 하겠지만, 큰 틀에서는 비슷하기 때문에 동일한 Action을 상속받아 사용하더라도 문제가 없도록 만들려고 한다. 
        /// </summary>
        public bool activate;

        [Header("Move Property")]
        public float moveSpeed = 5f;
        public float enemyHeight = 0.51f;
        public float turnSpeedOnLand = 100f;

        [Header("Attack Property")]
        public float preAttackDelay = 0f;
        public float afterAttackDelay = 0f;
        public float attackCoolTime = 3.0f;
        public float predictionAccuracy = 0f;
        protected float atTimer;
        public bool onAttack { get; set; }  //공격중일 때 
        public bool attackCool { get; set; }    //공격 쿨타임

        [Header("BodyAttack")]
        public float bodyDamage = 3.0f;

        //총알
        public List<ProjectileStruct> projectileStructs = new List<ProjectileStruct>();
        
        //로컬 변수
        bool isAimOn = false;   //조준 중인가요
        protected bool onChase = false;   //chase 중인가요
        protected bool startChase;        //chase 시작 시 한번만 실행.

        protected EnemyState preState = EnemyState.Die;
        protected Coroutine attackRoutine;

        public bool faceRight { get; set; }  //캐릭터가 오른쪽을 보고 있습니까? 
        public bool onAir { get; set; } //공중에 있는가


        //스크립트
        protected EnemyBrain brain { get; set; }
        protected CharacterGravity gravity;
        protected Rigidbody2D rb;
        public GameObject hitCollObject;    //피격 처리를 하는 Collider
        public GameObject iconUI;
       


        //이벤트
        public event System.Action EnemyStartRun;
        public event System.Action EnemyStartIdle;
        public event System.Action EnemyAttackEvent;
        public event System.Action EnemyAimOnEvent;
        public event System.Action EnemyAimOffEvent;
        public event System.Action EnemySeePlayerOnceEvent;
        public event System.Action EnemySeeDirection;
        public event System.Action EnemyHitEvent;
        public event System.Action EnemyDieEvent;
        public event System.Action EnemyAttackTransitionEvent;



        protected virtual void Awake()
        {
            gravity = GetComponent<CharacterGravity>();
            rb = GetComponent<Rigidbody2D>();
            brain = GetComponent<EnemyBrain>();
        }

        protected virtual void OnEnable()
        {
            iconUI.SetActive(true);
        }
        private void OnDisable()
        {
            iconUI.SetActive(false);
        }

        public void ResetAction()
        {
            activate = true;
            attackCool = false;
            onAttack = false;
            atTimer = 0;
            onAir = true;
            hitCollObject.SetActive(true);
        }

        protected virtual void Update()
        {
            EnemyState currState = brain.enemyState;

            if (currState != preState)
            {
                StopAction(preState);
                DoAction(currState);
                preState = currState;
            }

            if (!activate) return;

            if (attackCool)
            {
                atTimer += Time.deltaTime;
                if (atTimer > attackCoolTime)
                {
                    attackCool = false;
                    atTimer = 0;
                }
            }

            //공격 쿨타임 동안에는 행동을 멈춘다. 공격중에 이동하고 싶으면 Update를 수정해라.
            if (attackCool) return; 

            if(onAttack)
            {
                OnAttackAction();
            }

            if(onChase)
            {
                OnChaseAction();
            }
        }

        //상태가 바뀔 때 한번만 실행 
        protected virtual void DoAction(EnemyState state)
        {
            switch (state)
            {
                case EnemyState.Sleep:
                    OnSleepEvent();
                    StartIdleView();
                    break;

                case EnemyState.Chase:
                    onChase = true;
                    break;

                case EnemyState.Attack:
                    onAttack = true;
                    break;

                case EnemyState.Die:
                    OnDieAction();

                    break;
            }
        }
        //이전 하던 행동을 리셋.
        protected virtual void StopAction(EnemyState preState)
        {
            switch (preState)
            {
                case EnemyState.Sleep:
                    WakeUpEvent();
                    break;
                case EnemyState.Chase:
                    onChase = false;
                    break;
                case EnemyState.Attack:
                    onAttack = false;
                    break;
            }
        }

        protected abstract void OnChaseAction();
        protected abstract void OnAttackAction();
        protected virtual void OnDieAction()
        {
            StopAllCoroutines();
            DieView();

            activate = false;
            iconUI.SetActive(false);
            hitCollObject.SetActive(false);

            StartCoroutine(DieRoutine(3.0f));
        }

        #region Basic Actions
        protected void ShootAction(Vector2 pos, Quaternion Rot, int projIndex)
        {
            Vector2 gunTipPos = pos;
            Quaternion gunTipRot = Rot;

            //플레이어 움직임 반영
            //PlayerBehavior pb = GameManager.Instance.PlayerBehavior;
            //Vector2 playerPredictedPos = (Vector2)pb.transform.position + (pb.playerVelocity * predictionAccuracy);
            //Vector2 upVec = Quaternion.Euler(0, 0, 90) * (playerPredictedPos - (Vector2)transform.position).normalized ;
            //Quaternion gunTipRot = Quaternion.LookRotation(Vector3.forward, upVec);
            
            Quaternion targetRotation = gunTipRot;

            //랜덤 각도 추가
            float randomAngle = Random.Range(-projectileStructs[projIndex].spreadAngle * 0.5f, projectileStructs[projIndex].spreadAngle * 0.5f);
            Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

            //총알 생성
            GameObject projectile = PoolManager.instance.GetEnemyProj(projectileStructs[projIndex].projectile);
            projectile.transform.position = gunTipPos;
            projectile.transform.rotation = targetRotation * randomRotation;
            projectile.GetComponent<Projectile>().init(projectileStructs[projIndex].damage, projectileStructs[projIndex].speed, projectileStructs[projIndex].range, projectileStructs[projIndex].lifeTime);

            //View에서 애니메이션 실행
            AttackView();
        }

        protected void ShootAction(Vector2 pos, Quaternion Rot, int projIndex, Planet planet, bool isRight)
        {
            Vector2 gunTipPos = pos;
            Quaternion gunTipRot = Rot;

            //플레이어 움직임 반영
            //PlayerBehavior pb = GameManager.Instance.PlayerBehavior;
            //Vector2 playerPredictedPos = (Vector2)pb.transform.position + (pb.playerVelocity * predictionAccuracy);
            //Vector2 upVec = Quaternion.Euler(0, 0, 90) * (playerPredictedPos - (Vector2)transform.position).normalized ;
            //Quaternion gunTipRot = Quaternion.LookRotation(Vector3.forward, upVec);

            Quaternion targetRotation = gunTipRot;

            //랜덤 각도 추가
            float randomAngle = Random.Range(-projectileStructs[projIndex].spreadAngle * 0.5f, projectileStructs[projIndex].spreadAngle * 0.5f);
            Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

            //총알 생성
            GameObject projectile = PoolManager.instance.GetEnemyProj(projectileStructs[projIndex].projectile);
            projectile.transform.position = gunTipPos;
            projectile.transform.rotation = targetRotation * randomRotation;
            projectile.GetComponent<Projectile>().init(projectileStructs[projIndex].damage, projectileStructs[projIndex].speed, projectileStructs[projIndex].range, projectileStructs[projIndex].lifeTime, planet, isRight);

            //View에서 애니메이션 실행
            AttackView();
        }


        protected IEnumerator DelayRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
        }

        //Start에서 Sleep상태일 때. 적은 안 보이거나, 움직이지 않는다. 
        protected void OnSleepEvent()
        {
            activate = false;
            hitCollObject.SetActive(false);
        }

        //Brain 에서 Sleep > Idle 상태일 때 한 번만 실행. 
        public virtual void WakeUpEvent()
        {
            activate = true;
            hitCollObject.SetActive(true);
        }

        IEnumerator DieRoutine(float sec)
        {
            yield return new WaitForSeconds(sec);
            gameObject.SetActive(false);
        }

        #endregion

        #region Collide with Player
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (bodyDamage == 0) return;

            if (other.CompareTag("Player"))
            {
                if (other.TryGetComponent<PlayerBehavior>(out PlayerBehavior pb))
                {
                    pb.DamageEvent(bodyDamage, transform.position);
                }
            }
        }

        #endregion

        #region Animation View Events
        protected void StartRunView()
        {
            if (EnemyStartRun != null)
                EnemyStartRun();
        }
        protected void StartIdleView()
        {

            if (EnemyStartIdle != null)
                EnemyStartIdle();
        }

        protected void AttackView()
        {
            if (EnemyAttackEvent != null)
                EnemyAttackEvent();
        }

        protected void AimOnView()
        {
            if (EnemyAimOnEvent != null)
                EnemyAimOnEvent();
        }
        protected void AimOffView()
        {
            if (EnemyAimOffEvent != null)
                EnemyAimOffEvent();
        }

        protected void SeePlayerOnceEvent()
        {
            if (EnemySeePlayerOnceEvent != null)
                EnemySeePlayerOnceEvent();
        }

        protected void FlipToDirectionView()
        {
            if (EnemySeeDirection != null)
                EnemySeeDirection();
        }

        public virtual void HitView()
        {
            if (EnemyHitEvent != null)
                EnemyHitEvent();
        }

        public void DieView()
        {
            if (EnemyDieEvent != null)
                EnemyDieEvent();

        }

        public void AttackModeEvent()
        {
            if (EnemyAttackTransitionEvent != null)
                EnemyAttackTransitionEvent();
        }
        public void AimStart()
        {
            if (!isAimOn)
            {
                isAimOn = true;
                AimOnView();
            }
        }
        public void AimStop()
        {
            if (isAimOn)
            {
                isAimOn = false;
                AimOffView();
            }
        }
        #endregion


        private void OnDrawGizmos()
        {
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(transform.position, preState.ToString());
        }

    }


    [System.Serializable]
    public struct ProjectileStruct
    {
        public GameObject projectile;
        public float damage;
        public float speed;
        public float spreadAngle;
        public float range;
        public float lifeTime;

        public ProjectileStruct(GameObject projectile, float damage, float speed, float spreadAngle, float range, float lifeTime)
        {
            this.projectile = projectile;
            this.damage = damage;
            this.speed = speed;
            this.spreadAngle = spreadAngle;
            this.range = range;
            this.lifeTime = lifeTime;
        }
    }

}

