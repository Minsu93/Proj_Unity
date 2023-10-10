using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore;

namespace SpaceEnemy
{
    public abstract class EnemyAction : MonoBehaviour
    {
        [Header("Move Property")]
        public float chaseSpeed;
        public float enemyHeight = 0.51f;
        public float turnSpeedOnLand = 700f;
        float rotateAngle;

        [Header("KnockBack Property")]
        public float knockBackSpeed;
        public float knockBackTime = 0.5f;

        [Header("Attack Property")]
        public float chargeTime = 1.0f;
        public float shootDelay = 0.1f;
        public float afterShootDelay = 3.0f;


        [Header("Projectile Property")]
        public GameObject projectilePrefab;
        public Transform gunTip;
        public float randomSpreadAngle;
        public float damage;
        public float lifeTime;
        public float projectileSpeed;

        EnemyState preEnemyState;

        public EnemyBrain brain;
        CharacterGravity gravity;

        Rigidbody2D rb;

        public event System.Action EnemyShootEvent;
        public event System.Action EnemyStartAimEvent;
        public event System.Action EnemyStopAImEvent;
        public event System.Action EnemyStartGuardEvent;
        public event System.Action EnemyStopGuardEvent;
        public event System.Action EnemyHitEvent;
        public event System.Action EnemyDieEvent;



        protected virtual void Awake()
        {
            if (gravity == null)
                gravity = GetComponent<CharacterGravity>();
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (gunTip == null)
                gunTip = transform;
        }


        protected virtual void Update()
        {
            EnemyState currEnemyState = brain.enemyState;

            if(currEnemyState == EnemyState.Die)
            {
                return;
            }

            EnemyStateAction(currEnemyState);

            if (gravity == null)
                return;

            RotateCharacterToGround();
        }

        void EnemyStateAction(EnemyState curr)
        {
            
            if (curr != preEnemyState)
            {
                preEnemyState = curr;

                switch (curr)
                {
                    case EnemyState.Chase:
                        ChaseAction();
                        break;

                    case EnemyState.Attack:
                        AttackAction();
                        break;


                }

            }
        }




        public abstract void ChaseAction();

        public abstract void AttackAction();

        public abstract void DieAction();





        void RotateCharacterToGround()
        {
            if (gravity.nearestPlanet == null)
                return;

            Vector2 upVec = ((Vector2)transform.position - gravity.nearestPoint).normalized;
            RotateToVector(upVec, turnSpeedOnLand);
        }

        void RotateToVector(Vector2 normal, float turnSpeed)
        {
            Vector3 vectorToTarget = normal;
            int turnspeedMultiplier = 1;
            //normal과 transform.upward 사이의 값 차이가 크면 보정을 가해준다. 
            rotateAngle = Vector2.Angle(transform.up, normal);
            
            turnspeedMultiplier = Mathf.Clamp(Mathf.RoundToInt(rotateAngle * 0.1f), 1, 10);

            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnspeedMultiplier * turnSpeed * Time.deltaTime);

        }




        #region ChaseMethods




