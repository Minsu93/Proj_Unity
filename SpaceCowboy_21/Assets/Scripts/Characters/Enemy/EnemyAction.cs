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

        bool isAimOn = false;   //���� ���ΰ���

        protected EnemyBrain brain { get; set; }
        protected CharacterGravity gravity;
        protected Rigidbody2D rb;
        public GameObject hitCollObject;    //�ǰ� ó���� �ϴ� Collider

        //�߿�
        EnemyState preState = EnemyState.Die;

        [Header("Icon Property")]
        public GameObject iconUI;

        public bool faceRight { get; set; }  //ĳ���Ͱ� �������� ���� �ֽ��ϱ�? 
        public bool attackOn { get; set; }  //�������� �� 
        public bool attackCool { get; set; }    //���� ��Ÿ��
        public bool onAir { get; set; } //���߿� �ִ°�
        protected bool activate;  //Ȱ��ȭ


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

        //���°� �ٲ� �� �ѹ��� ���� 
        public abstract void DoAction(EnemyState state);


        #region ShootMethod
        protected IEnumerator ShootRoutine(Vector2 pos, Quaternion Rot, int projIndex, float delay)
        {
            //gunTipRot, gunTipPos ������Ʈ
            Vector2 gunTipPos = pos;
            //Quaternion gunTipRot = Rot;

            //�÷��̾� ������ �ݿ�
            PlayerBehavior pb = GameManager.Instance.PlayerBehavior;
            Vector2 playerPredictedPos = (Vector2)pb.transform.position + (pb.playerVelocity * predictionAccuracy);
            Vector2 upVec = Quaternion.Euler(0, 0, 90) * (playerPredictedPos - (Vector2)transform.position).normalized ;
            Quaternion gunTipRot = Quaternion.LookRotation(Vector3.forward, upVec);
            
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
            //�ẹ ���� �� 
            //�浹 ó�� x 
            hitCollObject.SetActive(false);
            //gravity.activate = false;
        }

        public virtual void AmbushEndEvent()
        {
            //�ẹ ���� �� 
            //�浹 ó�� o
            hitCollObject.SetActive(true);
            //gravity.activate = true;
            //�ִϸ��̼� ���� ����


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

