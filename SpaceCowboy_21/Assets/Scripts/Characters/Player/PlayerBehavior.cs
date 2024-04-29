using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;


namespace SpaceCowboy
{
    public class PlayerBehavior : MonoBehaviour, IHitable
    {
        [Header("State")]
        public PlayerState playerState = PlayerState.Idle;

        [Header("Jump")]
        public float jumpForce = 10f;
        public float superJumpForce = 18f;
        float lastJumpTime;     
        Vector2 pastPosition = Vector2.zero;
        Vector2 jumpVector; //���� ���� ����

        //�ν���
        [Header("Booster")]
        public float boosterForce = 2f;
        public float boostTime = 1.0f;
        float bTimer;

        [Header("Charge Jump")]
        public float maxChargeTime = 3f;  //������� �ɸ��� �ð� 
        public float chargeStateSpeedMultiplier = 0.3f;     //���� ���¿��� �÷��̾� ������ �ӵ� ����
        //public float minimumChargeTime = 1f;    //�������� �����ϱ� ���� �ð� 
        //public float maxChargePower = 3f;     //��¡���� �þ�� ���� �Ŀ�
        //float chargePressTimer;  //������ �ִ� �ð� 
        //public float chargedPower { get; set; } //������ ��
        public float curChargedTime { get; set; } //������ ������ �־��� �ð�
        bool startCharge;   //í¡�� �����Ѵ�. 
        bool jumpReady;
        bool sJumpReady;    //�������� ����


        [Header("Movement")]
        public float moveSpeed;
        public float acceleration = 0.25f;      //�ӵ��� �ٲ� �� �󸶳� ������ ������.
        public float inputResetTime = 0.5f; //��ǲ�� �ʱ�ȭ�� Ÿ�̸�

        float currSpeed;    //���� �̵� �ӵ�
        float targetSpeed;  //�̵� ��ǥ�� �� ���ǵ�
        float speedMultiplier = 1.0f;
        int currentEdgePointIndex = 0; // ���� ���󰡰� �ִ� ���� �ݶ��̴��� �� �ε���
        float inputResetTimer; //�Է����� ������ input���� �ʱ�ȭ�ϴ� �뵵�� Ÿ�̸�

        Vector2[] planetPoints;     //�޾ƿ� ���� �༺�� points 
        Vector2 preInputAxis = Vector2.zero;  //���� �����ӿ� �����ߴ� ��ǲ ��
        Vector2 moveDir = Vector2.zero;     //�̵����� ����
        Vector2 inputAxis = Vector2.zero;  //������ ��ǲ ��

        [Header("Sliding")]
        public float slideSpeed;
        bool slideON;

        [Header("OnAir")]
        public float turnSpeedOnLand = 100f;
        public float onAirTime = 1f;   //���� ǥ�� �ð� Ÿ�� ����Ʈ.
        public float onAirVelocityRotateSpeed = 1f; //���߿� ���� �ִ� ĳ������ ���� ������ �������� ȸ����ų���� �ӵ� 
       
        [SerializeField] bool OnAir;      //����� ������ �ν����Ϳ��� ���̰�
        float airTimer;

        [Header("KnockBack")]
        public float knockBackForce = 5f;
        public float stunTime = 1f;
        float _sTime; 

        //���� ����
        public Vector3 mousePos { get; set; }
        public float mouseDist { get; set; }
        public Vector2 aimDirection { get; set; }
        public Vector3 gunTipPos { get; set; }
        public bool onSpace { get; private set; }   //���ֿ� �ֽ��ϱ�?
        public bool faceRight { get; set; }         //�������� ���� �ֽ��ϱ�?
        public bool aimRight { get; set; }          //���������� �ܳ��ϰ� �ֽ��ϱ�? 
        bool planetChanged;        // �༺�� �ٲ�����ϱ�? �༺�� �ٲ�� ������ �� faceRight�� �ʱ�ȭ �����Ѵ�.
        public Vector2 playerVelocity { get; private set; }  //�� �������� ���� �÷��̾� �ӵ��� 
        public bool activate { get; private set;}

        [Header("VFX")]
        //����Ʈ��
        public ParticleSystem shieldhitEffect;  //�ǵ尡 �¾��� �� ����� ����Ʈ
        public ParticleSystem slidingEffect;    //�����̵��Ҷ� ����� ����Ʈ
        public TrailRenderer sJumpTrail;    //�������� Ʈ����

        [Header("WeaponDatas")]
        public WeaponData[] weapons;