        public void MoveToVisiblePoint(Vector2 playerPos, float maxTime)
        {
            //먼저 플레이어가 보이는 Collider2D 상의 포인트들의 값을 알아낸다.
            List<int> visiblePoints = new List<int>();
            int pointCounts = gravity.nearestEdgeCollider.pointCount - 1;
            int targetIndex = -1;

            for(int i = 0; i < pointCounts; i++)
            {
                //절반만 하자. i가 짝수면 통과.
                if (i % 2 == 0)
                    continue;

                Vector2 pointVector = GetPointPos(i);

                Vector2 edgeDirection = i < pointCounts ? gravity.nearestEdgeCollider.points[i + 1] - gravity.nearestEdgeCollider.points[i] : gravity.nearestEdgeCollider.points[i] - gravity.nearestEdgeCollider.points[i - 1];
                Vector2 normal = Vector2.Perpendicular(edgeDirection).normalized;

                pointVector = pointVector + (normal * enemyHeight);
                Vector2 dir = playerPos - pointVector;
                float dist = dir.magnitude;
                dir = dir.normalized;

                if (dist > brain.attackRange)
                    continue;

                RaycastHit2D hit = Physics2D.Raycast(pointVector, dir, dist, LayerMask.GetMask("Planet"));

                if (hit.collider != null)
                {
                    
                    Debug.DrawRay(pointVector, dir * dist, Color.blue, 1f);
                }
                else
                {
                    Debug.DrawRay(pointVector, dir * dist, Color.green, 1f);
                    visiblePoints.Add(i);
                }

            }


            if (visiblePoints.Count <= 0) 
            {
                //visiblePoints가 null이면 움직이지 않는다. 
                brain.enemyState = EnemyState.Idle;
                return;
            }



            //적 캐릭터의 현재 위치를 구한다. 
            float closestDistance = Mathf.Infinity;
            int closestIndex = -1;

            for (int i = 0; i < pointCounts; i++)
            {
                Vector2 worldEdgePoint = GetPointPos(i);
                float distance = Vector2.Distance(transform.position, worldEdgePoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }



            // 플레이어에게 가까운 포인트들을 구한다. 
            List<int> closePointsInVisiblePoints = new List<int>();


            for (int i = 0; i < 6 ; i++)
            {
                int min = int.MaxValue; // 현재까지의 최솟값을 저장할 변수를 초기화합니다.
                int minIndex = -1; // 최솟값의 인덱스를 저장할 변수를 초기화합니다.

                // 리스트를 순회하면서 최솟값을 찾습니다.
                for (int j = 0; j < visiblePoints.Count; j++)
                {
                    int pos = (visiblePoints[j] - closestIndex + pointCounts) % pointCounts;
                    int neg = (closestIndex - visiblePoints[j] + pointCounts) % pointCounts;
                    int betweenIndex = pos < neg ? pos : neg;


                    if (betweenIndex < min)
                    {
                        min = betweenIndex;
                        minIndex = j;
                    }
                }

                // 최솟값을 찾았으므로 결과 리스트에 추가하고 원래 리스트에서 제거합니다.
                if (minIndex != -1)
                {
                    closePointsInVisiblePoints.Add(visiblePoints[minIndex]);
                    visiblePoints.RemoveAt(minIndex);
                }
            }



            //이중 랜덤 포인트를 구한다
            int randomInt = Random.Range(0, closePointsInVisiblePoints.Count);
            targetIndex = closePointsInVisiblePoints[randomInt];






            //targetPoint 로 이동한다.
            int dirIndex = 1;
            int positive = (targetIndex - closestIndex + pointCounts) % pointCounts; 
            int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;

            //+방향(positive)이 가까우면 1, -방향(negative)이 가까우면 -1
            dirIndex = positive < negative ? 1 : -1;


            //이동
            StartCoroutine(MoveRoutine(targetIndex, closestIndex, dirIndex, pointCounts, maxTime));
        }





        IEnumerator MoveRoutine(int targetIndex, int closestIndex, int dirIndex, int pointCounts, float maxTime)
        {   //타겟이 있는 MoveRoutine
            int currIndex = closestIndex;
            int nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
            float currTime = Time.time;

            while (Time.time - currTime < maxTime)
            {
                //currTime += Time.deltaTime;

                Vector2 targetPointPos = GetPointPos(nextIndex);
                Vector2 pastPointPos = GetPointPos(currIndex);

                //움직일 장소의 노말을 구한다.
                Vector2 direction = targetPointPos - pastPointPos;
                Vector2 normal = Vector2.Perpendicular(direction).normalized * dirIndex;
                
                //최종 움직일 장소는 타겟 + 노말방향으로 키만큼 높은데 있는 장소
                Vector2 movePos = targetPointPos + (normal * enemyHeight);

                Vector2 moveDir = (movePos - rb.position).normalized;
                float moveDist = (movePos - rb.position).magnitude;
                
                
                
                Debug.DrawRay(rb.position, movePos - rb.position, Color.cyan, 0.5f);

                // 오브젝트를 이동 방향으로 이동
                rb.MovePosition(rb.position + moveDir * chaseSpeed * Time.fixedDeltaTime);


                // 움직일 거리가 거의 가까워졌으면 타겟을 바꾼다.
                if (moveDist < chaseSpeed * Time.fixedDeltaTime)
                {
                    currIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
                    //targetIndex에 도착했으면 중지한다.
                    if (currIndex == targetIndex)
                    {
                        brain.enemyState = EnemyState.Idle;
                        yield break;
                    }

                    nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;

                }

                yield return null;
            }

            //maxTime을 초과하면
            brain.enemyState = EnemyState.Idle;
            yield break;
        }



        IEnumerator MoveRoutine(int closestIndex, int dirIndex, int pointCounts, float maxTime, float speed)
        {   //타겟이 없는 MoveRoutine
            int currIndex = closestIndex;
            int nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
            float currTime = Time.time;

            while (Time.time - currTime < maxTime)
            {
                //currTime += Time.deltaTime;

                Vector2 targetPointPos = GetPointPos(nextIndex);
                Vector2 pastPointPos = GetPointPos(currIndex);

                //움직일 장소의 노말을 구한다.
                Vector2 direction = targetPointPos - pastPointPos;
                Vector2 normal = Vector2.Perpendicular(direction).normalized * dirIndex;
                //최종 움직일 장소는 타겟 + 노말방향으로 키만큼 높은데 있는 장소
                Vector2 movePos = targetPointPos + (normal * enemyHeight);

                Vector2 moveDir = (movePos - rb.position).normalized;
                float moveDist = (movePos - rb.position).magnitude;



                Debug.DrawRay(rb.position, movePos - rb.position, Color.blue, 0.5f);

                // 오브젝트를 이동 방향으로 이동
                rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);


                // 움직일 거리가 거의 가까워졌으면 타겟을 바꾼다.
                if (moveDist < speed * Time.fixedDeltaTime)
                {
                    currIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
                    nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
                }

                yield return null;
            }

            //maxTime을 초과하면
            brain.enemyState = EnemyState.Idle;
            yield break;
        }



