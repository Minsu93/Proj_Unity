using SpaceEnemy;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore;

namespace SpaceCowboy
{
    public class PlayerBehavior : MonoBehaviour
    {
        [Header("Jump Property")]
        public float jumpForce = 10f;
        public float moveForce = 1f;    //PreMoveDir�� ������ ������ ��ȭ��Ų��.
        Vector2 pastPosition = Vector2.zero;
        float lastJumpTime;     //���� �� ��õ��� onAir ���� ����.
        bool onFalling; //�������� �ִ� ��.
        bool airJump;   //���� ������ �������� ���� 
        Vector2 preJumpVec; //���� ���� ����

        [Header("Aim Property")]
        public Vector3 mousePos;
        public Vector3 aimDirection;
        //public Transform gunTip;

        [Header("PlayerProperty")]
        public PlayerState state = PlayerState.Idle;
        Vector2 preInputAxis = Vector2.zero;  //���� �����ӿ� �����ߴ� ��ǲ ��
        Vector2 moveDir = Vector2.zero;     //�̵����� ����
        public float moveSpeed;
        public float acceleration = 0.25f;      //�ӵ��� �ٲ� �� �󸶳� ������ ������.
        float currSpeed;    //���� �̵� �ӵ�
        float targetSpeed;  //�̵� ��ǥ�� �� ���ǵ�
        public float runSpeedMultiply = 2f;
        
        [Space]

        public float airMoveSpeed;
        public float turnSpeedOnLand = 100f;
        Vector2 inputAxis = Vector2.zero;  //������ ��ǲ ��
        [SerializeField] bool OnAir;      //����� ������ �ν����Ϳ��� ���̰�
        public bool onSpace { get; private set; }   //���ֿ� �ֽ��ϱ�?
        public bool onLessGravity { get; private set; } //���� �߷¿� �ֽ��ϱ�?
        public bool faceRight;
        bool reserveChangeFaceRight;        // �༺�� �ٲ�� ������ �� faceRight�� �ʱ�ȭ �����Ѵ�.
        public bool runON { get; private set; }

        [Header("EdgeFollow")]
        public float boasterForce;      //�ν����� �Ŀ�

        [Header("EdgeFollow")]
        int currentEdgePointIndex = 0; // ���� ���󰡰� �ִ� ���� �ݶ��̴��� �� �ε���

        public event System.Action ShootEvent;
        public event System.Action StartAimEvent;
        public event System.Action StopAimEvent;
        public event System.Action PlayerHitEvent;
        public event System.Action PlayerDieEvent;
        public event System.Action PlayerReloadEvent;
        public event System.Action PlayerStopReloadEvent;


        //public event System.Action PlayerChangeState;

        //�ѱ� ����

        float gunRecoil;

        

        PlayerWeapon playerWeapon;
        public  PlayerView playerView;
        CharacterGravity characterGravity;
        GravityLassoAdvance lassoAdvance;
        Rigidbody2D rb;
        Health health;
        Collider2D coll;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            characterGravity = GetComponent<CharacterGravity>();
            playerWeapon = GetComponent<PlayerWeapon>();
            lassoAdvance = GetComponentInChildren<GravityLassoAdvance>();
            //playerView = GetComponent<PlayerView>();
            health = GetComponent<Health>();
            coll = GetComponent<Collider2D>();

            health.ResetHealth();
        }



        // Update is called once per frame
        void Update()
        {
            if (state == PlayerState.Die)
                return;

            //ĳ���Ͱ� ���� ���ֿ� �ִ��� üũ
            CheckOnSpace();
            

            //���߿� �ִ��� üũ
            CheckOnAir();

            //���� ���� üũ
            //AimCheck();
        }