        //�ִϸ��̼� �̺�Ʈ
        public event System.Action PlayerIdleEvent;
        public event System.Action ShootEvent;
        public event System.Action PlayerHitEvent;
        public event System.Action PlayerDieEvent;
        public event System.Action PlayerJumpStartEvent;
        public event System.Action PlayerJumpEvent;


        [Header("Scripts")]
        //���� ��ũ��Ʈ��
        PlayerWeapon playerWeapon;
        public  PlayerView playerView;
        public JumpTrajectoryViewer jumpViewer;
        CharacterGravity characterGravity;
        Rigidbody2D rb;
        PlayerHealth health;
        //StarMagicGun stargun;


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
            TryChangeWeapon(0);
            
            activate = true;
        }


        void Update()
        {
            if(_sTime > 0 && playerState != PlayerState.Die)
            {
                _sTime -= Time.deltaTime;
                if(_sTime <= 0)
                {
                    activate = true;
                }
            }

            if (!activate) return;

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
                if (curChargedTime < maxChargeTime)
                {
                    curChargedTime += Time.deltaTime;
                    if(curChargedTime > maxChargeTime)
                    {
                        curChargedTime = maxChargeTime;
                        sJumpReady = true;
                    }
                }

                //chargedTimer += Time.deltaTime;
                //chargePressTimer += Time.deltaTime;

                ////���� ���� Ÿ�̸�
                //if (chargePressTimer < minimumChargeTime)
                //{
                //    chargePressTimer = minimumChargeTime;
                //}
                //else if(chargedTimer < maxChargeTime)
                //{
                //    chargedPower = maxChargePower / maxChargeTime * chargedTimer;
                //}
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
                UpdateJumpForce();


            //ĳ���� ���� ������Ʈ 
            aimRight = Vector2.SignedAngle(transform.up, aimDirection) < 0 ? true : false;

        }



        private void FixedUpdate()
        {
            if (!activate) return;

            //��AI���� �о���� ��
            playerVelocity = moveDir * currSpeed;

            //ĳ���� ȸ��
            RotateCharacterToGround();

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

            Vector2 upVec =  ((Vector2)transform.position - characterGravity.nearestPoint).normalized ;
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
            if (playerState != PlayerState.Jumping) return;
            
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
                    

                    playerState = PlayerState.Idle;
                    
                    
                    if (planetChanged)
                    {
                        faceRight = Vector2.SignedAngle(transform.up, inputAxis) < 0 ? true : false;
                        preInputAxis = inputAxis;
                        planetChanged = false;
                    }

                    GetClosestPoint();

                    if (sJumpReady)
                    {
                        sJumpReady = false;
                        sJumpTrail.emitting = false;
                    }

                    if (slideON) TrySlide();
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
            if (!activate) return;
            if (playerState == PlayerState.Jumping) return;
            if (OnAir) { return; }

            //�����̵� ���� ���� �����̵� ���
            if(playerState == PlayerState.Sliding) { StopSlide(); }

            //���� ǥ�� ����, ���� ���� �����ϰ�?
            UpdateJumpForce();  //���� �� �ʱ�ȭ
            jumpViewer.Activate = true;

            startCharge = true;

            //���� �غ� �ִϸ��̼� ���� 
            TryJumpStartEvent();

            playerState = PlayerState.JumpReady;
            jumpReady = true;
            speedMultiplier = chargeStateSpeedMultiplier;
        }

        public void TryJump()
        {
            if (!activate) return;
            if (playerState == PlayerState.Jumping) return;
            if (!jumpReady) return;

            playerState = PlayerState.Jumping;

            lastJumpTime = Time.time;


            rb.velocity = Vector2.zero;
            if (sJumpReady)
            {
                TrySuperJump();
            }
            else
            {
                rb.AddForce(jumpVector * jumpForce, ForceMode2D.Impulse);
            }

            //�ν��� ���
            bTimer = boostTime;

            //���󿡼� �������� ��. OnAir �� false�� ���¿��� ������.
            OnAir = true;
            jumpViewer.Activate = false;
            airTimer = 0;

            //���� ���� ���� ���� �ʱ�ȭ
            startCharge = false;
            //chargedPower = 0f;
            jumpReady = false;
            //sJumpReady = false;
            curChargedTime = 0f;


            //���� �غ� ���� ���� �ʱ�ȭ
            speedMultiplier = 1.0f;

            //���� �ִϸ��̼� ���
            TryJumpEvent();

            //���� ���� ���
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        }

