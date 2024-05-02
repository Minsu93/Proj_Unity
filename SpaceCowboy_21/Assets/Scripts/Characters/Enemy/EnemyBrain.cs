using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;


    [SelectionBase]

    public abstract class EnemyBrain : MonoBehaviour , IHitable
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

            if (health != null) health.ResetHealth();    //ü�� �ʱ�ȭ
        }


        protected void Start()
        {
            GameManager.Instance.PlayerDeadEvent += PlayerIsDead;   //�÷��̾ ���� ��� �����ϴ� �̺�Ʈ 
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
        public void ResetEnemyBrain(EnemyState eState)
        {
            activate = true;
            enemyState = eState;

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
        #endregion

        #region Hit and Damage
        public virtual void DamageEvent(float damage, Vector2 hitVec)
        {
            if (enemyState == EnemyState.Die) return;

            if (health.AnyDamage(damage))
            {
                //�´� ȿ�� 
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


        //�����, ��� Brain ���� Planet���� ��ȣ�� �޾Ƽ� �����. 
        public virtual void WakeUp()
        {
            if (enemyState == EnemyState.Die) return;
            
            ChangeState(EnemyState.Chase, 0f);
            action.WakeUpEvent();
        }

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
        Wait, 
        Strike
    }



