using System;
using System.Collections;
using UnityEngine;


public class PlayerBehavior : MonoBehaviour, IEnemyHitable, ITarget, ITeleportable
{
    [Header("State")]
    public PlayerState playerState = PlayerState.Idle;

    [Header("Jump")]
    float lastJumpTime;

    [Header("Movement")]
    public float moveSpeed;
    public float acceleration = 0.25f;      //속도가 바뀔 때 얼마나 빠르게 변할지.
    public float inputResetTime = 0.5f; //인풋이 초기화될 타이머

    float currSpeed;    //현재 이동 속도
    float targetSpeed;  //이동 목표가 될 스피드
    float speedMultiplier = 1.0f;
    int currentEdgePointIndex = 0; // 현재 따라가고 있는 엣지 콜라이더의 점 인덱스

    Vector2[] planetPoints;     //받아온 현재 행성의 points 
    Vector2 preInputAxis = Vector2.zero;  //이전 프레임에 조작했던 인풋 값
    Vector2 moveDir = Vector2.zero;     //이동중인 방향
    Vector2 inputAxis = Vector2.zero;  //움직임 인풋 값

    [Header("Sliding")]
    public float slideSpeed;
    public bool boostOn = false;

    [Header("OnAir")]
    public float turnSpeedOnLand = 100f;
    public float onAirTime = 1f;   //공중 표류 시간 타임 리미트.
    public float onAirVelocityRotateSpeed = 1f; //공중에 오래 있는 캐릭터의 진행 방향을 지면으로 회전시킬때의 속도 
    public bool OnAir { get; set; }     //디버그 용으로 인스펙터에서 보이게
    float airTimer;

    [Header("KnockBack")]
    public float knockBackForce = 5f;
    public float stunTime = 1f;
    public float unHitabltTime = 3f;


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
    public bool activate { get; private set; }

    [Header("VFX")]
    //이펙트들
    public ParticleSystem shieldhitEffect;  //실드가 맞았을 때 출력할 이펙트
    public ParticleSystem slidingEffect;    //슬라이딩할때 출력할 이펙트
    [SerializeField] ParticleSystem boosterEffect; 

    //애니메이션 이벤트
    public event System.Action PlayerIdleEvent;
    public event System.Action ShootEvent;
    public event System.Action PlayerHitEvent;
    public event System.Action PlayerDieEvent;
    public event System.Action PlayerJumpStartEvent;
    public event System.Action PlayerJumpEvent;
    public event Action<bool> PlayerAimEvent;


