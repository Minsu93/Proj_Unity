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
        Vector2 jumpVector; //이전 점프 벡터

        //부스터
        [Header("Booster")]
        public float boosterForce = 2f;
        public float boostTime = 1.0f;
        float bTimer;

        [Header("Charge Jump")]
        public float maxChargeTime = 3f;  //완충까지 걸리는 시간 
        public float chargeStateSpeedMultiplier = 0.3f;     //충전 상태에서 플레이어 움직임 속도 조절
        //public float minimumChargeTime = 1f;    //차지점프 시작하기 까지 시간 
        //public float maxChargePower = 3f;     //차징으로 늘어나는 점프 파워
        //float chargePressTimer;  //누르고 있는 시간 
        //public float chargedPower { get; set; } //충전된 힘
        public float curChargedTime { get; set; } //충전을 누르고 있었던 시간
        bool startCharge;   //챠징을 시작한다. 
        bool jumpReady;
        bool sJumpReady;    //슈퍼점프 레디


        [Header("Movement")]
        public float moveSpeed;
        public float acceleration = 0.25f;      //속도가 바뀔 때 얼마나 빠르게 변할지.
        public float inputResetTime = 0.5f; //인풋이 초기화될 타이머

        float currSpeed;    //현재 이동 속도
        float targetSpeed;  //이동 목표가 될 스피드
        float speedMultiplier = 1.0f;
        int currentEdgePointIndex = 0; // 현재 따라가고 있는 엣지 콜라이더의 점 인덱스
        float inputResetTimer; //입력하지 않으면 input값을 초기화하는 용도의 타이머

        Vector2[] planetPoints;     //받아온 현재 행성의 points 
        Vector2 preInputAxis = Vector2.zero;  //이전 프레임에 조작했던 인풋 값
        Vector2 moveDir = Vector2.zero;     //이동중인 방향
        Vector2 inputAxis = Vector2.zero;  //움직임 인풋 값

        [Header("Sliding")]
        public float slideSpeed;
        bool slideON;

        [Header("OnAir")]
        public float turnSpeedOnLand = 100f;
        public float onAirTime = 1f;   //공중 표류 시간 타임 리미트.
        public float onAirVelocityRotateSpeed = 1f; //공중에 오래 있는 캐릭터의 진행 방향을 지면으로 회전시킬때의 속도 
       
        [SerializeField] bool OnAir;      //디버그 용으로 인스펙터에서 보이게
        float airTimer;

        [Header("KnockBack")]
        public float knockBackForce = 5f;
        public float stunTime = 1f;
        float _sTime; 

        //참조 변수
        public Vector3 mousePos { get; set; }
        public float mouseDist { get; set; }
        public Vector2 aimDirection { get; set; }
        public Vector3 gunTipPos { get; set; }
        public bool onSpace { get; private set; }   //우주에 있습니까?
        public bool faceRight { get; set; }         //오른쪽을 보고 있습니까?
        public bool aimRight { get; set; }          //오른쪽으로 겨냥하고 있습니까? 
        bool planetChanged;        // 행성이 바뀌었습니까? 행성이 바뀌면 착지할 때 faceRight를 초기화 예약한다.
        public Vector2 playerVelocity { get; private set; }  //적 예측샷을 위한 플레이어 속도값 
        public bool activate { get; private set;}

        [Header("VFX")]
        //이펙트들
        public ParticleSystem shieldhitEffect;  //실드가 맞았을 때 출력할 이펙트
        public ParticleSystem slidingEffect;    //슬라이딩할때 출력할 이펙트
        public TrailRenderer sJumpTrail;    //슈퍼점프 트레일

        [Header("WeaponDatas")]
        public WeaponData[] weapons;

        //애니메이션 이벤트
        public event System.Action PlayerIdleEvent;
        public event System.Action ShootEvent;
        public event System.Action PlayerHitEvent;
        public event System.Action PlayerDieEvent;
        public event System.Action PlayerJumpStartEvent;
        public event System.Action PlayerJumpEvent;


        [Header("Scripts")]
        //본인 스크립트들
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

            //입력 리셋 타이머
            //if(inputResetTimer > 0)
            //{
            //    inputResetTimer -= Time.deltaTime;
            //    if(inputResetTimer < 0)
            //    {
            //        InputReset();
            //    }
            //}

            //차지가 시작될 때 
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

                ////차지 점프 타이머
                //if (chargePressTimer < minimumChargeTime)
                //{
                //    chargePressTimer = minimumChargeTime;
                //}
                //else if(chargedTimer < maxChargeTime)
                //{
                //    chargedPower = maxChargePower / maxChargeTime * chargedTimer;
                //}
            }

            //공중에 있는지 체크
            CheckOnAir();

            //공중에 오래 있으면 지표면 방향으로 떨어진다. 
            if (OnAir)
            {
                if (airTimer <= onAirTime)
                {
                    airTimer += Time.deltaTime;
                }
            }

            //JumpViewer 업데이트
            if (jumpViewer.Activate)
                UpdateJumpForce();


            //캐릭터 방향 업데이트 
            aimRight = Vector2.SignedAngle(transform.up, aimDirection) < 0 ? true : false;

        }



        private void FixedUpdate()
        {
            if (!activate) return;

            //적AI에서 읽어오는 값
            playerVelocity = moveDir * currSpeed;

            //캐릭터 회전
            RotateCharacterToGround();

            if(bTimer > 0)
            {
                bTimer -= Time.deltaTime;
                SelfBoost();
            }

            //실제 움직임 함수
            MoveUpdate();

            //이동 속도를 컨트롤
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
            //normal과 transform.upward 사이의 값 차이가 크면 보정을 가해준다. 
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
            
            //체크 시작할 때 딜레이를 주자. 
            if (Time.time - lastJumpTime < 0.1f)
                return;


            //캐릭터의 중심에서, 발 밑으로 레이를 쏴서 캐릭터가 공중에 떠 있는지 검사한다.
            RaycastHit2D footHit = Physics2D.CircleCast(transform.position, 0.5f, transform.up * -1, 1f, LayerMask.GetMask("Planet"));
            if (footHit.collider != null)
            {
                if (footHit.distance < 0.1f)
                {   //착지 후 한번 실행
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


            //떨어질 때에만 계산하기
            Vector2 jumpDir = rb.position - pastPosition;
            Debug.DrawLine((Vector2)transform.position, (Vector2)transform.position + jumpDir.normalized, Color.white);
            pastPosition = rb.position;
        }

        //void CheckOnSpace()
        //{
        //    //플레이어가 우주에 있는지 감지해서 onSpace에 적용, 현재는 적용 x
        //    bool pre = onSpace;

        //    if( characterGravity.nearestPlanet == null)
        //    {
        //        pre = true;
        //    }
        //    else
        //    {
        //        pre = false;
        //    }

        //    //우주에 있는게 달라지면 애니메이션 변경
        //    if (pre != onSpace)
        //        playerView.ChangeState();

        //    onSpace = pre;
        //}

        public void PrepareJump()
        {
            if (!activate) return;
            if (playerState == PlayerState.Jumping) return;
            if (OnAir) { return; }

            //슬라이딩 중일 때는 슬라이딩 취소
            if(playerState == PlayerState.Sliding) { StopSlide(); }

            //점프 표시 생성, 공중 점프 가능하게?
            UpdateJumpForce();  //이전 값 초기화
            jumpViewer.Activate = true;

            startCharge = true;

            //점프 준비 애니메이션 실행 
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

            //부스터 기능
            bTimer = boostTime;

            //지상에서 점프했을 때. OnAir 가 false인 상태에서 점프함.
            OnAir = true;
            jumpViewer.Activate = false;
            airTimer = 0;

            //충전 점프 관련 변수 초기화
            startCharge = false;
            //chargedPower = 0f;
            jumpReady = false;
            //sJumpReady = false;
            curChargedTime = 0f;


            //점프 준비 관련 변수 초기화
            speedMultiplier = 1.0f;

            //점프 애니메이션 출력
            TryJumpEvent();

            //점프 사운드 출력
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        }

        public void TrySpeedJump()
        {
            if (!activate) return;
            if (playerState == PlayerState.Jumping) return;
            if (OnAir) { return; }

            //점프 표시 생성, 공중 점프 가능하게?
            UpdateJumpForce();  //이전 값 초기화

            rb.velocity = Vector2.zero;

            //슬라이딩 중일 때는 슬라이딩 취소
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

            //지상에서 점프했을 때. OnAir 가 false인 상태에서 점프함.
            OnAir = true;
            //jumpViewer.Activate = false;
            airTimer = 0;
            lastJumpTime = Time.time;
            playerState = PlayerState.Jumping;

            //점프 애니메이션 출력
            TryJumpEvent();

            //점프 사운드 출력
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        }

        float UpdateJumpForce()
        {
            float force = jumpForce;
            
            //각도 조절. 행성 표면에 있는 경우 각도는 플레이어 위 기준 - 60 ~ +60
            float maxAngle = 70f;
            Vector2 upVec = transform.up;
            float angle = Vector2.SignedAngle(upVec, aimDirection);
            
            if (!OnAir)
            {
                if (Mathf.Abs(angle) > maxAngle)
                {
                    //점프 각도가 과하면 점프력이 낮아진다. 
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

            //지상에서 점프했을 때. OnAir 가 false인 상태에서 점프함.
            OnAir = true;
            airTimer = 0;
            lastJumpTime = Time.time;


            //점프 애니메이션 출력
            TryJumpEvent();

            //점프 사운드 출력
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
        //    //시간이 지나면 
        //    //preInputAxis = Vector2.zero;
        //}

        public void TryMove(Vector2 inputAxisRaw)
        {
            if (!activate) return;
            if (playerState == PlayerState.Sliding) return;
            if (playerState == PlayerState.Jumping) return;

            this.inputAxis = inputAxisRaw;

            targetSpeed = moveSpeed;

            //상태를 달리기로 변경(Idle인 경우에만)
            if (playerState == PlayerState.Idle)
            {
                //달리기 수정되면서 한번 실행.
                playerState = PlayerState.Running;

            }

            //지상에 있을 동안의 움직임 
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
            //조작 방향 바뀜
            if (inputAxis.x != preInputAxis.x || inputAxis.y != preInputAxis.y)
            {
                preInputAxis = inputAxis;

                faceRight = Vector2.SignedAngle(transform.up, inputAxis) < 0 ? true : false;
                // 방향이 바뀔 때, 그 방향의 가장 가까운 포인트를 가져온다.
                GetClosestPoint();

                //인풋 리셋은 취소.
                //inputResetTimer = -1f;
            }

        }

        //이동을 시작할 때 실행
        private void GetClosestPoint()
        {
            currentEdgePointIndex = characterGravity.nearestPlanet.GetClosestIndex(transform.position);
        }

        public void ChangePlanet()  
        {   //행성이 바뀌면 이벤트 발생. CharacterGravity에서 신호를 받는다.
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

            //움직이는 중에 속도가 0으로 가까워질 때
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
                //움직임
                int faceInt = faceRight ? 1 : -1;
                Vector2 targetPointPos;
                Vector2 moveVector;
                float moveDist;
                PolygonCollider2D coll = characterGravity.nearestCollider;
                
                //목표로 할 포인트를 구한다.
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

                // 오브젝트를 이동 방향으로 이동
                rb.MovePosition(rb.position + moveDir * currSpeed * speedMultiplier * Time.fixedDeltaTime);

            }
        }

        void MoveUpdateOnAir()
        {
            //플레이어가 공중에 있을 때, 떠 있는 시간이 길어지면 행성 방향으로 점점 가까워진다. 
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
            //스턴 시간
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
                //hit포인트가 좌/우 어디에 있는지 검사 후 반대 방향으로 튕겨나간다. 
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

            //맞은 부분이 캐릭터 앞쪽인지 뒷쪽인지 구한다
            Vector2 hitVec = hitPoint - (Vector2)transform.position;

            faceRight = Vector2.SignedAngle(transform.up, hitVec) < 0 ? false : true;

            //속도 지정(줄어들지 않음. Stun 이 되면서 SpeedControl 을 껏으니)
            currSpeed = speed;

            //캐릭터의 위치를 구한다. 
            GetClosestPoint();


            //플레이어 Flip을 막는다. 
            playerView.flipOn = false;


            //일정 시간동안 이동한다. 
            float timer = 0f;
            while (timer < time)
            {
                timer += Time.deltaTime;

                yield return null;
            }
            //플레이어 Flip을 회복한다. 
            playerView.flipOn = true;

            //playerState 를 회복한다. 움직이는 상태에서 자연스럽게 속도가 줄어들기 위해서 running으로. 
            playerState = PlayerState.Running;

            //조작을 초기화한다.
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

            //데미지를 적용
            if (health.AnyDamage(damage))
            {
                PlayerKnockBack(hitPoint, knockBackForce);
                if (health.currShield > 0)
                {
                    //실드가 닳은 경우
                    shieldhitEffect.Play();
                }
                else
                {
                    //체력이 닳은 경우
                    if (PlayerHitEvent != null)
                    {
                        PlayerHitEvent();
                    }
                }


                //CinemachineShake.instance.ShakeCamera(2f, 0.1f);
            }

            //죽었나요?
            if (health.IsDead())
            {
                DeadEvent();

                return;
            }

        }

        public void DeadEvent()
        {
            //true 인 경우 체력이 0 이하로 떨어졌다는 뜻.
            playerState = PlayerState.Die;

            //죽는 모션 실행
            if (PlayerDieEvent != null)
            {
                PlayerDieEvent();
            }

            currSpeed = 0;
            targetSpeed = 0;
            jumpViewer.Activate = false;
            //StopAllCoroutines();


            //충돌 판정을 멈춘다
            //coll.enabled = false;
            StopShoot();
            activate = false;

            health.healthState = PlayerHealth.HealthState.Dead;

            //전역 이벤트 발동. 적들에게 캐릭터가 죽었다고 전달. 
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
            //PlayerView와 PlayerWeapon 무기 변경
            playerWeapon.ChangeWeapon(data);
            playerView.SetSkin(data.GunType);
        }
        public void TryChangeWeapon(WeaponData data)
        {
            //PlayerView와 PlayerWeapon 무기 변경
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

        public void TryShootEvent()  //이벤트 발생을 위해서 > PlayerWeapon, PlayerView 의 ShootEvent를 발생
        {
            //view 애니메이션 실행
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
        //ChangeState     //state가 바뀔 필요가 있을때마다 실행시키는것? 
    }
}

