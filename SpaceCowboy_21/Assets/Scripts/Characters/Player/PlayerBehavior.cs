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
    public float acceleration = 0.25f;      //�ӵ��� �ٲ� �� �󸶳� ������ ������.
    public float inputResetTime = 0.5f; //��ǲ�� �ʱ�ȭ�� Ÿ�̸�

    float currSpeed;    //���� �̵� �ӵ�
    float targetSpeed;  //�̵� ��ǥ�� �� ���ǵ�
    float speedMultiplier = 1.0f;
    int currentEdgePointIndex = 0; // ���� ���󰡰� �ִ� ���� �ݶ��̴��� �� �ε���

    Vector2[] planetPoints;     //�޾ƿ� ���� �༺�� points 
    Vector2 preInputAxis = Vector2.zero;  //���� �����ӿ� �����ߴ� ��ǲ ��
    Vector2 moveDir = Vector2.zero;     //�̵����� ����
    Vector2 inputAxis = Vector2.zero;  //������ ��ǲ ��

    [Header("Sliding")]
    public float slideSpeed;
    public bool boostOn = false;

    [Header("OnAir")]
    public float turnSpeedOnLand = 100f;
    public float onAirTime = 1f;   //���� ǥ�� �ð� Ÿ�� ����Ʈ.
    public float onAirVelocityRotateSpeed = 1f; //���߿� ���� �ִ� ĳ������ ���� ������ �������� ȸ����ų���� �ӵ� 
    public bool OnAir { get; set; }     //����� ������ �ν����Ϳ��� ���̰�
    float airTimer;

    [Header("KnockBack")]
    public float knockBackForce = 5f;
    public float stunTime = 1f;
    public float unHitabltTime = 3f;


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
    public bool activate { get; private set; }

    [Header("VFX")]
    //����Ʈ��
    public ParticleSystem shieldhitEffect;  //�ǵ尡 �¾��� �� ����� ����Ʈ
    public ParticleSystem slidingEffect;    //�����̵��Ҷ� ����� ����Ʈ
    [SerializeField] ParticleSystem boosterEffect; 

    //�ִϸ��̼� �̺�Ʈ
    public event System.Action PlayerIdleEvent;
    public event System.Action ShootEvent;
    public event System.Action PlayerHitEvent;
    public event System.Action PlayerDieEvent;
    public event System.Action PlayerJumpStartEvent;
    public event System.Action PlayerJumpEvent;
    public event Action<bool> PlayerAimEvent;


    [Header("Scripts")]
    //���� ��ũ��Ʈ��
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

        //���� �ִϸ��̼� ����
        if (PlayerAimEvent != null) PlayerAimEvent(true);

        //���� ���� �� ���⵵.. �����̸�?
        playerState = PlayerState.Jumping;

    }

    void Update()
    {
        if (!activate) return;

        //���ֿ� �ִ��� üũ
        onSpace = CheckOnSpace();

        //���߿� �ִ��� üũ
        if (!playerJump.doingDash && !boostOn) CheckOnAir();

        //JumpArrowViewer ������Ʈ
        playerJump.UpdateJumpVector();

        //�ѱ� ������Ʈ
        playerView.GetGunTipPos();

        //ĳ���� ���� ������Ʈ 
        aimRight = Vector2.SignedAngle(transform.up, aimDirection) < 0 ? true : false;

    }


    private void FixedUpdate()
    {
        if (!activate) return;

        //ĳ���� ȸ��
        if (!playerJump.doingDash && !boostOn) RotateCharacterToGround();

        //���߿� ���� ������ ��ǥ�� �������� ��������.
        if (OnAir)
        {
            airTimer += Time.fixedDeltaTime;

            if (boostOn)
            {
                if (!playerJump.Boost())
                {
                    //�ν�Ʈ�� �߰��� ������ �� 
                    //boostOn = false;
                    //playerJump.UsingBoost = false;
                    //characterGravity.activate = true;
                    TryStopBoost();
                }
                //�ν�Ʈ �� ȸ��
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

            //���� ������ �Լ�
            MoveUpdate();

            //�̵� �ӵ��� ��Ʈ��
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
        //normal�� transform.upward ������ �� ���̰� ũ�� ������ �����ش�. 
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

        //üũ ������ �� �����̸� ����. 
        if (Time.time - lastJumpTime < 0.1f)
            return;


        //ĳ������ �߽ɿ���, �� ������ ���̸� ���� ĳ���Ͱ� ���߿� �� �ִ��� �˻��Ѵ�.
        RaycastHit2D footHit = Physics2D.CircleCast(transform.position, 0.5f, transform.up * -1, 1f, LayerMask.GetMask("Planet"));
        if (footHit.collider != null)
        {
            if (footHit.distance < 0.1f)
            {
                //���� �� ����
                OnAir = false;

                rb.velocity = Vector2.zero;
                playerState = PlayerState.Idle;

                //���� ���� �ʱ�ȭ
                playerJump.ResetJump();

                //���� �� �༺�� �ٲ���� �� 
                if (planetChanged)
                {
                    ChangeMoveDirection(aimDirection);
                    planetChanged = false;
                }

                //���� �� ��ġ �ľ�
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
            //�ν�Ʈ�� Ȱ��ȭ ������ �� > Ȱ��ȭ
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
            //�����̵� ���� ���� �����̵� ���
            if (playerState == PlayerState.Sliding) StopSlide();

            OnAir = true;
            playerState = PlayerState.Jumping;

            playerJump.Jump();

        }

        //�ʱ�ȭ
        airTimer = 0;
        lastJumpTime = Time.time;

        //���� �ִϸ��̼� ���
        TryJumpEvent();

    }
    public void TryStopBoost()
    {
        //�ν�Ʈ�� Ȱ��ȭ �ǰ� ���� �� > ��Ȱ��ȭ
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

    public void TrySlide()
    {
        if (!activate) return;
        if (OnAir) return;

        //�����̵� ����
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

        //input ������ �ٲ�� ���� ����� ���� �Ѵ�.
        this.inputAxis = inputAxisRaw;
        if (inputAxis.x != preInputAxis.x || inputAxis.y != preInputAxis.y)
        {
            preInputAxis = inputAxis;

            ChangeMoveDirection(inputAxis);
            GetClosestPlanetPoint(transform.position);
        }

        //��ǥ �ӵ� ����
        targetSpeed = moveSpeed;

        //���¸� �޸���� ����(Idle�� ��쿡��)
        if (playerState == PlayerState.Idle)
        {
            //�޸��� �����Ǹ鼭 �ѹ� ����.
            playerState = PlayerState.Running;

        }
    }

    //�Է��� ���� ������
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

        //�ٶ���� �ϴ� ������ ����ΰ�.
        Vector2 upVec = ((Vector2)transform.position - characterGravity.nearestPoint).normalized;

        faceRight = Vector2.SignedAngle(upVec, inputDirection) < 0 ? true : false;


    }

    void GetClosestPlanetPoint(Vector2 pos)
    {
        // ���� ����� ����Ʈ�� �����´�.
        currentEdgePointIndex = characterGravity.nearestPlanet.GetClosestIndex(transform.position);
    }

    public void ChangePlanet()
    {   //�༺�� �ٲ�� �̺�Ʈ �߻�. CharacterGravity���� ��ȣ�� �޴´�.
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

        //�����̴� �߿� �ӵ��� 0���� ������� ��
        if (currSpeed < 0.1f && playerState == PlayerState.Running)
        {
            currSpeed = 0;

            inputAxis = Vector2.zero;
            //preMoveDir = Vector2.zero;
            moveDir = Vector2.zero;

            playerState = PlayerState.Idle;

        }

        //��AI���� �о���� ��
        playerVelocity = moveDir * currSpeed;

    }


    void MoveUpdate()
    {
        if (currSpeed < acceleration)
            return;

        if (playerState == PlayerState.Running || playerState == PlayerState.Sliding)
        {
            //������
            int faceInt = faceRight ? 1 : -1;
            int targetIndex;
            Vector2 targetPointPos;
            Vector2 moveVector;
            float moveDist;
            PolygonCollider2D coll = characterGravity.nearestCollider;

            //��ǥ�� �� ����Ʈ�� ���Ѵ�.
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

            // ������Ʈ�� �̵� �������� �̵�
            rb.MovePosition(rb.position + moveDir * currSpeed * speedMultiplier * Time.fixedDeltaTime);

        }
    }

    void MoveUpdateOnAir()
    {
        //�÷��̾ ���߿� ���� ��, �� �ִ� �ð��� ������� �༺ �������� ���� ���������. 
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
            //hit����Ʈ�� ��/�� ��� �ִ��� �˻� �� �ݴ� �������� ƨ�ܳ�����. 
            int hitIndex = Vector2.SignedAngle(transform.up, hitPos - (Vector2)transform.position) > 0 ? 1 : -1;

            Vector2 dir = (transform.right * hitIndex + transform.up) * 0.71f;
            LauchPlayer(dir, forceAmount);
        }
    }


    //IEnumerator KnockBackRoutine(Vector2 hitPoint, float time, float speed)
    //{
    //    yield return null;

    //    //StopAllCoroutines();

    //    //���� �κ��� ĳ���� �������� �������� ���Ѵ�
    //    Vector2 hitVec = hitPoint - (Vector2)transform.position;

    //    faceRight = Vector2.SignedAngle(transform.up, hitVec) < 0 ? false : true;

    //    //�ӵ� ����(�پ���� ����. Stun �� �Ǹ鼭 SpeedControl �� ������)
    //    currSpeed = speed;

    //    //ĳ������ ��ġ�� ���Ѵ�. 
    //    GetClosestPoint();


    //    //�÷��̾� Flip�� ���´�. 
    //    playerView.flipOn = false;


    //    //���� �ð����� �̵��Ѵ�. 
    //    float timer = 0f;
    //    while (timer < time)
    //    {
    //        timer += Time.deltaTime;

    //        yield return null;
    //    }
    //    //�÷��̾� Flip�� ȸ���Ѵ�. 
    //    playerView.flipOn = true;

    //    //playerState �� ȸ���Ѵ�. �����̴� ���¿��� �ڿ������� �ӵ��� �پ��� ���ؼ� running����. 
    //    playerState = PlayerState.Running;

    //    //������ �ʱ�ȭ�Ѵ�.
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

    #region ������ �ǰ�, ü�°���, ���� ����, �ǰ� �� ����, ���ۺҰ�
    Vector2 lastHitPos = Vector2.zero; //�������� ���� ���

    public void DamageEvent(float damage, Vector2 hitPoint)
    {
        if (!activate) return;
        if (playerJump.doingDash) return;
        
        //�������� ����
        if (GameManager.Instance.playerManager.RemoveDrone())
        {
            //����� ��� �¾����� ��
            lastHitPos = transform.position;

            KnockBackEvent(hitPoint, knockBackForce);

            //�ǰ� �ִϸ��̼� 
            if (PlayerHitEvent != null)
            {
                PlayerHitEvent();
            }
        }
        else
        {
            //��� �¾��� ����� ���� �� 
            if (health.AnyDamage(damage))
            {
                lastHitPos = transform.position;

                KnockBackEvent(hitPoint, knockBackForce);

                //ü���� ���� ���
                if (PlayerHitEvent != null)
                {
                    PlayerHitEvent();
                }

                //if (health.currShield > 0)
                //{
                //    //�ǵ尡 ���� ���
                //    shieldhitEffect.Play();
                //}
                //else
                //{
                //    //ü���� ���� ���
                //    if (PlayerHitEvent != null)
                //    {
                //        PlayerHitEvent();
                //    }
                //}

                //CinemachineShake.instance.ShakeCamera(2f, 0.1f);
            }
        }

        //�׾�����?
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
        //true �� ��� ü���� 0 ���Ϸ� �������ٴ� ��.
        playerState = PlayerState.Die;

        //�״� ��� ����
        if (PlayerDieEvent != null) PlayerDieEvent();

        //���� �ִϸ��̼� ���
        if (PlayerAimEvent != null) PlayerAimEvent(false);

        currSpeed = 0;
        targetSpeed = 0;

        activate = false;
        PlayerIgnoreProjectile(true);

        //�÷��̾� ���� ����
        GameManager.Instance.playerManager.DisablePlayerInput();
        StopAllCoroutines();
        unHitRoutine = null;
        unControlRoutine = null;

        //UI����
        playerJump.RemoveJumpArrow(false);
        playerWeapon.ShowWeaponSight(false);

        //���� �̺�Ʈ �ߵ�. ���鿡�� ĳ���Ͱ� �׾��ٰ� ����. 
        GameManager.Instance.PlayerIsDead();

        //������ ���� ���� üũ
        if (GameManager.Instance.playerManager.CanRespawn())
        {
            StartCoroutine(ResapwnRoutine());
        }
        else
        {
            //�κ�� �̵�
            StartCoroutine(DieRoutine());
        }
    }
    public void DeactivatePlayer(bool isActive)
    {
        activate = isActive;
        playerColl.enabled = isActive;
        PlayerIgnoreProjectile(!isActive);

        //�÷��̾� ���� ����
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

        //UI����
        playerJump.RemoveJumpArrow(isActive);
        playerWeapon.ShowWeaponSight(isActive);

        //���� �ִϸ��̼� ���
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


    //�Ѿ˿� ���� �� �ִ�.
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
    //    //�÷��̾ �ش� ���� �ӵ��� �ʹ� ������ �ӵ� �߰��� �����Ѵ�. 
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

