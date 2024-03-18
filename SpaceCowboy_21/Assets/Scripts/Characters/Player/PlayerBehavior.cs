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
        public float moveForce = 1f;    //PreMoveDir이 점프의 방향을 변화시킨다.
        public float onAirVelocityRotateSpeed = 1f; //공중에 오래 있는 캐릭터의 진행 방향을 지면으로 회전시킬때의 속도 
        Vector2 pastPosition = Vector2.zero;
        float lastJumpTime;     //점프 후 잠시동안 onAir 감지 정지.
        float bTimer;
        float jumpF;
        Vector2 jumpVector; //이전 점프 벡터

        [Space]
        [Header("Charge Jump Property")] 
        public float jumpChargeTimeSpent = 3f;  //완충까지 걸리는 시간 
        public float timeToStartChargeJump = 1f;    //차지점프 시작하기 까지 시간 
        public float maxChargePower = 3f;     //차징으로 늘어나는 점프 파워
        float chargePressTimer;  //누르고 있는 시간 
        public float chargedPower { get; set; } //충전된 힘
        float chargedTimer; //충전에 걸리는 시간
        bool startCharge;   //챠징을 시작한다. 
        bool jumpReady;

        public float chargeStateSpeedMultiplier = 0.3f;     //충전 상태에서 플레이어 움직임 속도 조절

        public Vector3 mousePos { get; set; }
        public float mouseDist { get; set; }
        public Vector2 aimDirection { get; set; }
        public Vector3 gunTipPos { get; set; }


        [Header("PlayerProperty")]
        public PlayerState state = PlayerState.Idle;
        Vector2 preInputAxis = Vector2.zero;  //이전 프레임에 조작했던 인풋 값
        Vector2 moveDir = Vector2.zero;     //이동중인 방향
        public float moveSpeed;
        public float acceleration = 0.25f;      //속도가 바뀔 때 얼마나 빠르게 변할지.
        float currSpeed;    //현재 이동 속도
        float targetSpeed;  //이동 목표가 될 스피드
        public float inputResetTime = 0.5f; //인풋이 초기화될 타이머
        
        
        [Space]

        public float turnSpeedOnLand = 100f;
        Vector2 inputAxis = Vector2.zero;  //움직임 인풋 값
        [SerializeField] bool OnAir;      //디버그 용으로 인스펙터에서 보이게
        public float onAirTime = 1f;   //공중 표류 시간 타임 리미트.
        float airTimer;

        public bool onSpace { get; private set; }   //우주에 있습니까?
        public bool faceRight;
        bool planetChanged;        // 행성이 바뀌었습니까? 행성이 바뀌면 착지할 때 faceRight를 초기화 예약한다.

  

        [Header("EdgeFollow")]
        int currentEdgePointIndex = 0; // 현재 따라가고 있는 엣지 콜라이더의 점 인덱스

        //애니메이션 이벤트
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

        float inputResetTimer; //입력하지 않으면 input값을 초기화하는 용도의 타이머
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
                //차지 점프 타이머
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
                jumpF = UpdateJumpForce();

        }



        private void FixedUpdate()
        {
            //적AI에서 읽어오는 값
            playerVelocity = moveDir * currSpeed;

            if (state == PlayerState.Die)
                return;


            //캐릭터 회전
            RotateCharacterToGround();


            if (state == PlayerState.Stun) return;

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

            Vector2 upVec = characterGravity.nearestPointGravityVector *  -1f;
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
            if (state != PlayerState.Jumping) return;
            
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
            if (state == PlayerState.Die) return;
            if (state == PlayerState.Stun) return;
            if (state == PlayerState.Jumping) return;

            //점프 표시 생성, 공중 점프 가능하게?
            if (!OnAir)
            {
                UpdateJumpForce();  //이전 값 초기화
                jumpViewer.Activate = true;

                startCharge = true;
                chargePressTimer = 0f;
                chargedTimer = 0f;

                //점프 준비 애니메이션 실행 
                TryJumpStartEvent();

                //상태 변경
                state = PlayerState.JumpReady;

                //점프 변수 
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
            ////Vector2 forwardVector = moveDir.normalized;     //moveDir(현재 이동 방향) 기준으로 앞쪽으로 점프시키므로, moveDir 수치를 천천히 줄여서 점프 방향 수정하는 것도 만들 수 있다. 
            //Vector2 forwardVector = faceRight ? transform.right : transform.right * -1;
            //Vector2 jumpDir = upVector + (forwardVector * moveForce * currSpeed);
            //jumpDir = jumpDir.normalized;
            //preJumpVec = jumpDir;
            //Vector2 jumpVector = jumpDir * jumpForce;

            rb.velocity = Vector2.zero;
            rb.AddForce(jumpVector * (jumpF + chargedPower), ForceMode2D.Impulse);

            //부스터 기능
            bTimer = boostTime;

            //지상에서 점프했을 때. OnAir 가 false인 상태에서 점프함.
            OnAir = true;
            jumpViewer.Activate = false;
            airTimer = 0;

            //충전 점프 관련 변수 초기화
            startCharge = false;
            chargedPower = 0f;
            jumpReady = false;

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
            //대시 시작
            characterGravity.activate = false;
            rb.velocity = Vector2.zero;
            float dashSpeed = 30f;
            float dashDuration = .2f;
            Vector2 dir = aimDirection;

            rb.AddForce(dir * dashSpeed, ForceMode2D.Impulse);

            //대시 시간
            yield return new WaitForSeconds(dashDuration);

            // 속도 초기화
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
        //    //시간이 지나면 
        //    //preInputAxis = Vector2.zero;
        //}

        public void TryMove(Vector2 inputAxisRaw)
        {
            if (state == PlayerState.Die) return;
            //플레이어가 넉백중일때는 이동 조작이 불가능. 
            if (state == PlayerState.Stun) return;

            this.inputAxis = inputAxisRaw;

            //한번씩만 계산하는게 퍼포먼스가 좋을지, 아니면 매턴 변수에 집어넣는게 퍼포먼스가 좋을지 모르겠음. 어차피 검사는 해야하잖아.. 

            targetSpeed = moveSpeed;


            //지상에 있을 동안의 움직임 
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

            if (state != PlayerState.Running && state != PlayerState.JumpReady)
            {
                //달리기 수정되면서 한번 실행.
                state = PlayerState.Running;

            }

        }
        /*
        void MoveOnAir()
        {
            //근처에 행성이 없다면 공중 이동은 불가능.
            if (onSpace)
                return;

            //if (!onFalling) return;


            //조작 방향 바뀜
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





        //이동을 시작할 때 실행
        private void GetClosestPoint()
        {

            int closestPointIndex = 0;
            float closestDistance = float.MaxValue;

            // 엣지 콜라이더의 모든 포인트들을 순회하며 가장 가까운 포인트의 인덱스와 거리를 구함
            for (int i = 0; i < characterGravity.nearestCollider.points.Length; i++)
            {
                Vector2 pointPosition = characterGravity.nearestCollider.transform.TransformPoint(characterGravity.nearestCollider.points[i]);
                bool right = Vector2.SignedAngle(transform.up, pointPosition - rb.position) < 0 ? true : false;
                if (faceRight != right) continue; //캐릭터 진행방향과 다르면 제외한다
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
        {   //행성이 바뀌면 right 방향을 재계산. CharacterGravity에서 신호를 받는다.
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

            //움직이는 중에 속도가 0으로 가까워질 때
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
                //움직임
                int faceInt;
                Vector2 targetPointPos;
                Vector2 pastPointPos;
                int pastPoint;
                Vector2 direction;
                Vector2 moveVector;
                float moveDist;
                PolygonCollider2D coll = characterGravity.nearestCollider;

                //목표로 할 포인트를 구한다.
                do
                {
                    faceInt = faceRight ? 1 : -1;
                    targetPointPos = coll.transform.TransformPoint(coll.points[currentEdgePointIndex % (coll.points.Length - 1)]);   //콜라이더의 마지막  point 는 곧 0과 같다. 그래서 제외.

                    pastPoint = (currentEdgePointIndex - faceInt + (coll.points.Length - 1)) % (coll.points.Length - 1);
                    pastPointPos = coll.transform.TransformPoint(coll.points[pastPoint]);
                    direction = (targetPointPos - pastPointPos) * faceInt;
                    Vector2 normal = new Vector2(-direction.y, direction.x).normalized;

                    moveVector = targetPointPos + (normal * 0.51f);         //플레이어의 높이다. 타겟 포인트에서 노말방향 * 플레이어 높이를 목표로 움직임.
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

                //플레이어가 JumpReady인 상태일 때는 0.3배의 속도로 움직인다. 나머지 상태에서는 1의 상태로 움직인다. 
                float speedMultiplier = state == PlayerState.JumpReady ? chargeStateSpeedMultiplier : 1.0f;

                float speed = currSpeed;

                // 오브젝트를 이동 방향으로 이동
                rb.MovePosition(rb.position + moveDir * speed * speedMultiplier * Time.fixedDeltaTime);

            }
        }

        void MoveUpdateOnAir()
        {
            //플레이어가 공중에 있을 때, 떠 있는 시간이 길어지면 행성 방향으로 점점 가까워진다. 
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
            //연발 총을 쏘는 도중이라면 루틴을 중단한다. 
            StopShoot();

            //플레이어를 뒤로 넉백시킨다. 지상/공중일 때 따로..
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
                //움직임, 슈팅 등의 루틴들을 정지한다. 
                //if (characterGravity == null) return;

                StartCoroutine(KnockBackRoutine(hitPoint, knockBackTime, knockBackSpeed));
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
            state = PlayerState.Running;

            //조작을 초기화한다.
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

           

            //데미지를 적용
            if (health.AnyDamage(dmg))
            {
                //맞았으면 내부 실행
                if (PlayerHitEvent != null)
                {
                    PlayerHitEvent();
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
            state = PlayerState.Die;

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

        public void TryChangeWeapon(WeaponData data)
        {
            //PlayerView와 PlayerWeapon 무기 변경
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
        Stun,
        Running,
        JumpReady,
        Jumping, 
        Die,
        //ChangeState     //state가 바뀔 필요가 있을때마다 실행시키는것? 
    }
}

