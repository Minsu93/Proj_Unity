using SpaceCowboy;
using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceEnemy
{
    public class EnemyGuard : EnemyAction
    {
        [Header("Guard Property")]
        public float pushDistance;      //밀치기 거리. 플레이어가 이 거리 안으로 들어오면 플레이어를 반대 방향으로 밀어낸다. 
        public float pushTime;          //밀치는 시간
        public float pushSpeed;         //밀치는 힘
        public float attackCoolTime;    //공격(slash)를 사용할 쿨타임
        public GameObject shieldBodyPart;   //방어 역할을 할 Collider부분.

        float preAttackTime;
        PlayerBehavior playerBehavior;

        public SkeletonAnimation skeletonAnimation;
        [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
        public string eventName;

        [Space]
        public bool logDebugMessage = false;

        Spine.EventData eventData;

        protected override void Awake()
        {
            base.Awake();

            playerBehavior = GameManager.Instance.player.GetComponent<PlayerBehavior>();    
        }


        private void Start()
        {
            if (skeletonAnimation == null) return;
            skeletonAnimation.Initialize(false);
            if (!skeletonAnimation.valid) return;

            eventData = skeletonAnimation.Skeleton.Data.FindEvent(eventName);
            skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
        }

        private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (logDebugMessage) Debug.Log("Event fired! " + e.Data.Name);
            //bool eventMatch = string.Equals(e.Data.Name, eventName, System.StringComparison.Ordinal); // Testing recommendation: String compare.
            bool eventMatch = (eventData == e.Data); // Performance recommendation: Match cached reference instead of string.
            if (eventMatch)
            {
                Shoot();
            }
        }

        protected override void Update()
        {
            base.Update();

            /*
            //플레이어와의 거리를 가져와서 너무 가까우면 밀쳐냄
            if (brain.playerDistance < pushDistance)
            {
                if (brain.playerDistance == 0)  //가장 처음에 playerDistance 가 0일때 제외.
                    return; 

                if (playerBehavior != null)
                {
                    playerBehavior.PlayerKnockBack(transform.position, pushTime, pushSpeed);
                }
            }
            */
        }
        public override void ChaseAction()
        {
            //적을 발견하면 실드를 올린다.
            StartGuardEvent();
            MoveToVisiblePoint(brain.playerTr.position, 10f);
        }

        public override void AttackAction()
        {
            
            //공격 쿨타임이 있으며, 쿨타임이 아닌 경우에는 다시 idle로 옮겨감. 
            if (Time.time - preAttackTime > attackCoolTime)
            {
                StartCoroutine(AttackCoroutine());
            }
            else
            {
                if (brain.enemyState == EnemyState.Attack)
                    brain.enemyState = EnemyState.Idle;
            }

        }

        public override void DieAction()
        {
            StopAllCoroutines();
        }


        IEnumerator AttackCoroutine()
        {
            //실드 내린 후 기다림. 피격 타이밍
            StopGuardEvent();
            yield return StartCoroutine(ChargeRoutine(chargeTime));
            //공격
            yield return StartCoroutine(ShootRoutine(shootDelay));
            //공격 후 다시 실드 ON
            yield return StartCoroutine(DelayRoutine(afterShootDelay));
            StartGuardEvent();

            //공격 상태라면 다시 감지 상태로 돌아간다
            if (brain.enemyState == EnemyState.Attack)
                brain.enemyState = EnemyState.Idle;
        }

        protected IEnumerator ChargeRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
        }

        protected IEnumerator ShootRoutine(float delay)
        {
            ShootEvent();

            preAttackTime = Time.time;

            yield return new WaitForSeconds(delay);
        }

        void Shoot()
        {
            Vector3 dir = (brain.playerTr.position - gunTip.position).normalized; //발사 각도
            Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;       // 첫 발사 방향을 구한다. 
            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        


            //총알 생성
            GameObject projectile = GameManager.Instance.poolManager.GetEnemyProj(projectilePrefab);
            projectile.transform.position = gunTip.position;
            projectile.transform.rotation = targetRotation;
            projectile.GetComponent<Projectile>().init(damage, lifeTime, projectileSpeed);

        }

        protected override void StartGuardEvent()
        {
            base.StartGuardEvent();
            shieldBodyPart.SetActive(true);

        }
        protected override void StopGuardEvent()
        {
            base.StopGuardEvent();
            shieldBodyPart.SetActive(false);

        }



    }
}

