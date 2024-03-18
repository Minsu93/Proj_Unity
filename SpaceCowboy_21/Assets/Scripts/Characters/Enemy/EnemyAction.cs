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

        [Header("Move Property")]
        public float moveSpeed = 5f;
        public float enemyHeight = 0.51f;
        public float turnSpeedOnLand = 100f;

        [Header("KnockBack Property")]
        public float knockBackSpeed = 4f;
        public float knockBackTime = 0.5f;

        [Header("Attack Property")]
        public float preAttackDelay = 1.0f;
        public float AttackDelay = 0.1f;
        public float afterAttackDelay = 3.0f;
        public float attackCoolTime = 3.0f;
        public float predictionAccuracy = 0f;
        protected float timer;

        public List<ProjectileStruct> projectileStructs = new List<ProjectileStruct>();

        bool isAimOn = false;   //조준 중인가요

        protected EnemyBrain brain { get; set; }
        protected CharacterGravity gravity;
        protected Rigidbody2D rb;
        public GameObject hitCollObject;    //피격 처리를 하는 Collider

        //중요
        EnemyState preState = EnemyState.Die;

        [Header("Icon Property")]
        public GameObject iconUI;

        public bool faceRight { get; set; }  //캐릭터가 오른쪽을 보고 있습니까? 
        public bool attackOn { get; set; }  //공격중일 때 
        public bool attackCool { get; set; }    //공격 쿨타임
        public bool onAir { get; set; } //공중에 있는가
        protected bool activate;  //활성화


        protected Coroutine attackRoutine;

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
            if (gravity == null)
                gravity = GetComponent<CharacterGravity>();
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (brain == null)
            {
                brain = GetComponent<EnemyBrain>();
            }
            //enemyColl = GetComponents<Collider2D>()[1];
            activate = true;

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
            attackOn = false;
            timer = 0;
            onAir = true;
            hitCollObject.SetActive(true);

            DoAction(EnemyState.Idle);
        }

        protected virtual void Update()
        {
            if (!activate) return;

            EnemyState currState = brain.enemyState;

            if (currState != preState)
            {
                DoAction(currState);
                preState = currState;
            }

            if (preState == EnemyState.Die) activate = false;

            if (attackCool)
            {
                timer += Time.deltaTime;
                if (timer > attackCoolTime)
                {
                    attackCool = false;
                    timer = 0;
                }
            }
        }

        //상태가 바뀔 때 한번만 실행 
        public abstract void DoAction(EnemyState state);


        #region ShootMethod
        protected IEnumerator ShootRoutine(Vector2 pos, Quaternion Rot, int projIndex, float delay)
        {
            //gunTipRot, gunTipPos 업데이트
            Vector2 gunTipPos = pos;
            //Quaternion gunTipRot = Rot;

            //플레이어 움직임 반영
            PlayerBehavior pb = GameManager.Instance.PlayerBehavior;
            Vector2 playerPredictedPos = (Vector2)pb.transform.position + (pb.playerVelocity * predictionAccuracy);
            Vector2 upVec = Quaternion.Euler(0, 0, 90) * (playerPredictedPos - (Vector2)transform.position).normalized ;
            Quaternion gunTipRot = Quaternion.LookRotation(Vector3.forward, upVec);
            
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

            yield return new WaitForSeconds(delay);
        }


        #endregion

        #region DelayMethod
        protected IEnumerator DelayRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
        }

        #endregion


        #region AnimationEvents
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

            iconUI.SetActive(false);
        }

        public void AmbushStartEvent()
        {
            //잠복 시작 시 
            //충돌 처리 x 
            hitCollObject.SetActive(false);
            //gravity.activate = false;
        }

        public virtual void AmbushEndEvent()
        {
            //잠복 종료 시 
            //충돌 처리 o
            hitCollObject.SetActive(true);
            //gravity.activate = true;
            //애니메이션 실행 시작


        }

        public void AttackModeEvent()
        {
            if (EnemyAttackTransitionEvent != null)
                EnemyAttackTransitionEvent();
        }

        #endregion

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

        public void DelayedDying(float sec)
        {
            StartCoroutine(DyeRoutine(sec)); 
        }
        IEnumerator DyeRoutine(float sec)
        {
            yield return new WaitForSeconds(sec);
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            //drawString(preState.ToString(), transform.position, Color.green);
            UnityEditor.Handles.color = Color.white;
            UnityEditor.Handles.Label(transform.position, preState.ToString());
        }

        static void drawString(string text, Vector3 worldPos, Color? colour = null)
        {
            UnityEditor.Handles.BeginGUI();
            if (colour.HasValue) GUI.color = colour.Value;
            var view = UnityEditor.SceneView.currentDrawingSceneView;
            Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
            Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
            GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
            UnityEditor.Handles.EndGUI();
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

