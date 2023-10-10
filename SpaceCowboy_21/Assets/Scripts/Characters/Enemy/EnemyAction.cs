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
            //normal�� transform.upward ������ �� ���̰� ũ�� ������ �����ش�. 
            rotateAngle = Vector2.Angle(transform.up, normal);
            
            turnspeedMultiplier = Mathf.Clamp(Mathf.RoundToInt(rotateAngle * 0.1f), 1, 10);

            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnspeedMultiplier * turnSpeed * Time.deltaTime);

        }




        #region ChaseMethods




        public void MoveToVisiblePoint(Vector2 playerPos, float maxTime)
        {
            //���� �÷��̾ ���̴� Collider2D ���� ����Ʈ���� ���� �˾Ƴ���.
            List<int> visiblePoints = new List<int>();
            int pointCounts = gravity.nearestEdgeCollider.pointCount - 1;
            int targetIndex = -1;

            for(int i = 0; i < pointCounts; i++)
            {
                //���ݸ� ����. i�� ¦���� ���.
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
                //visiblePoints�� null�̸� �������� �ʴ´�. 
                brain.enemyState = EnemyState.Idle;
                return;
            }



            //�� ĳ������ ���� ��ġ�� ���Ѵ�. 
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



            // �÷��̾�� ����� ����Ʈ���� ���Ѵ�. 
            List<int> closePointsInVisiblePoints = new List<int>();


            for (int i = 0; i < 6 ; i++)
            {
                int min = int.MaxValue; // ��������� �ּڰ��� ������ ������ �ʱ�ȭ�մϴ�.
                int minIndex = -1; // �ּڰ��� �ε����� ������ ������ �ʱ�ȭ�մϴ�.

                // ����Ʈ�� ��ȸ�ϸ鼭 �ּڰ��� ã���ϴ�.
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

                // �ּڰ��� ã�����Ƿ� ��� ����Ʈ�� �߰��ϰ� ���� ����Ʈ���� �����մϴ�.
                if (minIndex != -1)
                {
                    closePointsInVisiblePoints.Add(visiblePoints[minIndex]);
                    visiblePoints.RemoveAt(minIndex);
                }
            }



            //���� ���� ����Ʈ�� ���Ѵ�
            int randomInt = Random.Range(0, closePointsInVisiblePoints.Count);
            targetIndex = closePointsInVisiblePoints[randomInt];






            //targetPoint �� �̵��Ѵ�.
            int dirIndex = 1;
            int positive = (targetIndex - closestIndex + pointCounts) % pointCounts; 
            int negative = (closestIndex - targetIndex + pointCounts) % pointCounts;

            //+����(positive)�� ������ 1, -����(negative)�� ������ -1
            dirIndex = positive < negative ? 1 : -1;


            //�̵�
            StartCoroutine(MoveRoutine(targetIndex, closestIndex, dirIndex, pointCounts, maxTime));
        }





        IEnumerator MoveRoutine(int targetIndex, int closestIndex, int dirIndex, int pointCounts, float maxTime)
        {   //Ÿ���� �ִ� MoveRoutine
            int currIndex = closestIndex;
            int nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
            float currTime = Time.time;

            while (Time.time - currTime < maxTime)
            {
                //currTime += Time.deltaTime;

                Vector2 targetPointPos = GetPointPos(nextIndex);
                Vector2 pastPointPos = GetPointPos(currIndex);

                //������ ����� �븻�� ���Ѵ�.
                Vector2 direction = targetPointPos - pastPointPos;
                Vector2 normal = Vector2.Perpendicular(direction).normalized * dirIndex;
                
                //���� ������ ��Ҵ� Ÿ�� + �븻�������� Ű��ŭ ������ �ִ� ���
                Vector2 movePos = targetPointPos + (normal * enemyHeight);

                Vector2 moveDir = (movePos - rb.position).normalized;
                float moveDist = (movePos - rb.position).magnitude;
                
                
                
                Debug.DrawRay(rb.position, movePos - rb.position, Color.cyan, 0.5f);

                // ������Ʈ�� �̵� �������� �̵�
                rb.MovePosition(rb.position + moveDir * chaseSpeed * Time.fixedDeltaTime);


                // ������ �Ÿ��� ���� ����������� Ÿ���� �ٲ۴�.
                if (moveDist < chaseSpeed * Time.fixedDeltaTime)
                {
                    currIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
                    //targetIndex�� ���������� �����Ѵ�.
                    if (currIndex == targetIndex)
                    {
                        brain.enemyState = EnemyState.Idle;
                        yield break;
                    }

                    nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;

                }

                yield return null;
            }

            //maxTime�� �ʰ��ϸ�
            brain.enemyState = EnemyState.Idle;
            yield break;
        }



        IEnumerator MoveRoutine(int closestIndex, int dirIndex, int pointCounts, float maxTime, float speed)
        {   //Ÿ���� ���� MoveRoutine
            int currIndex = closestIndex;
            int nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
            float currTime = Time.time;

            while (Time.time - currTime < maxTime)
            {
                //currTime += Time.deltaTime;

                Vector2 targetPointPos = GetPointPos(nextIndex);
                Vector2 pastPointPos = GetPointPos(currIndex);

                //������ ����� �븻�� ���Ѵ�.
                Vector2 direction = targetPointPos - pastPointPos;
                Vector2 normal = Vector2.Perpendicular(direction).normalized * dirIndex;
                //���� ������ ��Ҵ� Ÿ�� + �븻�������� Ű��ŭ ������ �ִ� ���
                Vector2 movePos = targetPointPos + (normal * enemyHeight);

                Vector2 moveDir = (movePos - rb.position).normalized;
                float moveDist = (movePos - rb.position).magnitude;



                Debug.DrawRay(rb.position, movePos - rb.position, Color.blue, 0.5f);

                // ������Ʈ�� �̵� �������� �̵�
                rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);


                // ������ �Ÿ��� ���� ����������� Ÿ���� �ٲ۴�.
                if (moveDist < speed * Time.fixedDeltaTime)
                {
                    currIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
                    nextIndex = (currIndex + dirIndex + pointCounts) % pointCounts;
                }

                yield return null;
            }

            //maxTime�� �ʰ��ϸ�
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
            //������, ���� ���� ��ƾ���� �����Ѵ�. 
            if (gravity == null)
                return;

            StopAllCoroutines();
            brain.enemyState = EnemyState.Stun;

            //���� �κ��� ĳ���� �������� �������� ���Ѵ�
            Vector2 hitVec = hitPoint - (Vector2)transform.position;

            int dirInt = Vector2.SignedAngle(transform.up, hitVec) < 0 ? -1 : 1;

            int pointCounts = gravity.nearestEdgeCollider.pointCount - 1;

            //ĳ������ ��ġ�� ���Ѵ�. 
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

            //���� �κ��� �ݴ�������� ĳ���͸� �� �ʰ� �̵��ñ��

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