        public void TrySpeedJump()
        {
            if (!activate) return;
            if (playerState == PlayerState.Jumping) return;
            if (OnAir) { return; }

            //���� ǥ�� ����, ���� ���� �����ϰ�?
            UpdateJumpForce();  //���� �� �ʱ�ȭ

            rb.velocity = Vector2.zero;

            //�����̵� ���� ���� �����̵� ���
            if (playerState == PlayerState.Sliding)
            {
                StopSlide();
                sJumpReady = true;
                TrySuperJump();
            }
            else
            {
                rb.AddForce(jumpVector * jumpForce, ForceMode2D.Impulse);
            }

            //���󿡼� �������� ��. OnAir �� false�� ���¿��� ������.
            OnAir = true;
            //jumpViewer.Activate = false;
            airTimer = 0;
            lastJumpTime = Time.time;
            playerState = PlayerState.Jumping;

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

            jumpViewer.trajectoryLength = jumpForce;
            //jumpViewer.trajectoryLength = force + chargedPower;
            jumpViewer.trajectoryVector = jumpVector;

            return force;
        }

        void TrySuperJump()
        {
            rb.AddForce(jumpVector * superJumpForce, ForceMode2D.Impulse);
            sJumpTrail.emitting = true;
        }

        void SelfBoost()
        {
            rb.AddForce(jumpVector * boosterForce, ForceMode2D.Force);
        }

        public void LauchPlayer(Vector2 Vec, float force)
        {
            if (playerState == PlayerState.Sliding) { StopSlide(); }

            playerState = PlayerState.Jumping;

            rb.velocity = Vector2.zero;
            rb.AddForce(Vec * force, ForceMode2D.Impulse);

            //���󿡼� �������� ��. OnAir �� false�� ���¿��� ������.
            OnAir = true;
            airTimer = 0;
            lastJumpTime = Time.time;


            //���� �ִϸ��̼� ���
            TryJumpEvent();

            //���� ���� ���
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        }

        #endregion

        #region Sliding

        public void ToggleSlide()
        {
            if (!slideON) TrySlide();
            else TryStopSlide();
        }
        public void TrySlide()
        {
            slideON = true;

            if (!activate) return;
            if (OnAir) return;

            playerState = PlayerState.Sliding;

            targetSpeed = slideSpeed;
            
            slidingEffect.Play();
            slidingEffect.transform.localScale = new Vector3(faceRight? 1 : -1 , 1, 1);

            MoveOnLand();
        }

        public void TryStopSlide()
        {
            slideON = false;
            StopSlide();
        }

        public void StopSlide()
        {
            if(playerState != PlayerState.Sliding) { return; }

            //state = PlayerState.Idle;
            playerState = PlayerState.Running;
            targetSpeed = 0;
            
            slidingEffect.Stop();
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
            if (!activate) return;
            if (playerState == PlayerState.Sliding) return;
            if (playerState == PlayerState.Jumping) return;

            this.inputAxis = inputAxisRaw;

            targetSpeed = moveSpeed;

            //���¸� �޸���� ����(Idle�� ��쿡��)
            if (playerState == PlayerState.Idle)
            {
                //�޸��� �����Ǹ鼭 �ѹ� ����.
                playerState = PlayerState.Running;

            }

            //���� ���� ������ ������ 
            if (!OnAir)
            {
                MoveOnLand();
            }

        }

        public void TryStop()
        {
            if (playerState == PlayerState.Sliding) return;

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

        }

        //�̵��� ������ �� ����
        private void GetClosestPoint()
        {
            currentEdgePointIndex = characterGravity.nearestPlanet.GetClosestIndex(transform.position);
        }

        public void ChangePlanet()  
        {   //�༺�� �ٲ�� �̺�Ʈ �߻�. CharacterGravity���� ��ȣ�� �޴´�.
            planetChanged = true;
            if(characterGravity.nearestPlanet != null)
                planetPoints = characterGravity.nearestPlanet.GetPoints(0.51f);
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
            if(currSpeed < 0.1f && playerState == PlayerState.Running)
            {
                currSpeed = 0;

                inputAxis = Vector2.zero;
                //preMoveDir = Vector2.zero;
                moveDir = Vector2.zero;

                playerState = PlayerState.Idle;

                preInputAxis = Vector2.zero;

            }
        }