        Vector2 GetPointPos(int pointIndex)
        {
            Vector3 localPoint = gravity.nearestEdgeCollider.points[pointIndex];
            Vector2 pointPos = gravity.nearestEdgeCollider.transform.TransformPoint(localPoint);
            return pointPos;
        }




        #endregion



        public void EnemyKnockBack(Vector2 hitPoint)
        {
            //움직임, 슈팅 등의 루틴들을 정지한다. 
            if (gravity == null)
                return;

            StopAllCoroutines();
            brain.enemyState = EnemyState.Stun;

            //맞은 부분이 캐릭터 앞쪽인지 뒷쪽인지 구한다
            Vector2 hitVec = hitPoint - (Vector2)transform.position;

            int dirInt = Vector2.SignedAngle(transform.up, hitVec) < 0 ? -1 : 1;

            int pointCounts = gravity.nearestEdgeCollider.pointCount - 1;

            //캐릭터의 위치를 구한다. 
            float closestDistance = Mathf.Infinity;
            int closestIndex = -1;

            for (int i = 0; i < pointCounts; i++)
            {
                Vector2 worldEdgePoint = GetPointPos(i);
                float distance = Vector2.Distance(transform.position, worldEdgePoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            //맞은 부분의 반대방향으로 캐릭터를 몇 초간 이동시긴다

            StartCoroutine(MoveRoutine(closestIndex, dirInt, pointCounts, knockBackTime, knockBackSpeed));

        }





        #region AnimationEvents

        protected void ShootEvent()
        {
            if (EnemyShootEvent != null)
                EnemyShootEvent(); 
        }
        protected void StartAimEvent()
        {
            if (EnemyStartAimEvent != null)
                EnemyStartAimEvent();
        }
        protected void StopAimEvent()
        {
            if (EnemyStopAImEvent != null)
                EnemyStopAImEvent();
        }

        protected virtual void StartGuardEvent()
        {
            if (EnemyStartGuardEvent != null)
                EnemyStartGuardEvent();
        }
        protected virtual void StopGuardEvent()
        {
            if (EnemyStopGuardEvent != null)
                EnemyStopGuardEvent();
        }

        public void HitEvent()
        {
            if (EnemyHitEvent != null)
                EnemyHitEvent();
        }

        public void DieEvent()
        {

            StopAllCoroutines();

            if (EnemyDieEvent != null)
                EnemyDieEvent();
        }

        protected IEnumerator DelayRoutine(float delay)
        {
            yield return new WaitForSeconds(delay);
        }

        #endregion
    }
}

