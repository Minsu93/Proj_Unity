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
        public float moveForce = 1f;    //PreMoveDir이 점프의 방향을 변화시킨다.
        Vector2 pastPosition = Vector2.zero;
        float lastJumpTime;     //점프 후 잠시동안 onAir 감지 정지.
        bool onFalling; //떨어지고 있는 중.
        bool airJump;   //공중 점프가 가능한지 여부 

        [Header("Aim Property")]
        public Vector3 mousePos;
        public Vector3 aimDirection;
        //public Transform gunTip;

        [Header("PlayerProperty")]
        public PlayerState state = PlayerState.Idle;
        Vector2 preInputAxis = Vector2.zero;  //이전 프레임에 조작했던 인풋 값
        Vector2 moveDir = Vector2.zero;     //이동중인 방향
        public float moveSpeed;
        public float acceleration = 0.25f;      //속도가 바뀔 때 얼마나 빠르게 변할지.
        float currSpeed;    //현재 이동 속도
        float targetSpeed;  //이동 목표가 될 스피드
        public float runSpeedMultiply = 2f;
        
        [Space]

        public float airMoveSpeed;
        public float turnSpeedOnLand = 100f;
        Vector2 inputAxis = Vector2.zero;  //움직임 인풋 값
        [SerializeField] bool OnAir;      //디버그 용으로 인스펙터에서 보이게
        public bool onSpace { get; private set; }   //우주에 있습니까?
        public bool faceRight;
        bool reserveChangeFaceRight;        // 행성이 바뀌면 착지할 때 faceRight를 초기화 예약한다.
        bool runON;

        [Header("EdgeFollow")]
        int currentEdgePointIndex = 0; // 현재 따라가고 있는 엣지 콜라이더의 점 인덱스

        public event System.Action ShootEvent;
        public event System.Action StartAimEvent;
        public event System.Action StopAimEvent;
        public event System.Action PlayerHitEvent;
        public event System.Action PlayerDieEvent;
        public event System.Action PlayerReloadEvent;

        //public event System.Action PlayerChangeState;

        //총기 관련
        float lastShootTime;
        float shootInterval = 0.3f;
        float gunRecoil;
        float reloadTime;
        bool isSingleShot;
        bool needReload;
        bool isReloading;
        Coroutine shootRoutine;
        

        PlayerWeapon playerWeapon;
        public  PlayerView playerView;
        CharacterGravity characterGravity;
        Rigidbody2D rb;
        Health health;
        Collider2D coll;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            characterGravity = GetComponent<CharacterGravity>();
            playerWeapon = GetComponent<PlayerWeapon>();
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

            //캐릭터가 현재 우주에 있는지 체크
            CheckOnSpace();
            

            //공중에 있는지 체크
            CheckOnAir();

            //에임 방향 체크
            AimCheck();
        }



        private void FixedUpdate()
        {
            if (state == PlayerState.Die)
                return;


            //캐릭터 회전
            RotateCharacterToGround();

            //실제 움직임 함수
            MoveUpdate();

            if (state == PlayerState.Stun) return;

            //이동 속도를 컨트롤
            SpeedControl();





        }


        void RotateCharacterToGround()
        {
            /*
            //건물 위 등 고정 중력방향에 들어갔을 때 
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
            
            /*
            int faceInt;
            Vector2 targetPointPos;
            Vector2 pastPointPos;
            int pastPoint;
            Vector2 direction;
            EdgeCollider2D edge = characterGravity.nearestEdgeCollider;

            faceInt = faceRight ? 1 : -1;
            targetPointPos = edge.transform.TransformPoint(edge.points[currentEdgePointIndex % (edge.points.Length - 1)]);   //콜라이더의 마지막  point 는 곧 0과 같다. 그래서 제외.

            pastPoint = (currentEdgePointIndex - faceInt + (edge.points.Length - 1)) % (edge.points.Length - 1);
            pastPointPos = edge.transform.TransformPoint(edge.points[pastPoint]);
            direction = (targetPointPos - pastPointPos) * faceInt;
            Vector2 normal = new Vector2(-direction.y, direction.x).normalized;

            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: normal);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeedOnLand * Time.deltaTime);
            */
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

        void CheckOnAir()
        {
            if (state != PlayerState.Jumping) return;
            
            //체크 시작할 때 딜레이를 주자. 
            if (Time.time - lastJumpTime < 0.1f)
                return;

            //떨어질 때에만 계산하기
            Vector2 jumpDir = rb.position - pastPosition;
            Vector2 upDir = transform.up;
            float angle = Vector2.Angle(upDir, jumpDir);
            if(angle > 90 && !onFalling)
            {
                onFalling = true;
                
            }

            //캐릭터의 중심에서, 발 밑으로 레이를 쏴서 캐릭터가 공중에 떠 있는지 검사한다.
            RaycastHit2D footHit = Physics2D.CircleCast(transform.position, 0.5f, transform.up * -1, 1f, LayerMask.GetMask("Planet"));
            if (footHit.collider != null)
            {
                if (footHit.distance < 0.1f)
                {   //착지 후 한번 실행
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
            //플레이어가 우주에 있는지 감지해서 onSpace에 적용
            bool pre = onSpace;
            onSpace = characterGravity.nearestPlanet == null ? true : false;
            //우주에 있는게 달라지면 애니메이션 변경
            if (pre != onSpace)
                playerView.ChangeState();
        }

        public void TryJump()
        {
            if (state == PlayerState.Die) return;

            if (state == PlayerState.Stun) return;

            if (state == PlayerState.Jumping)
            {
                if(airJump)
                    TryAirJump();
                return;
            }

            state = PlayerState.Jumping;
            airJump = true;

            lastJumpTime = Time.time;

            //지상에서 점프했을 때. OnAir 가 false인 상태에서 점프함.
            OnAir = true;

            Vector2 upVector = (Vector2)transform.position - characterGravity.nearestPoint;
            //Vector2 forwardVector = moveDir.normalized;     //moveDir(현재 이동 방향) 기준으로 앞쪽으로 점프시키므로, moveDir 수치를 천천히 줄여서 점프 방향 수정하는 것도 만들 수 있다. 
            Vector2 forwardVector = faceRight ? transform.right : transform.right * -1;
            Vector2 jumpDir = upVector + (forwardVector * moveForce * currSpeed);
            jumpDir = jumpDir.normalized;
            Vector2 jumpVector = jumpDir * jumpForce;

            rb.velocity = Vector2.zero;
            rb.AddForce(jumpVector, ForceMode2D.Impulse);

            //점프 사운드 출력
            //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
        }


        public void TryAirJump()
        {
            airJump = false;

            Vector2 upVector = (Vector2)transform.position - characterGravity.nearestPoint;
            //Vector2 forwardVector = moveDir.normalized;     //moveDir(현재 이동 방향) 기준으로 앞쪽으로 점프시키므로, moveDir 수치를 천천히 줄여서 점프 방향 수정하는 것도 만들 수 있다. 
            Vector2 forwardVector = faceRight ? transform.right : transform.right * -1;
            Vector2 jumpDir = upVector;
            jumpDir = jumpDir.normalized;
            Vector2 jumpVector = jumpDir * jumpForce * 0.5f;

            //rb.velocity = Vector2.zero;
            rb.AddForce(jumpVector, ForceMode2D.Impulse);
        }


        #region Movement



        public void TryMove(Vector2 inputAxisRaw)
        {
            if (state == PlayerState.Die) return;
            //플레이어가 넉백중일때는 이동 조작이 불가능. 
            if (state == PlayerState.Stun) return;


            this.inputAxis = inputAxisRaw;

            //한번씩만 계산하는게 퍼포먼스가 좋을지, 아니면 매턴 변수에 집어넣는게 퍼포먼스가 좋을지 모르겠음. 어차피 검사는 해야하잖아.. 

            if (runON)
            {
                targetSpeed = moveSpeed * runSpeedMultiply;
            }
            else
            {
                targetSpeed = moveSpeed;
            }


            //지상에 있을 동안의 움직임 
            if (!OnAir)
            {
                MoveOnLand();
            }
            //공중에 있을 때 움직임 
            else
            {
                MoveOnAir();
            }



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

                // 방향이 바뀔 때 view에서 Running애니메이션을 바꾸기 위해서    >> 다른 방법으로..
                playerView.ChangeState();
            }

            if (state != PlayerState.Running)
            {
                //달리기 수정되면서 한번 실행.
                state = PlayerState.Running;
            }

        }

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

        public void TryStop()
        {
            targetSpeed = 0;
        }




        //이동을 시작할 때 실행
        private void GetClosestPoint()
        {
            int closestPointIndex = 0;
            float closestDistance = float.MaxValue;

            // 엣지 콜라이더의 모든 포인트들을 순회하며 가장 가까운 포인트의 인덱스와 거리를 구함
            for (int i = 0; i < characterGravity.nearestEdgeCollider.points.Length; i++)
            {
                Vector2 pointPosition = characterGravity.nearestEdgeCollider.transform.TransformPoint(characterGravity.nearestEdgeCollider.points[i]);
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
        {   //행성이 바뀌면 right 방향을 재계산. 
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

            //움직이는 중에 속도가 0으로 가까워질 때
            if(currSpeed < 0.1f && state == PlayerState.Running)
            {
                inputAxis = Vector2.zero;
                //preMoveDir = Vector2.zero;
                moveDir = Vector2.zero;

                state = PlayerState.Idle;

                if (runON)
                {
                    StopRun();
                }
            }
        }


        void MoveUpdate()
        {
            if (currSpeed < acceleration)
                return;

            if (state == PlayerState.Running || state == PlayerState.Stun)
            {
                //움직임
                int faceInt;
                Vector2 targetPointPos;
                Vector2 pastPointPos;
                int pastPoint;
                Vector2 direction;
                Vector2 moveVector;
                float moveDist;
                EdgeCollider2D edge = characterGravity.nearestEdgeCollider;

                //목표로 할 포인트를 구한다.
                do
                {
                    //do문 내부 단순화. MoveDist만 있어도 된다. Edge Collider의.
                    faceInt = faceRight ? 1 : -1;
                    targetPointPos = edge.transform.TransformPoint(edge.points[currentEdgePointIndex % (edge.points.Length - 1)]);   //콜라이더의 마지막  point 는 곧 0과 같다. 그래서 제외.

                    pastPoint = (currentEdgePointIndex - faceInt + (edge.points.Length - 1)) % (edge.points.Length - 1);
                    pastPointPos = edge.transform.TransformPoint(edge.points[pastPoint]);
                    direction = (targetPointPos - pastPointPos) * faceInt;
                    Vector2 normal = new Vector2(-direction.y, direction.x).normalized;

                    moveVector = targetPointPos + (normal * 0.51f);         //플레이어의 높이다. 타겟 포인트에서 노말방향 * 플레이어 높이를 목표로 움직임.
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

                Debug.DrawLine(pastPointPos, targetPointPos, Color.red);

                // 오브젝트를 이동 방향으로 이동
                rb.MovePosition(rb.position + moveDir * currSpeed * Time.fixedDeltaTime);
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
            TryStopShoot();

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
            //aim을 중지한다.
            if(StopAimEvent != null)
                StopAimEvent();

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

            //aim을 회복한다. 
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
            //StopAllCoroutines();

            //aim을 중지한다. 
            if (StopAimEvent != null)
                StopAimEvent();

            //충돌 판정을 멈춘다
            //coll.enabled = false;

            //전역 이벤트 발동
            GameManager.Instance.PlayerIsDead();
        }

        #endregion


        public void TryRun()
        {   //input에서 실행
            if (runON)
            {
                StopRun();
            }
            else
            {
                runON = true;
            }
        }

        public void StopRun()
        {   //input에서 실행
            runON = false;
        }





        //총기 관련 


        #region Shoot

        void AimCheck()
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            aimDirection = (mousePos - transform.position).normalized;
            /*
            Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * aimDirection;
            Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);      //aim방향의 쿼터니언 값
            */
        }

        public void TryStartShoot()
        {
            //총 쏘기 이벤트를 시작한다. 단발총인 경우는 한 발만 발사. 연발 총인 경우는 발사 루틴을 지속

            //단발총인 경우에는 shootevent를 한번 실행한다. 쿨이 안되서 다시 눌러도 소용없음. 
            if (isSingleShot)
            {
                TryShoot();
            }
            //연발총인 경우에는 계속 쏘기 이벤트를 실행한다. 
            else
            {
                shootRoutine = StartCoroutine(ShootRepeater());
            }
        }
        public void TryStopShoot()
        {
            //총 쏘기 이벤트를 중단한다. 단발총인 경우는 총 쏘기 초기화. 연발 총의 경우에는 발사 중지. 

            //연발 총인 경우에는 계속 쏘기 이벤트를 중단한다. 

            if (!isSingleShot)
            {
                if(shootRoutine != null)
                {
                    StopCoroutine(shootRoutine);
                    shootRoutine = null;
                }
            }
        }

        IEnumerator ShootRepeater()
        {
            while (true)
            {
                //총알 쿨타임마다 계속 발사한다. 
                TryShoot();
                yield return null;
            }
        }


        public void TryShoot()  //이벤트 발생을 위해서 > PlayerWeapon, PlayerView 의 ShootEvent를 발생
        {
            if (state == PlayerState.Die) return;
            if (state == PlayerState.Stun)
                return;

            if (isReloading)
                return;

            //없으면 리로드하기
            if (needReload)
            {
                TryReload();
                return;
            }

            //총알 발사구가 행성 내부에 있다면 발사하지 않는다. 
            RaycastHit2D hit = Physics2D.Raycast(transform.position, aimDirection, 1f, LayerMask.GetMask("Planet"));
            if (hit.collider != null)
                return;



            float currentTime = Time.time;

            if (currentTime - lastShootTime > shootInterval)
            {
                lastShootTime = currentTime;
                //view 애니메이션 실행
                playerView.PreShoot();
                if (ShootEvent != null) ShootEvent();
                //총알 발사
                needReload = playerWeapon.PlayShootEvent();
            }

            if (runON)
            {
                StopRun();
            }


            if (onSpace)
            {
                //aim의 반대 방향으로 반동을 준다
                Vector2 recoilDir = aimDirection * -1f;
                rb.AddForce(recoilDir * gunRecoil, ForceMode2D.Impulse);
            }
        }

        public void TryReload()
        {
            //재장전 중이 아닐 때 
            if (isReloading)
                return;

            //재장전이 가능할떄 (currAmmo 가 maxAmmo보다 작을때)
            if (playerWeapon.CanReload())
            {
                StartCoroutine(ReloadRoutine());
            }

        }

        IEnumerator ReloadRoutine()
        {
            isReloading = true;
            if (StopAimEvent != null) StopAimEvent();
            if (PlayerReloadEvent != null) PlayerReloadEvent();

            yield return new WaitForSeconds(reloadTime);


            playerWeapon.ReloadAmmo();
            isReloading = false;
            needReload = false;
            if (StartAimEvent != null) StartAimEvent();


        }


        public void TryChangeWeapon(WeaponData _data)
        {
            //playerWeapon 변수에서 총기Interval 을 가져온다. 
            shootInterval = _data.ShootInterval;

            //1-Handed or 2-Handed 스킨 변경
            playerView.ChangeHand(_data.OneHand);

            //recoil 변경
            gunRecoil = _data.Recoil;

            //재장전 시간 변경
            reloadTime = _data.ReloadTime;

            //연발 or 단발 
            isSingleShot = _data.SingleShot;

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
        //ChangeState     //state가 바뀔 필요가 있을때마다 실행시키는것? 
    }
}

