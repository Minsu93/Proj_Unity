using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceEnemy
{
    [SelectionBase]

    public class EnemyBrain : MonoBehaviour
    {
        public EnemyState enemyState = EnemyState.Idle;

        public bool activate;

        [Space]

        //�ʿ��� ����
        public float timeBetweenChecks = 0.5f;
        public float checkRange;        //���� �Ÿ�
        public float attackRange;

        public bool attackRangeOn;      //���� �����Ÿ� üũ
        public bool visionOn;           //�þ� üũ 
        public bool filpToPlayerOn;     //ĳ���Ͱ� �ִ� �������� ȸ�� �� ���ΰ�?

        float lastCheckTime;
        public float playerDistance { get; set; }     //�÷��̾���� �Ÿ��� ����ؼ� �ٸ� �ൿ�� �� �� �ֵ���.
        public Vector2 playerDirection;
        public Transform playerTr;


        Health health;
        EnemyAction action;
        Collider2D coll;

        private void OnValidate()
        {
            if(health == null)
                health = GetComponent<Health>();
            health.ResetHealth();

            if(action == null)
                action = GetComponent<EnemyAction>();
        }

        private void Start()
        {
            coll = GetComponent<Collider2D>();  
            playerTr = GameManager.Instance.player;

            GameManager.Instance.PlayerDeadEvent += PlayerIsDead;
            activate = true;

        }

        // Update is called once per frame
        void Update()
        {
            if (!activate)
                return;

            //�׾����� ���̻� �۵����� �ʴ´�. 
            if (enemyState == EnemyState.Die)
                return;

            //�÷��̾��� �Ÿ��� �����ͼ� �󸶳� �������ִ��� ����Ѵ�. 
            playerDistance = (playerTr.position - transform.position).magnitude;

            //timeBetweenChecks���� �����Ѵ�
            if (Time.time - lastCheckTime > timeBetweenChecks)
            {
                lastCheckTime = Time.time;
            }
            else
            {
                return;
            }


            switch (enemyState)
            {
                case EnemyState.Idle:
                    CheckRangeCheck();
                    break;

                case EnemyState.Chase:
                    return;

                case EnemyState.Attack:
                    return;

                case EnemyState.Stun:
                    return;

            }

        }

        void CheckRangeCheck()
        {

            if(playerDistance <= checkRange)
            {
                //�÷��̾ check�Ÿ� ���̴�
                //attackRangeüũ�� �ϳ���?
                if (attackRangeOn)
                {
                    AttackRangeCheck();
                }
                else
                {
                    //�����Ÿ� üũ�� �ʿ������ �����ض�
                    enemyState = EnemyState.Attack;
                }

            }
            else
            {
                //�÷��̰� check�Ÿ� ���̸�
                //��� ����
                if (enemyState != EnemyState.Idle) { enemyState = EnemyState.Idle; }
                
            }
        }

        void AttackRangeCheck()
        {
            //�÷��̾ �����Ÿ� ������ üũ�Ѵ�
            if (playerDistance < attackRange)
            {
                if(visionOn)
                {
                    VisionCheck();
                }
                else
                {
                    //�����Ÿ� ���ε� �þ� üũ�� �ʿ���ٸ� �����ض�
                    enemyState = EnemyState.Attack;
                }
            }
            else
            {
                //�����Ÿ� üũ�� �ߴµ� �����Ÿ� ���� �ƴϸ� �߰��ض�
                enemyState = EnemyState.Chase;
            }

        }

        void VisionCheck()
        {
            //�÷��̾ �þ߿� ���̴��� üũ�Ѵ�
            Vector2 playerVector = playerTr.position - transform.position;
            playerDirection = playerVector.normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, playerDistance, LayerMask.GetMask("Planet"));
            if(hit.collider != null)
            {
                //���� �÷��̾� ���̿� ��ֹ��� �ִ�
                enemyState = EnemyState.Chase;

            }
            else
            {
                //���� �÷��̾� ���̿� ��ֹ��� ����
                enemyState = EnemyState.Attack;
            }
        }


        public void DamageEvent(float dmg)
        {
            if (enemyState == EnemyState.Die)
                return;


            //�������� ����
            if (health.AnyDamage(dmg))
            {
                //�ǰ� ����Ʈ�� ����
                action.HitEvent();
            }

            if (health.IsDead())
            {
                //true �� ��� ü���� 0 ���Ϸ� �������ٴ� ��.
                enemyState = EnemyState.Die;
                DeathEvent();
                return;
            }


        }

        /*
        public void KnockBackEvent(Vector2 objPos)
        {
            //���尡 ���� ������ �˹� ���� �ʴ´�. 
            if (guardON)
                return;

            action.EnemyKnockBack(objPos);
        }
        */


        public void DeathEvent()
        {
            //���� ��Ȱ��ȭ �Ѵ�.
            
            action.DieEvent();
            coll.enabled = false;

        }

        public void DeadActive()
        {
            //���� View���� ����� �״� �ִϸ��̼��� ������ ��ȣ�� �༭ ��Ȱ��ȭ ��Ŵ.
            this.gameObject.SetActive(false);
        }



        public void PlayerIsDead()
        {            
            //�÷��̾ ������ ��������Ʈ�� ���� �ߵ�

            activate = false;
            enemyState = EnemyState.Idle;
        }




    }

    public enum EnemyState
    {
        //Sleep, //��Ȱ��ȭ
        Idle, //Ȱ��ȭ + �÷��̾� ���� x
        Chase, //�߰���
        Attack, //���� ����
        Stun, //���� ����
        Die     //�׾��� ��
    }
    
}
