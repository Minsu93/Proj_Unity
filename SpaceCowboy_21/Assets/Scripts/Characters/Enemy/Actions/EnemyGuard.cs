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
        public float pushDistance;      //��ġ�� �Ÿ�. �÷��̾ �� �Ÿ� ������ ������ �÷��̾ �ݴ� �������� �о��. 
        public float pushTime;          //��ġ�� �ð�
        public float pushSpeed;         //��ġ�� ��
        public float attackCoolTime;    //����(slash)�� ����� ��Ÿ��
        public GameObject shieldBodyPart;   //��� ������ �� Collider�κ�.

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
            //�÷��̾���� �Ÿ��� �����ͼ� �ʹ� ������ ���ĳ�
            if (brain.playerDistance < pushDistance)
            {
                if (brain.playerDistance == 0)  //���� ó���� playerDistance �� 0�϶� ����.
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
            //���� �߰��ϸ� �ǵ带 �ø���.
            StartGuardEvent();
            MoveToVisiblePoint(brain.playerTr.position, 10f);
        }

        public override void AttackAction()
        {
            
            //���� ��Ÿ���� ������, ��Ÿ���� �ƴ� ��쿡�� �ٽ� idle�� �Űܰ�. 
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
            //�ǵ� ���� �� ��ٸ�. �ǰ� Ÿ�̹�
            StopGuardEvent();
            yield return StartCoroutine(ChargeRoutine(chargeTime));
            //����
            yield return StartCoroutine(ShootRoutine(shootDelay));
            //���� �� �ٽ� �ǵ� ON
            yield return StartCoroutine(DelayRoutine(afterShootDelay));
            StartGuardEvent();

            //���� ���¶�� �ٽ� ���� ���·� ���ư���
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
            Vector3 dir = (brain.playerTr.position - gunTip.position).normalized; //�߻� ����
            Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;       // ù �߻� ������ ���Ѵ�. 
            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        


            //�Ѿ� ����
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