        void MoveUpdate()
        {
            if (currSpeed < acceleration)
                return;

            if (playerState == PlayerState.Running || playerState == PlayerState.JumpReady || playerState == PlayerState.Sliding)
            {
                //������
                int faceInt = faceRight ? 1 : -1;
                Vector2 targetPointPos;
                Vector2 moveVector;
                float moveDist;
                PolygonCollider2D coll = characterGravity.nearestCollider;
                
                //��ǥ�� �� ����Ʈ�� ���Ѵ�.
                do
                {
                    targetPointPos = planetPoints[currentEdgePointIndex];

                    moveVector = targetPointPos - rb.position;
                    moveDist = moveVector.magnitude;

                    if (moveDist < currSpeed * Time.fixedDeltaTime)
                    {
                        currentEdgePointIndex = (currentEdgePointIndex + faceInt + (coll.points.Length - 1)) % ((coll.points.Length - 1));
                    }
                }
                while (moveDist < currSpeed * Time.fixedDeltaTime);

                moveDir = moveVector.normalized;

                // ������Ʈ�� �̵� �������� �̵�
                rb.MovePosition(rb.position + moveDir * currSpeed * speedMultiplier * Time.fixedDeltaTime);

            }
        }

        void MoveUpdateOnAir()
        {
            //�÷��̾ ���߿� ���� ��, �� �ִ� �ð��� ������� �༺ �������� ���� ���������. 
            if (characterGravity.nearestPlanet)
            {
                Vector2 vec = (characterGravity.nearestPoint - (Vector2)transform.position).normalized;
                float vel = rb.velocity.magnitude;

                rb.velocity = Vector2.Lerp(rb.velocity, vec * vel, onAirVelocityRotateSpeed * Time.fixedDeltaTime);
            }
        }


        #endregion

        #region KnockBack
        void PlayerKnockBack(Vector2 hitPoint, float force)
        {
            //���� �ð�
            if (activate) activate = false;
            _sTime = stunTime;

            if (OnAir)
            {
                Vector2 dir = (Vector2)transform.position - hitPoint;
                dir = dir.normalized;

                rb.AddForce(dir * force, ForceMode2D.Impulse);
            }
            else
            {
                //hit����Ʈ�� ��/�� ��� �ִ��� �˻� �� �ݴ� �������� ƨ�ܳ�����. 
                int hitIndex = Vector2.SignedAngle(transform.up, hitPoint - (Vector2)transform.position) > 0 ? 1: -1;

                //Vector2 dir = (((Vector2)transform.position - hitPoint).normalized + (Vector2)transform.up ) * 0.71f;
                Vector2 dir = (transform.right * hitIndex + transform.up) * 0.71f;
                LauchPlayer(dir, force);
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
            playerState = PlayerState.Running;

            //������ �ʱ�ȭ�Ѵ�.
            inputAxis = Vector2.zero;
            preInputAxis = Vector2.zero;

        }


        IEnumerator KnockBackOnAirRoutine(float knockBackTime)
        {
            yield return new WaitForSeconds(knockBackTime);

            if (OnAir)
            {
                playerState = PlayerState.Jumping;

            }
            else
            {
                playerState = PlayerState.Idle;

            }
        }

        #endregion

        #region Damage and Die
        public void DamageEvent(float damage, Vector2 hitPoint)
        {
            if (!activate) return;

            //�������� ����
            if (health.AnyDamage(damage))
            {
                PlayerKnockBack(hitPoint, knockBackForce);
                if (health.currShield > 0)
                {
                    //�ǵ尡 ���� ���
                    shieldhitEffect.Play();
                }
                else
                {
                    //ü���� ���� ���
                    if (PlayerHitEvent != null)
                    {
                        PlayerHitEvent();
                    }
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
            playerState = PlayerState.Die;

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
            StopShoot();
            activate = false;

            health.healthState = PlayerHealth.HealthState.Dead;

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

        public void TryChangeWeapon(int index)
        {
            WeaponData data = weapons[index];
            //PlayerView�� PlayerWeapon ���� ����
            playerWeapon.ChangeWeapon(data);
            playerView.SetSkin(data.GunType);
        }
        public void TryChangeWeapon(WeaponData data)
        {
            //PlayerView�� PlayerWeapon ���� ����
            playerWeapon.ChangeWeapon(data);
            playerView.SetSkin(data.GunType);
        }

        //public void TryMagicShoot()
        //{
        //    stargun.MagicShoot();
        //}
        #endregion

        #region Events

        public void TryIdleEvent()
        {
            if (PlayerIdleEvent != null) PlayerIdleEvent();
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
        Running,
        JumpReady,
        Jumping, 
        Die,
        Sliding
        //ChangeState     //state�� �ٲ� �ʿ䰡 ���������� �����Ű�°�? 
    }
}

