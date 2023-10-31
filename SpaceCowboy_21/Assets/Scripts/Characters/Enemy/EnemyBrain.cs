using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceEnemy
{
    [SelectionBase]

    public class EnemyBrain : Enemy
    {
        public EnemyState enemyState = EnemyState.Idle;

        [Space]

        //필요한 내용
        public float timeBetweenChecks = 0.5f;
        public float checkRange;        //감지 거리
        public float attackRange;

        public bool attackRangeOn;      //공격 사정거리 체크
        public bool visionOn;           //시야 체크 
        public bool filpToPlayerOn;     //캐릭터가 있는 방향으로 회전 할 것인가?

        float lastCheckTime;
        public float playerDistance { get; set; }     //플레이어와의 거리를 기록해서 다른 행동을 할 수 있도록.
        public Vector2 playerDirection;
        public Transform playerTr;



        protected override void Start()
        {
            coll = GetComponent<Collider2D>();  
            playerTr = GameManager.Instance.player;

            GameManager.Instance.PlayerDeadEvent += PlayerIsDead;

        }

        // Update is called once per frame
        protected override void Update()
        {
            if (!activate)
                return;

            //죽었으면 더이상 작동하지 않는다. 
            if (enemyState == EnemyState.Die)
                return;

            //플레이어의 거리를 가져와서 얼마나 떨어져있는지 계산한다. 
            playerDistance = (playerTr.position - transform.position).magnitude;

            //timeBetweenChecks마다 감지한다
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
                //플레이어가 check거리 안이다
                //attackRange체크를 하나요?
                if (attackRangeOn)
                {
                    AttackRangeCheck();
                }
                else
                {
                    //사정거리 체크가 필요없으면 공격해라
                    enemyState = EnemyState.Attack;
                }

            }
            else
            {
                //플레이가 check거리 밖이면
                //대기 상태
                if (enemyState != EnemyState.Idle) { enemyState = EnemyState.Idle; }
                
            }
        }

        void AttackRangeCheck()
        {
            //플레이어가 사정거리 안인지 체크한다
            if (playerDistance < attackRange)
            {
                if(visionOn)
                {
                    VisionCheck();
                }
                else
                {
                    //사정거리 안인데 시야 체크가 필요없다면 공격해라
                    enemyState = EnemyState.Attack;
                }
            }
            else
            {
                //사정거리 체크를 했는데 사정거리 안이 아니면 추격해라
                enemyState = EnemyState.Chase;
            }

        }


        void VisionCheck()
        {
            //플레이어가 시야에 보이는지 체크한다
            Vector2 playerVector = playerTr.position - transform.position;
            playerDirection = playerVector.normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, playerDirection, playerDistance, LayerMask.GetMask("Planet"));
            if(hit.collider != null)
            {
                //적과 플레이어 사이에 장애물이 있다
                enemyState = EnemyState.Chase;

            }
            else
            {
                //적과 플레이어 사이에 장애물이 없다
                enemyState = EnemyState.Attack;
            }
        }



        public override void DamageEvent(float dmg)
        {
            if (enemyState == EnemyState.Die)
                return;


            //데미지를 적용
            if (health.AnyDamage(dmg))
            {
                //피격 이펙트를 적용
                action.HitEvent();
            }

            if (health.IsDead())
            {
                //true 인 경우 체력이 0 이하로 떨어졌다는 뜻.
                enemyState = EnemyState.Die;
                DeathEvent();
                return;
            }


        }

        /*
        public void KnockBackEvent(Vector2 objPos)
        {
            //가드가 켜져 있으면 넉백 하지 않는다. 
            if (guardON)
                return;

            action.EnemyKnockBack(objPos);
        }
        */


        public override void DeathEvent()
        {
            //적을 비활성화 한다.
            
            action.DieAction();
            coll.enabled = false;

        }

        public override void DeadActive()
        {
            //구지 View에서 여기로 죽는 애니메이션이 끝나면 신호를 줘서 비활성화 시킴.
            this.gameObject.SetActive(false);
        }



        public override void PlayerIsDead()
        {            
            //플레이어가 죽으면 델리케이트를 통해 발동

            activate = false;
            enemyState = EnemyState.Idle;
        }



        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, checkRange);

            if (attackRangeOn)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, attackRange);
            }

        }

    }


    public enum EnemyState
    {
        //Sleep, //비활성화
        Idle, //활성화 + 플레이어 감지 x
        Chase, //추격중
        Attack, //공격 상태
        Stun, //스턴 상태
        Die     //죽었을 때
    }


}


