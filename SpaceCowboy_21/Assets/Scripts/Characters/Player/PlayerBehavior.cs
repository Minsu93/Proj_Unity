using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpaceCowboy
{
    public class PlayerBehavior : MonoBehaviour
    {
        [Header("Jump Property")]
        public float jumpForce = 10f;
        public float boosterForce = 2f;
        public float boostTime = 1.0f;
        public float moveForce = 1f;    //PreMoveDir�� ������ ������ ��ȭ��Ų��.
        public float onAirVelocityRotateSpeed = 1f; //���߿� ���� �ִ� ĳ������ ���� ������ �������� ȸ����ų���� �ӵ� 
        Vector2 pastPosition = Vector2.zero;
        float lastJumpTime;     //���� �� ��õ��� onAir ���� ����.
        float bTimer;
        float jumpF;
        Vector2 jumpVector; //���� ���� ����

        [Space]
        [Header("Charge Jump Property")] 
        public float jumpChargeTimeSpent = 3f;  //������� �ɸ��� �ð� 
        public float timeToStartChargeJump = 1f;    //�������� �����ϱ� ���� �ð� 
        public float maxChargePower = 3f;     //��¡���� �þ�� ���� �Ŀ�
        float chargePressTimer;  //������ �ִ� �ð� 
        public float chargedPower { get; set; } //������ ��
        float chargedTimer; //������ �ɸ��� �ð�
        bool startCharge;   //í¡�� �����Ѵ�. 
        bool jumpReady;

        public float chargeStateSpeedMultiplier = 0.3f;     //���� ���¿��� �÷��̾� ������ �ӵ� ����

        public Vector3 mousePos { get; set; }
        public float mouseDist { get; set; }
        public Vector2 aimDirection { get; set; }
        public Vector3 gunTipPos { get; set; }


        [Header("PlayerProperty")]
        public PlayerState state = PlayerState.Idle;
        Vector2 preInputAxis = Vector2.zero;  //���� �����ӿ� �����ߴ� ��ǲ ��
        Vector2 moveDir = Vector2.zero;     //�̵����� ����
        public float moveSpeed;
        public float acceleration = 0.25f;      //�ӵ��� �ٲ� �� �󸶳� ������ ������.
        float currSpeed;    //���� �̵� �ӵ�
        float targetSpeed;  //�̵� ��ǥ�� �� ���ǵ�
        public float inputResetTime = 0.5f; //��ǲ�� �ʱ�ȭ�� Ÿ�̸�
        
        
        [Space]

        public float turnSpeedOnLand = 100f;
        Vector2 inputAxis = Vector2.zero;  //������ ��ǲ ��
        [SerializeField] bool OnAir;      //����� ������ �ν����Ϳ��� ���̰�
        public float onAirTime = 1f;   //���� ǥ�� �ð� Ÿ�� ����Ʈ.
        float airTimer;

        public bool onSpace { get; private set; }   //���ֿ� �ֽ��ϱ�?
        public bool faceRight;
        bool planetChanged;        // �༺�� �ٲ�����ϱ�? �༺�� �ٲ�� ������ �� faceRight�� �ʱ�ȭ �����Ѵ�.

  

        [Header("EdgeFollow")]
        int currentEdgePointIndex = 0; // ���� ���󰡰� �ִ� ���� �ݶ��̴��� �� �ε���

        //�ִϸ��̼� �̺�Ʈ
        public event System.Action PlayerIdleEvent;
        public event System.Action PlayerRunForwardEvent;
        public event System.Action PlayerRunBackwardEvent;

        public event System.Action ShootEvent;
        public event System.Action PlayerHitEvent;
        public event System.Action PlayerDieEvent;
        public event System.Action PlayerJumpStartEvent;
        public event System.Action PlayerJumpEvent;


        PlayerWeapon playerWeapon;
        public  PlayerView playerView;
        public JumpTrajectoryViewer jumpViewer;
        CharacterGravity characterGravity;
        Rigidbody2D rb;
        PlayerHealth health;

        float inputResetTimer; //�Է����� ������ input���� �ʱ�ȭ�ϴ� �뵵�� Ÿ�̸�
        //For Debug
        public Vector2 playerVelocity;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            characterGravity = GetComponent<CharacterGravity>();
            playerWeapon = GetComponent<PlayerWeapon>();
            health = GetComponent<PlayerHealth>();

            GameManager.Instance.player = this.transform;
        }

        private void Start()
        {
            health.ResetHealth();
            TryChangeWeapon(playerWeapon.baseWeapon);

            chargePressTimer = timeToStartChargeJump;
        }


        // Update is called once per frame
        void Update()
        {
            if (state == PlayerState.Die)
                return;

            //�Է� ���� Ÿ�̸�
            //if(inputResetTimer > 0)
            //{
            //    inputResetTimer -= Time.deltaTime;
            //    if(inputResetTimer < 0)
            //    {
            //        InputReset();
            //    }
            //}
            
            //������ ���۵� �� 
            if (startCharge)
            {
                //���� ���� Ÿ�̸�
                if (chargePressTimer < timeToStartChargeJump)
                {
                    chargePressTimer += Time.deltaTime;
                }
                else if(chargedTimer < jumpChargeTimeSpent)
                {
                    chargedTimer += Time.deltaTime;
                    chargedPower = maxChargePower / jumpChargeTimeSpent * chargedTimer;
                }
            }

            //���߿� �ִ��� üũ
            CheckOnAir();

            //���߿� ���� ������ ��ǥ�� �������� ��������. 
            if (OnAir)
            {
                if (airTimer <= onAirTime)
                {
                    airTimer += Time.deltaTime;
                }
            }


            //JumpViewer ������Ʈ
            if (jumpViewer.Activate)
                jumpF = UpdateJumpForce();

        }



        private void FixedUpdate()
        {
            //��AI���� �о���� ��
            playerVelocity = moveDir * currSpeed;

            if (state == PlayerState.Die)
                return;


            //ĳ���� ȸ��
            RotateCharacterToGround();


            if (state == PlayerState.Stun) return;

            if(bTimer > 0)
            {
                bTimer -= Time.deltaTime;
                SelfBoost();
            }

            //���� ������ �Լ�
            MoveUpdate();

            //�̵� �ӵ��� ��Ʈ��
            SpeedControl();

            if(OnAir)
            {
                if(airTimer > onAirTime)
                {
                    MoveUpdateOnAir();
                }
            }
        }

        #region RotateCharacterToGround
        void RotateCharacterToGround()
        {
            if (onSpace)
                return;

            Vector2 upVec = characterGravity.nearestPointGravityVector *  -1f;
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
        #endregion

        #region Jump

        void CheckOnAir()
        {
            if (state != PlayerState.Jumping) return;
            
            //üũ ������ �� �����̸� ����. 
            if (Time.time - lastJumpTime < 0.1f)
                return;


            //ĳ������ �߽ɿ���, �� ������ ���̸� ���� ĳ���Ͱ� ���߿� �� �ִ��� �˻��Ѵ�.
            RaycastHit2D footHit = Physics2D.CircleCast(transform.position, 0.5f, transform.up * -1, 1f, LayerMask.GetMask("Planet"));
            if (footHit.collider != null)
            {
                if (footHit.distance < 0.1f)
                {   //���� �� �ѹ� ����
                    OnAir = false;
                    rb.velocity = Vector2.zero;
                    jumpViewer.Activate = false;
                    

                    state = PlayerState.Idle;
                    
                    
                    if (planetChanged)
                    {
                        faceRight = Vector2.SignedAngle(transform.up, inputAxis) < 0 ? true : false;
                        preInputAxis = inputAxis;
                        planetChanged = false;
                    }

                    GetClosestPoint();
                }
                else
                {
                    if (!OnAir)
                    {
                        OnAir = true;
                        airTimer = 0f;
                    }
                }
            }


            //������ ������ ����ϱ�
            Vector2 jumpDir = rb.position - pastPosition;
            Debug.DrawLine((Vector2)transform.position, (Vector2)transform.position + jumpDir.normalized, Color.white);
            pastPosition = rb.position;
        }

        //void CheckOnSpace()
        //{
        //    //�÷��̾ ���ֿ� �ִ��� �����ؼ� onSpace�� ����, ����� ���� x
        //    bool pre = onSpace;

        //    if( characterGravity.nearestPlanet == null)
        //    {
        //        pre = true;
        //    }
        //    else
        //    {
        //        pre = false;
        //    }

        //    //���ֿ� �ִ°� �޶����� �ִϸ��̼� ����
        //    if (pre != onSpace)
        //        playerView.ChangeState();

        //    onSpace = pre;
        //}

        public void PrepareJump()
        {
            if (state == PlayerState.Die) return;
            if (state == PlayerState.Stun) return;
            if (state == PlayerState.Jumping) return;

            //���� ǥ�� ����, ���� ���� �����ϰ�?
            if (!OnAir)
            {
                UpdateJumpForce();  //���� �� �ʱ�ȭ
                jumpViewer.Activate = true;

                startCharge = true;
                chargePressTimer = 0f;
                chargedTimer = 0f;

                //���� �غ� �ִϸ��̼� ���� 
                TryJumpStartEvent();

                //���� ����
                state = PlayerState.JumpReady;

                //���� ���� 
                jumpReady = true;
            }
        }

        public void TryJump()
        {
            if (state == PlayerState.Die) return;
            if (state == PlayerState.Stun) return;
            if (state == PlayerState.Jumping) return;
            if (!jumpReady) return; 

            state = PlayerState.Jumping;

            lastJumpTime = Time.time;


            //Vector2 upVector = (Vector2)transform.position - characterGravity.nearestPoint;
            //upVector = upVector.normalized;
            ////Vector2 forwardVector = moveDir.normalized;     //moveDir(���� �̵� ����) �������� �������� ������Ű�Ƿ�, moveDir ��ġ�� õõ�� �ٿ��� ���� ���� �����ϴ� �͵� ���� �� �ִ�. 
            //Vector2 forwardVector = faceRight ? transform.right : transform.right * -1;
            //Vector2 jumpDir = upVector + (forwardVector * moveForce * currSpeed);
            //jumpDir = jumpDir.normalized;
            //preJumpVec = jumpDir;
            //Vector2 jumpVector = jumpDir * jumpForce;

            rb.velocity = Vector2.zero;
            rb.AddForce(jumpVector * (jumpF + chargedPower), ForceMode2D.Impulse);

            //�ν��� ���
            bTimer = boostTime;

            //���󿡼� �������� ��. OnAir �� false�� ���¿��� ������.
            OnAir = true;
            jumpViewer.Activate = false;
            airTimer = 0;

            //���� ���� ���� ���� �ʱ�ȭ
            startCharge = false;
            chargedPower = 0f;
            jumpReady = false;

            //���� �ִϸ��̼� ���
            TryJumpEvent();

            //���� ���� ���
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        }

        float UpdateJumpForce()
        {
            float force = jumpForce;
            
            //���� ����. �༺ ǥ�鿡 �ִ� ��� ������ �÷��̾� �� ���� - 60 ~ +60
            float maxAngle = 70f;
            Vector2 upVec = transform.up;
            float angle = Vector2.SignedAngle(upVec, aimDirection);
            
            if (!OnAir)
            {
                if (Mathf.Abs(angle) > maxAngle)
                {
                    //���� ������ ���ϸ� �������� ��������. 
                    force = force * (maxAngle / Mathf.Abs(angle));
                }

                angle = Mathf.Clamp(angle, -maxAngle, maxAngle);
                jumpVector = Quaternion.AngleAxis(angle, Vector3.forward) * transform.up;
            }
            else
            {
                jumpVector = aimDirection;
            }

            jumpViewer.trajectoryLength = force + chargedPower;
            jumpViewer.trajectoryVector = jumpVector;

            return force;
        }

        void SelfBoost()
        {
            rb.AddForce(jumpVector * boosterForce, ForceMode2D.Force);
        }

        #endregion

        #region Dash
        public void TryDash()
        {
            //DashForce();
            //StartCoroutine(DashRoutine());
            if (OnAir)
                StartCoroutine(EmergencyLanding());
            //else
            //{
            //    StartCoroutine(EmergencyHop());
            //}

        }
        void DashForce()
        {
            rb.velocity = Vector2.zero;
            Vector2 dir = aimDirection;
            float dashSpeed = 15f;
            rb.AddForce(dir * dashSpeed, ForceMode2D.Impulse);

        }
        IEnumerator DashRoutine()
        {
            //��� ����
            characterGravity.activate = false;
            rb.velocity = Vector2.zero;
            float dashSpeed = 30f;
            float dashDuration = .2f;
            Vector2 dir = aimDirection;

            rb.AddForce(dir * dashSpeed, ForceMode2D.Impulse);

            //��� �ð�
            yield return new WaitForSeconds(dashDuration);

            // �ӵ� �ʱ�ȭ
            rb.velocity = rb.velocity * 0.2f;
            //rb.AddForce(dir * -1f * dashSpeed *0.3f, ForceMode2D.Impulse);
            characterGravity.activate = true;
        }
        IEnumerator EmergencyHop()
        {
            state = PlayerState.Jumping;

            lastJumpTime = Time.time;
            OnAir = true;
            airTimer = 0f;

            Vector2 dir = transform.up;
            float dist = 3f;
            Vector2 startPos = (Vector2)transform.position;

            float timer = 0f;
            float hopingDuration = 0.2f;

            while (timer < hopingDuration)
            {
                timer += Time.deltaTime;
                rb.MovePosition(startPos + dir * dist * timer / hopingDuration);

                yield return null;
            }
        }
        IEnumerator EmergencyLanding()
        {
            float timer = 0f;
            float landingDuration = 0.2f;
            Vector2 dir = characterGravity.nearestPoint - (Vector2)transform.position;
            Vector2 startPos = (Vector2)transform.position;
            float dist = dir.magnitude;
            dir = dir.normalized;

            while (timer < landingDuration)
            {
                timer += Time.deltaTime;
                rb.MovePosition(startPos + dir * dist * timer / landingDuration);

                yield return null;
            }
            
        }
        #endregion

        #region Movement


        //void InputReset()
        //{
        //    //�ð��� ������ 
        //    //preInputAxis = Vector2.zero;
        //}

        public void TryMove(Vector2 inputAxisRaw)
        {
            if (state == PlayerState.Die) return;
            //�÷��̾ �˹����϶��� �̵� ������ �Ұ���. 
            if (state == PlayerState.Stun) return;

            this.inputAxis = inputAxisRaw;

            //�ѹ����� ����ϴ°� �����ս��� ������, �ƴϸ� ���� ������ ����ִ°� �����ս��� ������ �𸣰���. ������ �˻�� �ؾ����ݾ�.. 

            targetSpeed = moveSpeed;


            //���� ���� ������ ������ 
            if (!OnAir)
            {
                MoveOnLand();
            }

        }

        public void TryStop()
        {
            targetSpeed = 0;
            //inputResetTimer = inputResetTime;
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

                //��ǲ ������ ���.
                //inputResetTimer = -1f;
            }

            if (state != PlayerState.Running && state != PlayerState.JumpReady)
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





        //�̵��� ������ �� ����
        private void GetClosestPoint()
        {

            int closestPointIndex = 0;
            float closestDistance = float.MaxValue;

            // ���� �ݶ��̴��� ��� ����Ʈ���� ��ȸ�ϸ� ���� ����� ����Ʈ�� �ε����� �Ÿ��� ����
            for (int i = 0; i < characterGravity.nearestCollider.points.Length; i++)
            {
                Vector2 pointPosition = characterGravity.nearestCollider.transform.TransformPoint(characterGravity.nearestCollider.points[i]);
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
            planetChanged = true;
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
                currSpeed = 0;

                inputAxis = Vector2.zero;
                //preMoveDir = Vector2.zero;
                moveDir = Vector2.zero;

                state = PlayerState.Idle;

                preInputAxis = Vector2.zero;

            }
        }


        void MoveUpdate()
        {
            if (currSpeed < acceleration)
                return;

            if (state == PlayerState.Running || state == PlayerState.Stun || state == PlayerState.JumpReady)
            {
                //������
                int faceInt;
                Vector2 targetPointPos;
                Vector2 pastPointPos;
                int pastPoint;
                Vector2 direction;
                Vector2 moveVector;
                float moveDist;
                PolygonCollider2D coll = characterGravity.nearestCollider;

                //��ǥ�� �� ����Ʈ�� ���Ѵ�.
                do
                {
                    faceInt = faceRight ? 1 : -1;
                    targetPointPos = coll.transform.TransformPoint(coll.points[currentEdgePointIndex % (coll.points.Length - 1)]);   //�ݶ��̴��� ������  point �� �� 0�� ����. �׷��� ����.

                    pastPoint = (currentEdgePointIndex - faceInt + (coll.points.Length - 1)) % (coll.points.Length - 1);
                    pastPointPos = coll.transform.TransformPoint(coll.points[pastPoint]);
                    direction = (targetPointPos - pastPointPos) * faceInt;
                    Vector2 normal = new Vector2(-direction.y, direction.x).normalized;

                    moveVector = targetPointPos + (normal * 0.51f);         //�÷��̾��� ���̴�. Ÿ�� ����Ʈ���� �븻���� * �÷��̾� ���̸� ��ǥ�� ������.
                    moveVector -= rb.position;
                    moveDist = moveVector.magnitude;

                    Debug.DrawRay(rb.position, moveVector, Color.cyan, 0.5f);


                    if (moveDist < currSpeed * Time.fixedDeltaTime)
                    {
                        currentEdgePointIndex = (currentEdgePointIndex + faceInt + (coll.points.Length - 1)) % ((coll.points.Length - 1));
                    }
                }
                while (moveDist < currSpeed * Time.fixedDeltaTime);

                moveDir = moveVector.normalized;

                //�÷��̾ JumpReady�� ������ ���� 0.3���� �ӵ��� �����δ�. ������ ���¿����� 1�� ���·� �����δ�. 
                float speedMultiplier = state == PlayerState.JumpReady ? chargeStateSpeedMultiplier : 1.0f;

                float speed = currSpeed;

                // ������Ʈ�� �̵� �������� �̵�
                rb.MovePosition(rb.position + moveDir * speed * speedMultiplier * Time.fixedDeltaTime);

            }
        }

        void MoveUpdateOnAir()
        {
            //�÷��̾ ���߿� ���� ��, �� �ִ� �ð��� ������� �༺ �������� ���� ���������. 
            if (characterGravity.nearestPlanet)
            {
                Vector2 vec = characterGravity.nearestPointGravityVector;
                float vel = rb.velocity.magnitude;

                rb.velocity = Vector2.Lerp(rb.velocity, vec * vel, onAirVelocityRotateSpeed * Time.fixedDeltaTime);
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
            StopShoot();

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
            jumpViewer.Activate = false;
            //StopAllCoroutines();


            //�浹 ������ �����
            //coll.enabled = false;

            //���� �̺�Ʈ �ߵ�. ���鿡�� ĳ���Ͱ� �׾��ٰ� ����. 
            GameManager.Instance.PlayerIsDead();
        }

        #endregion

        #region Shoot

        public void StartShoot()
        {
            if(!playerWeapon.shootON)
            {
                playerWeapon.shootON = true;
            }
        }

        public void StopShoot()
        {
            if(playerWeapon.shootON)
            {
                playerWeapon.shootON = false;
            }
        }

        public void TryChangeWeapon(WeaponData data)
        {
            //PlayerView�� PlayerWeapon ���� ����
            playerWeapon.ChangeWeapon(data);
            playerView.SetSkin(data.GunType);
        }
        #endregion

        #region Events

        public void TryIdleEvent()
        {
            if (PlayerIdleEvent != null) PlayerIdleEvent();
        }

        public void TryRunForwardEvent()
        {
            if(PlayerRunForwardEvent != null) PlayerRunForwardEvent();
        }

        public void TryRunBackwardEvent()
        {
            if(PlayerRunBackwardEvent != null) PlayerRunBackwardEvent();
        }

        public void TryJumpEvent()
        {
            if(PlayerJumpEvent != null) PlayerJumpEvent();
        }

        public void TryShootEvent()  //�̺�Ʈ �߻��� ���ؼ� > PlayerWeapon, PlayerView �� ShootEvent�� �߻�
        {
            //view �ִϸ��̼� ����
            playerView.PreShoot();

            if (ShootEvent != null) ShootEvent();
        }

        public void TryJumpStartEvent()
        {
            if (PlayerJumpStartEvent != null) PlayerJumpStartEvent();

        }

        #endregion
    }




    public enum PlayerState
    {
        Idle,
        Stun,
        Running,
        JumpReady,
        Jumping, 
        Die,
        //ChangeState     //state�� �ٲ� �ʿ䰡 ���������� �����Ű�°�? 
    }
}