    [Header("Scripts")]
    //본인 스크립트들
    PlayerWeapon playerWeapon;
    PlayerView playerView;
    PlayerJump playerJump;
    CharacterGravity characterGravity;
    Rigidbody2D rb;
    PlayerHealth health;
    Collider2D playerColl;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        characterGravity = GetComponent<CharacterGravity>();
        playerWeapon = GetComponent<PlayerWeapon>();
        health = GetComponent<PlayerHealth>();
        playerColl = GetComponent<Collider2D>();
        playerView = GetComponentInChildren<PlayerView>();
        playerJump = GetComponent<PlayerJump>();

    }

    public void InitPlayer()
    {
        activate = false;
        health.ResetHealth();

        GameManager.Instance.playerManager.EnablePlayerInput();
        PlayerIgnoreProjectile(false);
        playerJump.RemoveJumpArrow(true);
        playerWeapon.ShowWeaponSight(true);

        //조준 애니메이션 시작
        if (PlayerAimEvent != null) PlayerAimEvent(true);

        //문제 생길 것 같기도.. 공중이면?
        playerState = PlayerState.Jumping;

    }

    void Update()
    {
        if (!activate) return;

        //우주에 있는지 체크
        onSpace = CheckOnSpace();

        //공중에 있는지 체크
        if (!playerJump.doingDash && !boostOn) CheckOnAir();

        //JumpArrowViewer 업데이트
        playerJump.UpdateJumpVector();

        //총구 업데이트
        playerView.GetGunTipPos();

        //캐릭터 방향 업데이트 
        aimRight = Vector2.SignedAngle(transform.up, aimDirection) < 0 ? true : false;

    }


    private void FixedUpdate()
    {
        if (!activate) return;

        //캐릭터 회전
        if (!playerJump.doingDash && !boostOn) RotateCharacterToGround();

        //공중에 오래 있으면 지표면 방향으로 떨어진다.
        if (OnAir)
        {
            airTimer += Time.fixedDeltaTime;

            if (boostOn)
            {
                if (!playerJump.Boost())
                {
                    //부스트가 중간에 꺼졌을 때 
                    //boostOn = false;
                    //playerJump.UsingBoost = false;
                    //characterGravity.activate = true;
                    TryStopBoost();
                }
                //부스트 중 회전
                RotateToVector(aimDirection, turnSpeedOnLand);
            }
            else if (airTimer > onAirTime)
            {
                MoveUpdateOnAir();

            }
        }
        else
        {
            //!OnAir

            //실제 움직임 함수
            MoveUpdate();

            //이동 속도를 컨트롤
            SpeedControl();
        }
    }


    #region RotateCharacterToGround
    void RotateCharacterToGround()
    {
        if (onSpace)
            return;

        Vector2 upVec = ((Vector2)transform.position - characterGravity.nearestPoint).normalized;
        RotateToVector(upVec, turnSpeedOnLand);

    }

    void RotateToVector(Vector2 normal, float turnSpeed)
    {
        Vector3 vectorToTarget = normal;
        //normal과 transform.upward 사이의 값 차이가 크면 보정을 가해준다. 
        float rotateAngle = Vector2.Angle(transform.up, normal);

        int turnspeedMultiplier = Mathf.Clamp(Mathf.RoundToInt(rotateAngle * 0.1f), 1, 10);

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
            {
                //착지 후 실행
                OnAir = false;

                rb.velocity = Vector2.zero;
                playerState = PlayerState.Idle;

                //점프 변수 초기화
                playerJump.ResetJump();

                //점프 중 행성이 바뀌었을 때 
                if (planetChanged)
                {
                    ChangeMoveDirection(aimDirection);
                    planetChanged = false;
                }

                //착지 후 위치 파악
                GetClosestPlanetPoint(transform.position);

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

    }

    bool CheckOnSpace()
    {
        bool onSpace;
        if (characterGravity.nearestPlanet == null)
        {
            onSpace = true;
        }
        else
        {
            onSpace = false;
        }
        return onSpace;
    }


    public void TryJump()
    {
        if (!activate) return;

        if (OnAir)
        {
            //부스트가 활성화 가능할 때 > 활성화
            if (playerJump.Boost() && !boostOn)
            {
                boostOn = true;
                playerJump.UsingBoost = true;
                characterGravity.activate = false;

                boosterEffect.Play();
            }
        }
        else if (playerState != PlayerState.Jumping)
        {
            //슬라이딩 중일 때는 슬라이딩 취소
            if (playerState == PlayerState.Sliding) StopSlide();

            OnAir = true;
            playerState = PlayerState.Jumping;

            playerJump.Jump();

        }

        //초기화
        airTimer = 0;
        lastJumpTime = Time.time;

        //점프 애니메이션 출력
        TryJumpEvent();

    }
    public void TryStopBoost()
    {
        //부스트가 활성화 되고 있을 때 > 비활성화
        if (boostOn)
        {
            boostOn = false;
            playerJump.UsingBoost = false;
            characterGravity.activate = true;

            boosterEffect.Stop();

        }

    }

    public void TryDash()
    {
        if (!activate) return;
        if (OnAir)
        {
            if (playerJump.Dash())
            {
                playerJump.UsingBoost = false;
            }

        }
    }

    public void ShowBoostEffect(bool show)
    {
        if (show)
        {
            boosterEffect.Play();
        }
        else
        {
            boosterEffect.Stop();
        }
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

    public void TrySlide()
    {
        if (!activate) return;
        if (OnAir) return;

        //슬라이딩 관련
        playerState = PlayerState.Sliding;

        speedMultiplier = slideSpeed;

        slidingEffect.Play();
        slidingEffect.transform.localScale = new Vector3(faceRight ? 1 : -1, 1, 1);
    }

    public void StopSlide()
    {
        if (playerState == PlayerState.Sliding)
        {
            playerState = PlayerState.Running;
        }

        targetSpeed = 0;

        speedMultiplier = 1;

        slidingEffect.Stop();
    }

    #endregion

    #region Movement

    public void TryMove(Vector2 inputAxisRaw)
    {
        if (!activate) return;
        if (playerState == PlayerState.Jumping) return;

        //input 방향이 바뀌면 방향 계산을 새로 한다.
        this.inputAxis = inputAxisRaw;
        if (inputAxis.x != preInputAxis.x || inputAxis.y != preInputAxis.y)
        {
            preInputAxis = inputAxis;

            ChangeMoveDirection(inputAxis);
            GetClosestPlanetPoint(transform.position);
        }

        //목표 속도 설정
        targetSpeed = moveSpeed;

        //상태를 달리기로 변경(Idle인 경우에만)
        if (playerState == PlayerState.Idle)
        {
            //달리기 수정되면서 한번 실행.
            playerState = PlayerState.Running;

        }
    }

    //입력이 완전 없으면
    public void TryStop()
    {
        if (playerState == PlayerState.Sliding) StopSlide();

        preInputAxis = Vector2.zero;
        ChangeMoveDirection(Vector2.zero);

        targetSpeed = 0;
    }

    public void TryPause()
    {
        if (playerState == PlayerState.Sliding) playerState = PlayerState.Running;
        targetSpeed = 0;
    }


    void ChangeMoveDirection(Vector2 inputDirection)
    {
        if (inputDirection == Vector2.zero) return;

        //바라봐야 하는 방향이 어디인가.
        Vector2 upVec = ((Vector2)transform.position - characterGravity.nearestPoint).normalized;

        faceRight = Vector2.SignedAngle(upVec, inputDirection) < 0 ? true : false;


    }

    void GetClosestPlanetPoint(Vector2 pos)
    {
        // 가장 가까운 포인트를 가져온다.
        currentEdgePointIndex = characterGravity.nearestPlanet.GetClosestIndex(transform.position);
    }

    public void ChangePlanet()
    {   //행성이 바뀌면 이벤트 발생. CharacterGravity에서 신호를 받는다.
        planetChanged = true;
        if (characterGravity.nearestPlanet != null)
        {
            planetPoints = characterGravity.nearestPlanet.GetPoints(0.51f);

        }

    }

    void SpeedControl()
    {
        if (currSpeed < targetSpeed)
        {
            currSpeed += acceleration;
        }
        else if (currSpeed > targetSpeed)
        {
            currSpeed -= acceleration;
        }

        //움직이는 중에 속도가 0으로 가까워질 때
        if (currSpeed < 0.1f && playerState == PlayerState.Running)
        {
            currSpeed = 0;

            inputAxis = Vector2.zero;
            //preMoveDir = Vector2.zero;
            moveDir = Vector2.zero;

            playerState = PlayerState.Idle;

        }

        //적AI에서 읽어오는 값
        playerVelocity = moveDir * currSpeed;

    }


    void MoveUpdate()
    {
        if (currSpeed < acceleration)
            return;

        if (playerState == PlayerState.Running || playerState == PlayerState.Sliding)
        {
            //움직임
            int faceInt = faceRight ? 1 : -1;
            int targetIndex;
            Vector2 targetPointPos;
            Vector2 moveVector;
            float moveDist;
            PolygonCollider2D coll = characterGravity.nearestCollider;

            //목표로 할 포인트를 구한다.
            do
            {
                targetIndex = (currentEdgePointIndex + faceInt + (coll.points.Length - 1)) % ((coll.points.Length - 1));
                targetPointPos = planetPoints[targetIndex];

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
        if (!onSpace)
        {
            Vector2 vec = (characterGravity.nearestPoint - (Vector2)transform.position).normalized;
            float vel = rb.velocity.magnitude;

            rb.velocity = Vector2.Lerp(rb.velocity, vec * vel, onAirVelocityRotateSpeed * Time.fixedDeltaTime);
        }
    }


    #endregion

    #region KnockBack

    public void KnockBackEvent(Vector2 hitPos, float forceAmount)
    {
        if (!activate) return;
        PlayerUncontrollable(stunTime);
        PlayerUnhittable(unHitabltTime);

        if (OnAir)
        {
            Vector2 dir = (Vector2)transform.position - hitPos;
            dir = dir.normalized;

            rb.velocity = Vector2.zero;
            rb.AddForce(dir * forceAmount, ForceMode2D.Impulse);
        }
        else
        {
            //hit포인트가 좌/우 어디에 있는지 검사 후 반대 방향으로 튕겨나간다. 
            int hitIndex = Vector2.SignedAngle(transform.up, hitPos - (Vector2)transform.position) > 0 ? 1 : -1;

            Vector2 dir = (transform.right * hitIndex + transform.up) * 0.71f;
            LauchPlayer(dir, forceAmount);
        }
    }


    //IEnumerator KnockBackRoutine(Vector2 hitPoint, float time, float speed)
    //{
    //    yield return null;

    //    //StopAllCoroutines();

    //    //맞은 부분이 캐릭터 앞쪽인지 뒷쪽인지 구한다
    //    Vector2 hitVec = hitPoint - (Vector2)transform.position;

    //    faceRight = Vector2.SignedAngle(transform.up, hitVec) < 0 ? false : true;

    //    //속도 지정(줄어들지 않음. Stun 이 되면서 SpeedControl 을 껏으니)
    //    currSpeed = speed;

    //    //캐릭터의 위치를 구한다. 
    //    GetClosestPoint();


    //    //플레이어 Flip을 막는다. 
    //    playerView.flipOn = false;


    //    //일정 시간동안 이동한다. 
    //    float timer = 0f;
    //    while (timer < time)
    //    {
    //        timer += Time.deltaTime;

    //        yield return null;
    //    }
    //    //플레이어 Flip을 회복한다. 
    //    playerView.flipOn = true;

    //    //playerState 를 회복한다. 움직이는 상태에서 자연스럽게 속도가 줄어들기 위해서 running으로. 
    //    playerState = PlayerState.Running;

    //    //조작을 초기화한다.
    //    inputAxis = Vector2.zero;
    //    preInputAxis = Vector2.zero;

    //}


    //IEnumerator KnockBackOnAirRoutine(float knockBackTime)
    //{
    //    yield return new WaitForSeconds(knockBackTime);

    //    if (OnAir)
    //    {
    //        playerState = PlayerState.Jumping;

    //    }
    //    else
    //    {
    //        playerState = PlayerState.Idle;

    //    }
    //}

    #endregion

    #region 데미지 피격, 체력관련, 죽음 관련, 피격 후 스턴, 조작불가
    Vector2 lastHitPos = Vector2.zero; //마지막에 맞은 장소

    public void DamageEvent(float damage, Vector2 hitPoint)
    {
        if (!activate) return;
        if (playerJump.doingDash) return;
        
        //데미지를 적용
        if (GameManager.Instance.playerManager.RemoveDrone())
        {
            //드론이 대신 맞아줬을 때
            lastHitPos = transform.position;

            KnockBackEvent(hitPoint, knockBackForce);

            //피격 애니메이션 
            if (PlayerHitEvent != null)
            {
                PlayerHitEvent();
            }
        }
        else
        {
            //대신 맞아줄 드론이 없을 때 
            if (health.AnyDamage(damage))
            {
                lastHitPos = transform.position;

                KnockBackEvent(hitPoint, knockBackForce);

                //체력이 닳은 경우
                if (PlayerHitEvent != null)
                {
                    PlayerHitEvent();
                }

                //if (health.currShield > 0)
                //{
                //    //실드가 닳은 경우
                //    shieldhitEffect.Play();
                //}
                //else
                //{
                //    //체력이 닳은 경우
                //    if (PlayerHitEvent != null)
                //    {
                //        PlayerHitEvent();
                //    }
                //}

                //CinemachineShake.instance.ShakeCamera(2f, 0.1f);
            }
        }

        //죽었나요?
        if (health.IsDead())
        {
            DeadEvent();

            return;
        }


    }

    public bool healEvent(float amount)
    {
        return health.HealthUp(amount);
    }

    public void DeadEvent()
    {
        //true 인 경우 체력이 0 이하로 떨어졌다는 뜻.
        playerState = PlayerState.Die;

        //죽는 모션 실행
        if (PlayerDieEvent != null) PlayerDieEvent();

        //조준 애니메이션 취소
        if (PlayerAimEvent != null) PlayerAimEvent(false);

        currSpeed = 0;
        targetSpeed = 0;

        activate = false;
        PlayerIgnoreProjectile(true);

        //플레이어 조작 정지
        GameManager.Instance.playerManager.DisablePlayerInput();
        StopAllCoroutines();
        unHitRoutine = null;
        unControlRoutine = null;

        //UI제거
        playerJump.RemoveJumpArrow(false);
        playerWeapon.ShowWeaponSight(false);

        //전역 이벤트 발동. 적들에게 캐릭터가 죽었다고 전달. 
        GameManager.Instance.PlayerIsDead();

        //리스폰 가능 여부 체크
        if (GameManager.Instance.playerManager.CanRespawn())
        {
            StartCoroutine(ResapwnRoutine());
        }
        else
        {
            //로비로 이동
            StartCoroutine(DieRoutine());
        }
    }
    public void DeactivatePlayer(bool isActive)
    {
        activate = isActive;
        playerColl.enabled = isActive;
        PlayerIgnoreProjectile(!isActive);

        //플레이어 조작 정지
        if (!isActive)
        {
            GameManager.Instance.playerManager.DisablePlayerInput();
        }
        else
        {
            GameManager.Instance.playerManager.EnablePlayerInput();
        }
        StopAllCoroutines();
        unHitRoutine = null;
        unControlRoutine = null;

        //UI제거
        playerJump.RemoveJumpArrow(isActive);
        playerWeapon.ShowWeaponSight(isActive);

        //조준 애니메이션 취소
        if (PlayerAimEvent != null) PlayerAimEvent(isActive);

        //this.gameObject.SetActive(false);   
    }

    IEnumerator ResapwnRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        GameManager.Instance.RespawnPlayer(lastHitPos, Quaternion.identity);
    }

    IEnumerator DieRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        GameManager.Instance.TransitionFadeOut(true);
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.ReturnToLobby();
    }


    //총알에 맞을 수 있다.
    public void PlayerIgnoreProjectile(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("EnemyProj"), ignore);
    }

    Coroutine unHitRoutine;
    public void PlayerUnhittable(float second)
    {
        if (unHitRoutine != null) StopCoroutine(unHitRoutine);
        unHitRoutine = StartCoroutine(UnhittableRoutine(second));
    }
    IEnumerator UnhittableRoutine(float sec)
    {
        PlayerIgnoreProjectile(true);
        yield return new WaitForSeconds(sec);
        PlayerIgnoreProjectile(false);
    }

    Coroutine unControlRoutine;
    void PlayerUncontrollable(float sec)
    {
        if (unControlRoutine != null) StopCoroutine(unControlRoutine);
        unControlRoutine = StartCoroutine(UncontrolRoutine(sec));
    }
    IEnumerator UncontrolRoutine(float sec)
    {
        GameManager.Instance.playerManager.DisablePlayerInput();
        yield return new WaitForSeconds(sec);
        GameManager.Instance.playerManager.EnablePlayerInput();


    }
    public void PlayerInputControl(bool enable)
    {
        if (enable)
            GameManager.Instance.playerManager.EnablePlayerInput();
        else GameManager.Instance.playerManager.DisablePlayerInput();
    }


    #endregion

    #region Shoot & Throw


    public void TryShoot()
    {
        playerWeapon.ShootProcess();
    }

    public void TryResetShoot()
    {
        //playerWeapon.shootOnce = false;
        playerWeapon.ShootOffProcess();
    }

    public void TryChangeWeaponSkin(WeaponData weaponData)
    {
        playerView.SetSkin(weaponData);
    }


    //public void GunRecoil(float amount, Vector2 dir)
    //{
    //    if (!OnAir) return;
    //    //플레이어가 해당 방향 속도가 너무 빠르면 속도 추가를 정지한다. 
    //    Vector2 velocity = rb.velocity;

    //    if (velocity.magnitude > velocityOnAirLimit && Vector2.Angle(dir, velocity) < recoilLimitAngle) return;

    //    rb.AddForce(dir * amount, ForceMode2D.Impulse);
    //}

    #endregion

    #region Events

    public void TryIdleEvent()
    {
        if (PlayerIdleEvent != null) PlayerIdleEvent();
    }

    public void TryJumpEvent()
    {
        if (PlayerJumpEvent != null) PlayerJumpEvent();
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


    public Collider2D GetCollider()
    {
        return playerColl;
    }
}




public enum PlayerState
{
    Idle,
    Running,
    Jumping,
    Die,
    Sliding
}