        private void FixedUpdate()
        {
            if (state == PlayerState.Die)
                return;

            //�÷��̾� ���� üũ
            //Vector2 height = characterGravity.nearestPoint - (Vector2)transform.position;
            //Debug.Log(height.magnitude);

            //ĳ���� ȸ��
            RotateCharacterToGround();


            if (lassoAdvance.lassoState == LassoState.OnMove) return;
            if (state == PlayerState.Stun) return;


            //���� ������ �Լ�
            MoveUpdate();


            //�̵� �ӵ��� ��Ʈ��
            SpeedControl();

            //���� �ν��� ����
            //AIrJumpFunction();



        }


        void RotateCharacterToGround()
        {
            /*
            //�ǹ� �� �� ���� �߷¹��⿡ ���� �� 
            if (characterGravity.fixGravityOn)
            {
                RotateToVector(gravityFinder.fixGravityDir * -1f, turnSpeedOnLand);
                return;
            }
            */

            
            if (onSpace)
                return;

            Vector2 upVec = ((Vector2)transform.position - characterGravity.nearestPoint).normalized;
            RotateToVector(upVec, turnSpeedOnLand);

        }

        void RotateToVector(Vector2 normal, float turnSpeed)
        {
            Vector3 vectorToTarget = normal;
            int turnspeedMultiplier = 1;
            //normal�� transform.upward ������ �� ���̰� ũ�� ������ �����ش�. 
            float rotateAngle = Vector2.Angle(transform.up, normal);

            turnspeedMultiplier = Mathf.Clamp(Mathf.RoundToInt(rotateAngle * 0.1f), 1, 10);

            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: vectorToTarget);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnspeedMultiplier * turnSpeed * Time.deltaTime);
        }

        void CheckOnAir()
        {
            if (state != PlayerState.Jumping) return;
            
            //üũ ������ �� �����̸� ����. 
            if (Time.time - lastJumpTime < 0.1f)
                return;

            //������ ������ ����ϱ�
            Vector2 jumpDir = rb.position - pastPosition;
            Vector2 upDir = transform.up;
            float angle = Vector2.Angle(upDir, jumpDir);
            if(angle > 90 && !onFalling)
            {
                onFalling = true;
                
            }

            //ĳ������ �߽ɿ���, �� ������ ���̸� ���� ĳ���Ͱ� ���߿� �� �ִ��� �˻��Ѵ�.
            RaycastHit2D footHit = Physics2D.CircleCast(transform.position, 0.5f, transform.up * -1, 1f, LayerMask.GetMask("Planet"));
            if (footHit.collider != null)
            {
                if (footHit.distance < 0.1f)
                {   //���� �� �ѹ� ����
                    OnAir = false;
                    onFalling = false;
                    rb.velocity = Vector2.zero;
                    if (airJump) airJump = false;

                    state = PlayerState.Idle;
                    GetClosestPoint();
                    if (reserveChangeFaceRight)
                    {
                        preInputAxis = inputAxis;
                        faceRight = Vector2.SignedAngle(transform.up, inputAxis) < 0 ? true : false;
                        reserveChangeFaceRight = false;
                    }
                }
                else
                {
                    OnAir = true;
                }
            }

            Debug.DrawLine((Vector2)transform.position, (Vector2)transform.position + jumpDir.normalized, Color.white);
            pastPosition = rb.position;
    
        }

        void CheckOnSpace()
        {
            //�÷��̾ ���ֿ� �ִ��� �����ؼ� onSpace�� ����
            bool pre = onSpace;

            if( characterGravity.nearestPlanet == null)
            {
                pre = true;
            }
            else
            {
                pre = false;
                //���߷� üũ
                if(characterGravity.nearestPlanet.gravityForce < 400f)
                {
                    onLessGravity = true;
                }
                else
                {
                    onLessGravity = false;
                }
            }
            //onSpace = characterGravity.nearestPlanet == null ? true : false;

            //���ֿ� �ִ°� �޶����� �ִϸ��̼� ����
            if (pre != onSpace)
                playerView.ChangeState();

            onSpace = pre;

        }

        public void TryJump()
        {
            if (state == PlayerState.Die) return;

            if (state == PlayerState.Stun) return;

            if (state == PlayerState.Jumping)
            {
                if(OnAir)
                    StartAirJump();
                return;
            }

            state = PlayerState.Jumping;

            lastJumpTime = Time.time;

            //���󿡼� �������� ��. OnAir �� false�� ���¿��� ������.
            OnAir = true;

            Vector2 upVector = (Vector2)transform.position - characterGravity.nearestPoint;
            upVector = upVector.normalized;
            //Vector2 forwardVector = moveDir.normalized;     //moveDir(���� �̵� ����) �������� �������� ������Ű�Ƿ�, moveDir ��ġ�� õõ�� �ٿ��� ���� ���� �����ϴ� �͵� ���� �� �ִ�. 
            Vector2 forwardVector = faceRight ? transform.right : transform.right * -1;
            Vector2 jumpDir = upVector + (forwardVector * moveForce * currSpeed);
            jumpDir = jumpDir.normalized;
            preJumpVec = jumpDir;
            Vector2 jumpVector = jumpDir * jumpForce;

            rb.velocity = Vector2.zero;
            rb.AddForce(jumpVector, ForceMode2D.Impulse);

            //���� ���� ���
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        }


        public void StartAirJump()
        {
            airJump = true;

            //Vector2 jumpVector = preJumpVec * jumpForce * 0.8f;

            //rb.velocity = Vector2.zero;
            //rb.AddForce(jumpVector, ForceMode2D.Impulse);
        }

        public void StopJump()
        {
            airJump = false;
        }

        void AIrJumpFunction()
        {
            if (!airJump)
                return;

            rb.AddForce(transform.up * boasterForce);
        }


        #region Movement



        public void TryMove(Vector2 inputAxisRaw)
        {
            if (state == PlayerState.Die) return;
            //�÷��̾ �˹����϶��� �̵� ������ �Ұ���. 
            if (state == PlayerState.Stun) return;

            this.inputAxis = inputAxisRaw;

            //�ѹ����� ����ϴ°� �����ս��� ������, �ƴϸ� ���� ������ ����ִ°� �����ս��� ������ �𸣰���. ������ �˻�� �ؾ����ݾ�.. 

            if (runON)
            {
                targetSpeed = moveSpeed * runSpeedMultiply;
            }
            else
            {
                targetSpeed = moveSpeed;
            }


            //���� ���� ������ ������ 
            if (!OnAir)
            {
                MoveOnLand();
            }
            //���߿� ���� �� ������ 
            //else
            //{
            //   MoveOnAir();
            //}



        }

        void MoveOnLand()
        {
            //���� ���� �ٲ�
            if (inputAxis.x != preInputAxis.x || inputAxis.y != preInputAxis.y)
            {
                preInputAxis = inputAxis;

                faceRight = Vector2.SignedAngle(transform.up, inputAxis) < 0 ? true : false;
                // ������ �ٲ� ��, �� ������ ���� ����� ����Ʈ�� �����´�.
                GetClosestPoint();

                // ������ �ٲ� �� view���� Running�ִϸ��̼��� �ٲٱ� ���ؼ�    >> �ٸ� �������..
                playerView.ChangeState();
            }

            if (state != PlayerState.Running)
            {
                //�޸��� �����Ǹ鼭 �ѹ� ����.
                state = PlayerState.Running;
            }

        }
        /*
        void MoveOnAir()
        {
            //��ó�� �༺�� ���ٸ� ���� �̵��� �Ұ���.
            if (onSpace)
                return;

            //if (!onFalling) return;


            //���� ���� �ٲ�
            if (inputAxis.x != preInputAxis.x || inputAxis.y != preInputAxis.y)
            {
                preInputAxis = inputAxis;
                faceRight = Vector2.SignedAngle(transform.up, inputAxis) < 0 ? true : false;
            }

            Vector2 velocity = rb.velocity;
            Vector2 moveVec = transform.rotation * preInputAxis.normalized;
            velocity = velocity + moveVec * airMoveSpeed * Time.fixedDeltaTime;

            rb.velocity = velocity;
            //rb.MovePosition(rb.position + (preInputAxis.normalized * moveSpeed * Time.fixedDeltaTime));
            //rb.AddForce(preInputAxis.normalized * moveSpeed , ForceMode2D.Force);
        }
        */

        public void TryStop()
        {
            targetSpeed = 0;
        }




        //�̵��� ������ �� ����
        private void GetClosestPoint()
        {
            int closestPointIndex = 0;
            float closestDistance = float.MaxValue;

            // ���� �ݶ��̴��� ��� ����Ʈ���� ��ȸ�ϸ� ���� ����� ����Ʈ�� �ε����� �Ÿ��� ����
            for (int i = 0; i < characterGravity.nearestEdgeCollider.points.Length; i++)
            {
                Vector2 pointPosition = characterGravity.nearestEdgeCollider.transform.TransformPoint(characterGravity.nearestEdgeCollider.points[i]);
                bool right = Vector2.SignedAngle(transform.up, pointPosition - rb.position) < 0 ? true : false;
                if (faceRight != right) continue; //ĳ���� �������� �ٸ��� �����Ѵ�
                float distance = Vector2.Distance(rb.position, pointPosition);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPointIndex = i;
                }
            }
            currentEdgePointIndex = closestPointIndex;
        }


        public void ChangePlanet()  
        {   //�༺�� �ٲ�� right ������ ����. CharacterGravity���� ��ȣ�� �޴´�.
            reserveChangeFaceRight = true;
        }


        void SpeedControl()
        {

            if(currSpeed < targetSpeed)
            {
                currSpeed += acceleration;
            }
            else if(currSpeed > targetSpeed) 
            {
                currSpeed -= acceleration;
            }

            //�����̴� �߿� �ӵ��� 0���� ������� ��
            if(currSpeed < 0.1f && state == PlayerState.Running)
            {
                inputAxis = Vector2.zero;
                //preMoveDir = Vector2.zero;
                moveDir = Vector2.zero;

                state = PlayerState.Idle;

            }
        }


        void MoveUpdate()
        {
            if (currSpeed < acceleration)
                return;

            if (state == PlayerState.Running || state == PlayerState.Stun)
            {
                //������
                int faceInt;
                Vector2 targetPointPos;
                Vector2 pastPointPos;
                int pastPoint;
                Vector2 direction;
                Vector2 moveVector;
                float moveDist;
                EdgeCollider2D edge = characterGravity.nearestEdgeCollider;

                //��ǥ�� �� ����Ʈ�� ���Ѵ�.
                do
                {
                    //do�� ���� �ܼ�ȭ. MoveDist�� �־ �ȴ�. Edge Collider��.
                    faceInt = faceRight ? 1 : -1;
                    targetPointPos = edge.transform.TransformPoint(edge.points[currentEdgePointIndex % (edge.points.Length - 1)]);   //�ݶ��̴��� ������  point �� �� 0�� ����. �׷��� ����.

                    pastPoint = (currentEdgePointIndex - faceInt + (edge.points.Length - 1)) % (edge.points.Length - 1);
                    pastPointPos = edge.transform.TransformPoint(edge.points[pastPoint]);
                    direction = (targetPointPos - pastPointPos) * faceInt;
                    Vector2 normal = new Vector2(-direction.y, direction.x).normalized;

                    moveVector = targetPointPos + (normal * 0.51f);         //�÷��̾��� ���̴�. Ÿ�� ����Ʈ���� �븻���� * �÷��̾� ���̸� ��ǥ�� ������.
                    moveVector -= rb.position;
                    moveDist = moveVector.magnitude;

                    Debug.DrawRay(rb.position, moveVector, Color.cyan, 0.5f);


                    if (moveDist < currSpeed * Time.fixedDeltaTime)
                    {
                        currentEdgePointIndex = (currentEdgePointIndex + faceInt + (edge.points.Length - 1)) % ((edge.points.Length - 1));
                    }
                }
                while (moveDist < currSpeed * Time.fixedDeltaTime);



                moveDir = moveVector.normalized;
                float speed = currSpeed;

                if(lassoAdvance.lassoState == LassoState.OnGrab)
                {
                    float t = lassoAdvance.forceByLasso.magnitude;
                    speed = currSpeed - t;
                }

                //Debug.DrawLine(pastPointPos, targetPointPos, Color.red);
                // ������Ʈ�� �̵� �������� �̵�
                rb.MovePosition(rb.position + moveDir * speed * Time.fixedDeltaTime);

            }
        }


        #endregion


        #region KnockBack
        public void PlayerKnockBack(Vector2 hitPoint, float knockBackTime, float knockBackSpeed)
        {
            if (state == PlayerState.Die) return;
            if (state == PlayerState.Stun)
                return;

            state = PlayerState.Stun;
            //���� ���� ��� �����̶�� ��ƾ�� �ߴ��Ѵ�. 
            playerWeapon.TryStopShoot();

            //�÷��̾ �ڷ� �˹��Ų��. ����/������ �� ����..
            if (OnAir)
            {
                Debug.Log("Knockback On Air");
                Vector2 dir = (Vector2)transform.position - hitPoint;
                dir = dir.normalized;

                rb.AddForce(dir * knockBackSpeed * 2f, ForceMode2D.Impulse);
                StartCoroutine(KnockBackOnAirRoutine(knockBackTime));
            }
            else
            {
                Debug.Log("Knockback On Land");
                //������, ���� ���� ��ƾ���� �����Ѵ�. 
                //if (characterGravity == null) return;

                StartCoroutine(KnockBackRoutine(hitPoint, knockBackTime, knockBackSpeed));
            }


        }


        IEnumerator KnockBackRoutine(Vector2 hitPoint, float time, float speed)
        {
            yield return null;
            //aim�� �����Ѵ�.
            if(StopAimEvent != null)
                StopAimEvent();

            //StopAllCoroutines();

            //���� �κ��� ĳ���� �������� �������� ���Ѵ�
            Vector2 hitVec = hitPoint - (Vector2)transform.position;

            faceRight = Vector2.SignedAngle(transform.up, hitVec) < 0 ? false : true;

            //�ӵ� ����(�پ���� ����. Stun �� �Ǹ鼭 SpeedControl �� ������)
            currSpeed = speed;

            //ĳ������ ��ġ�� ���Ѵ�. 
            GetClosestPoint();


            //�÷��̾� Flip�� ���´�. 
            playerView.flipOn = false;


            //���� �ð����� �̵��Ѵ�. 
            float timer = 0f;
            while (timer < time)
            {
                timer += Time.deltaTime;

                yield return null;
            }
            //�÷��̾� Flip�� ȸ���Ѵ�. 
            playerView.flipOn = true;

            //playerState �� ȸ���Ѵ�. �����̴� ���¿��� �ڿ������� �ӵ��� �پ��� ���ؼ� running����. 
            state = PlayerState.Running;

            //������ �ʱ�ȭ�Ѵ�.
            inputAxis = Vector2.zero;
            preInputAxis = Vector2.zero;

            //aim�� ȸ���Ѵ�. 
            if (StartAimEvent != null)
                StartAimEvent();

        }


        IEnumerator KnockBackOnAirRoutine(float knockBackTime)
        {
            yield return new WaitForSeconds(knockBackTime);

            if (OnAir)
            {
                state = PlayerState.Jumping;

            }
            else
            {
                state = PlayerState.Idle;

            }
        }

        #endregion


        #region Damage and Die
        public void DamageEvent(float dmg)
        {
            if (state == PlayerState.Die)
                return;

           

            //�������� ����
            if (health.AnyDamage(dmg))
            {
                //�¾����� ���� ����
                if (PlayerHitEvent != null)
                {
                    PlayerHitEvent();
                }

                //CinemachineShake.instance.ShakeCamera(2f, 0.1f);
            }

            //�׾�����?
            if (health.IsDead())
            {
                DeadEvent();

                return;
            }

        }


        public void DeadEvent()
        {
            //true �� ��� ü���� 0 ���Ϸ� �������ٴ� ��.
            state = PlayerState.Die;

            //�״� ��� ����
            if (PlayerDieEvent != null)
            {
                PlayerDieEvent();
            }

            currSpeed = 0;
            targetSpeed = 0;
            //StopAllCoroutines();

            //aim�� �����Ѵ�. 
            if (StopAimEvent != null)
                StopAimEvent();

            //�浹 ������ �����
            //coll.enabled = false;

            //���� �̺�Ʈ �ߵ�. ���鿡�� ĳ���Ͱ� �׾��ٰ� ����. 
            GameManager.Instance.PlayerIsDead();
        }

        #endregion


        public void TryRun()
        {   //input���� ����
            runON = true;
            //���� ����
            if (StopAimEvent != null)
                StopAimEvent();

        }

        public void StopRun()
        {   //input���� ����
            runON = false;
            //���� �ٽ� ����
            if (StartAimEvent != null)
                StartAimEvent();
        }





        //�ѱ� ���� 


        #region Shoot

        void AimCheck()
        {
            Vector3 inputPos = Input.mousePosition;
            inputPos.z = 10;

            mousePos = Camera.main.ScreenToWorldPoint(inputPos);
            mousePos.z = 0;

            aimDirection = (mousePos - transform.position).normalized;

        }

  

        public void TryShootEvent()  //�̺�Ʈ �߻��� ���ؼ� > PlayerWeapon, PlayerView �� ShootEvent�� �߻�
        {

            //view �ִϸ��̼� ����
            playerView.PreShoot();

            if (ShootEvent != null) ShootEvent();


            //���ֿ� �ִ� ���� ���� Recoil�� �����Ѵ�!
            if (OnAir)
            {
                //aim�� �ݴ� �������� �ݵ��� �ش�
                //Vector2 recoilDir = aimDirection * -1f;
                //rb.AddForce(recoilDir * gunRecoil, ForceMode2D.Impulse);
                StartCoroutine(RecoilRoutine());    

            }
            //else if (onLessGravity && OnAir)
            //{
            //    //aim�� �ݴ� �������� �ݵ��� �ش�
            //    Vector2 recoilDir = aimDirection * -1f;
            //    rb.AddForce(recoilDir * gunRecoil, ForceMode2D.Impulse);
            //}
        }

        IEnumerator RecoilRoutine()
        {
            //���� �� �������� ��� �ش� 
            Vector2 recoilDir = aimDirection * -1f;
            rb.AddForce(recoilDir * gunRecoil, ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.1f);
            //���Ŀ� �ݴ� �������� ��¦ �ش�
            recoilDir = aimDirection;
            rb.AddForce(recoilDir * gunRecoil * 0.3f, ForceMode2D.Impulse); ;
        }

        public void ReloadStart()
        {
            if (StopAimEvent != null) StopAimEvent();
            if (PlayerReloadEvent != null) PlayerReloadEvent();
        }
        
        public void ReloadEnd()
        {
            if(PlayerStopReloadEvent != null) PlayerStopReloadEvent();  
            if (StartAimEvent != null) StartAimEvent();
        }



        public void TryChangeWeapon(WeaponData _data)
        {

            //1-Handed or 2-Handed ��Ų ����
            playerView.ChangeWeapon(_data.SkinIndex, _data.OneHand);

            //recoil ����
            gunRecoil = _data.Recoil;

        }


    }

    #endregion




    public enum PlayerState
    {
        Idle,
        Stun,
        Running,
        Jumping, 
        Die,
        //ChangeState     //state�� �ٲ� �ʿ䰡 ���������� �����Ű�°�? 
    }
}

