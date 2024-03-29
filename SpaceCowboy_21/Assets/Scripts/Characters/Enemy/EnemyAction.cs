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
        /// ���� ���� �ٸ� �ൿ�� �Ѵ�. EnemyAction�� Ground, Orbit, Ship ��ο� ���� �ٸ� �ൿ�� �ϵ��� ������� �ִ�. �̵��� �������� Brain �� EnemyState�� ���� �޾ƿ�, State�� �ٲ� �� �׿� �´� �ൿ�� �ϵ��� �ٲ�� ���̴�. 
        /// Brain�� State�� ������ ���ϱ� ������, Action������ �ൿ�� ��ȭ�� õõ�� �Ͼ ������ ���ȴ�. 
        /// �� ���ָ��� �ٸ� Action�� �ϰ�����, ū Ʋ������ ����ϱ� ������ ������ Action�� ��ӹ޾� ����ϴ��� ������ ������ ������� �Ѵ�. 
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
        public bool onAttack { get; set; }  //�������� �� 
        public bool attackCool { get; set; }    //���� ��Ÿ��

        [Header("BodyAttack")]
        public float bodyDamage = 3.0f;

        //�Ѿ�
        public List<ProjectileStruct> projectileStructs = new List<ProjectileStruct>();
        
        //���� ����
        bool isAimOn = false;   //���� ���ΰ���
        protected bool onChase = false;   //chase ���ΰ���
        protected bool startChase;        //chase ���� �� �ѹ��� ����.

        protected EnemyState preState = EnemyState.Die;
        protected Coroutine attackRoutine;

        public bool faceRight { get; set; }  //ĳ���Ͱ� �������� ���� �ֽ��ϱ�? 
        public bool onAir { get; set; } //���߿� �ִ°�


        //��ũ��Ʈ
        protected EnemyBrain brain { get; set; }
        protected CharacterGravity gravity;
        protected Rigidbody2D rb;
        public GameObject hitCollObject;    //�ǰ� ó���� �ϴ� Collider
        public GameObject iconUI;
       


        //�̺�Ʈ
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

            //���� ��Ÿ�� ���ȿ��� �ൿ�� �����. �����߿� �̵��ϰ� ������ Update�� �����ض�.
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

        //���°� �ٲ� �� �ѹ��� ���� 
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
        //���� �ϴ� �ൿ�� ����.
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

            //�÷��̾� ������ �ݿ�
            //PlayerBehavior pb = GameManager.Instance.PlayerBehavior;
            //Vector2 playerPredictedPos = (Vector2)pb.transform.position + (pb.playerVelocity * predictionAccuracy);
            //Vector2 upVec = Quaternion.Euler(0, 0, 90) * (playerPredictedPos - (Vector2)transform.position).normalized ;
            //Quaternion gunTipRot = Quaternion.LookRotation(Vector3.forward, upVec);
            
            Quaternion targetRotation = gunTipRot;

            //���� ���� �߰�
            float randomAngle = Random.Range(-projectileStructs[projIndex].spreadAngle * 0.5f, projectileStructs[projIndex].spreadAngle * 0.5f);
            Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

            //�Ѿ� ����
            GameObject projectile = PoolManager.instance.GetEnemyProj(projectileStructs[projIndex].projectile);
            projectile.transform.position = gunTipPos;
            projectile.transform.rotation = targetRotation * randomRotation;
            projectile.GetComponent<Projectile>().init(projectileStructs[projIndex].damage, projectileStructs[projIndex].speed, projectileStructs[projIndex].range, projectileStructs[projIndex].lifeTime);

            //View���� �ִϸ��̼� ����
            AttackView();
        }

        protected void ShootAction(Vector2 pos, Quaternion Rot, int projIndex, Planet planet, bool isRight)
        {
            Vector2 gunTipPos = pos;
            Quaternion gunTipRot = Rot;

            //�÷��̾� ������ �ݿ�
            //PlayerBehavior pb = GameManager.Instance.PlayerBehavior;
            //Vector2 playerPredictedPos = (Vector2)pb.transform.position + (pb.playerVelocity * predictionAccuracy);
            //Vector2 upVec = Quaternion.Euler(0, 0, 90) * (playerPredictedPos - (Vector2)transform.position).normalized ;
            //Quaternion gunTipRot = Quaternion.LookRotation(Vector3.forward, upVec);

            Quaternion targetRotation = gunTipRot;

            //���� ���� �߰�
            float randomAngle = Random.Range(-projectileStructs[projIndex].spreadAngle * 0.5f, projectileStructs[projIndex].spreadAngle * 0.5f);
            Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

            //�Ѿ� ����
            GameObject projectile = PoolManager.instance.GetEnemyProj(projectileStructs[projIndex].projectile);
            projectile.transform.position = gunTipPos;
            projectile.transform.rotation = targetRotation * randomRotation;
            projectile.GetComponent<Projectile>().init(projectileStructs[projIndex].damage, projectileStructs[projIndex].speed, projectileStructs[projIndex].range, projectileStructs[projIndex].lifeTime, planet, isRight);

            //View���� �ִϸ��̼� ����
            AttackView();
        }


        protected IEnumerator DelayRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
        }

        //Start���� Sleep������ ��. ���� �� ���̰ų�, �������� �ʴ´�. 
        protected void OnSleepEvent()
        {
            activate = false;
            hitCollObject.SetActive(false);
        }

        //Brain ���� Sleep > Idle ������ �� �� ���� ����. 
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

