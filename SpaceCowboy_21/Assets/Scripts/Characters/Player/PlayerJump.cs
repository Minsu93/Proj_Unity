using System.Collections;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    [Header("Jump")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] float maxAngleOnLand = 70f;

    [Header("Dash")]
    [SerializeField] float dashForce = 18f;
    [SerializeField] float dashRadius = 0.5f;
    [SerializeField] int dashMaxCount = 1;
    [SerializeField] float dashTime = 0.5f; //대시 시작 시 무적 시간 (대시 작동 시간)
    //[SerializeField] float dashDamage = 10f;    //데미지
    //[SerializeField] float dashKnockBackPower = 5f; //넉백 파워
    [SerializeField] float unHittableTime = 1f;//대시 공격 성공 후 무적시간
    [SerializeField] float dashGaugeCost = 100.0f;

    [Header("Boost")]
    [SerializeField] float boostForce = 10f;
    [SerializeField] float speedLimit = 15.0f;
    [SerializeField] float rechargeSpeed = 10.0f;
    [SerializeField] float boostGaugeRate = 30.0f;
    [SerializeField] float boostMaxGauge = 100.0f;
    float boostCurGauge;
    [SerializeField] float waitSecondToRecharge = 2.0f;
    float rechargeTimer;

    public bool doingDash;    //슈퍼점프
    int dashCount = 0;
    Vector2 jumpVector;     //실제 점프 방향벡터
    //ctor2 preVelocity;    //KnockBack 속도 계산용도.
    bool usingBoost;
    public bool UsingBoost
    {
        get { return usingBoost; } 
        set
        {
            if(value)
            {
                //true일 때 
            }
            else
            {
                //사용을 중단했을 때 
                rechargeTimer = 0;
            }
            usingBoost = value;
        }
    }

    //점프 Trail 관련
    public TrailRenderer sJumpTrail;    //슈퍼점프 트레일
    [SerializeField] ParticleSystem impactParticle;
    [SerializeField] ParticleSystem noImpactParticle;


    Rigidbody2D rb;
    JumpArrowViewer jumpArrowViewer;
    PlayerBehavior playerBehavior;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();   
        playerBehavior = GetComponent<PlayerBehavior>();
        jumpArrowViewer = GetComponentInChildren<JumpArrowViewer>();

        boostCurGauge = boostMaxGauge;
    }


    private void OnEnable()
    {
        GameManager.Instance.PlayerTeleportStart += ClearTrail;
        GameManager.Instance.PlayerTeleportEnd += ShowTrail;
        playerBehavior.PlayerHitEvent += ResetDash;
    }

    /// <summary>
    /// 부스트를 사용하지 않고 일정 시간(rechargeTimer)이 지나야 충전이 시작된다. 
    /// 슬라이딩으로 부스트 사용시 게이지 감소도 여기서. 
    /// 마지막으로 UI를 업데이트한다.
    /// </summary>
    private void Update()
    {
        if(!usingBoost)
        {
            if(rechargeTimer < waitSecondToRecharge)
            {
                rechargeTimer += Time.deltaTime;
            }
            //충전 시작
            else 
            {
                if (boostCurGauge < boostMaxGauge)
                {
                    boostCurGauge += rechargeSpeed * Time.deltaTime;
                    GameManager.Instance.playerManager.UpdateBoosterUI(boostCurGauge / boostMaxGauge);
                }
                else if( boostCurGauge > boostMaxGauge)
                {
                    boostCurGauge = boostMaxGauge;
                    GameManager.Instance.playerManager.HideBoosterUI();
                }
            }
        }
        else
        {
            //부스터 사용 시 게이지 감소
            if(boostCurGauge > 0)
            {
                boostCurGauge -= boostGaugeRate * Time.deltaTime;
                GameManager.Instance.playerManager.UpdateBoosterUI(boostCurGauge / boostMaxGauge);
            }
        }
        //UI
        
    }


    public void ClearTrail()
    {
        if(sJumpTrail != null && sJumpTrail.emitting)
            sJumpTrail.Clear();
    }
    public void ShowTrail()
    {
        if(doingDash)
            sJumpTrail.emitting = true;
        if (sJumpTrail != null && sJumpTrail.emitting)
            sJumpTrail.Clear();
    }

    #region  Jump

    public void RemoveJumpArrow(bool activate)
    {
        jumpArrowViewer.gameObject.SetActive(activate);
    }

    //점프에 사용되는 변수 JumpVector를 업데이트한다.
    public void UpdateJumpVector()
    {
        //점프 각도 조절
        if (!playerBehavior.OnAir)
        {
            //float force = jumpForce;
            Vector2 upVec = transform.up;
            float angle = Vector2.SignedAngle(upVec, playerBehavior.aimDirection);

            //if (Mathf.Abs(angle) > maxAngleOnLand)
            //{
            //    //점프 각도가 과하면 점프력이 낮아진다. 
            //    force = force * (maxAngleOnLand / Mathf.Abs(angle));
            //}

            angle = Mathf.Clamp(angle, -maxAngleOnLand, maxAngleOnLand);
            jumpVector = Quaternion.AngleAxis(angle, Vector3.forward) * transform.up;
        }
        else
        {
            jumpVector = playerBehavior.aimDirection;
        }

        //jumpArrowViewer.trajectoryVector = jumpVector;
        jumpArrowViewer.SetArrowTip(jumpVector);
    }

    //일반 점프
    public void Jump()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(jumpVector * jumpForce, ForceMode2D.Impulse);

        //점프 사운드 출력
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
    }

    //점프 리셋
    public void ResetJump()
    {
        dashCount = 0;

        //대시중일때.
        if (doingDash)
        {
            ResetDash();
        }
    }

    #endregion

    #region Dash
    //공격형 점프
    public bool Dash()
    {
        //if(boostCurGauge < dashGaugeCost)
        //{
        //    return false;
        //}
        if (dashCount >= dashMaxCount)
        {
            return false;
        }

        dashCount++;
        //boostCurGauge = 0;

        doingDash = true;
        StartCoroutine(DashAttackRoutine());

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpVector * dashForce, ForceMode2D.Impulse);

        sJumpTrail.emitting = true;
        sJumpTrail.Clear();

        return true;
    }

    void ResetDash()
    {
        doingDash = false;
        sJumpTrail.emitting = false;
        sJumpTrail.Clear();

        playerBehavior.PlayerInputControl(true);
        playerBehavior.PlayerIgnoreProjectile(false);

    }

    IEnumerator DashAttackRoutine()
    {
        //대시 시작
        float timer = 0;
        bool attackActivate = true;
        playerBehavior.PlayerInputControl(false);
        playerBehavior.PlayerIgnoreProjectile(true);
        int targetLayer = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("StageObject");

        while(doingDash && timer < dashTime)
        {
            timer += Time.deltaTime;
            
            //행성 및 적 부딪침 감지
            Collider2D hit = Physics2D.OverlapCircle(transform.position, dashRadius,targetLayer);
            if(hit != null)
            {
                if(hit.TryGetComponent<ITarget>(out ITarget target))
                {

                    float force;
                    ParticleSystem kickParticle;

                    //적 데미지 & 넉백
                    if (hit.TryGetComponent<IKickable>(out IKickable kickable))
                    {
                        kickable.Kicked(this.transform.position);

                        playerBehavior.PlayerUnhittable(unHittableTime);
                        force = playerBehavior.knockBackForce;
                        kickParticle = impactParticle;

                        //dashCount = 0;
                    }
                    else
                    {
                        force = playerBehavior.knockBackForce;
                        kickParticle = noImpactParticle;
                    }

                    playerBehavior.KnockBackEvent(hit.transform.position, force);
                    GameManager.Instance.particleManager.GetParticle(kickParticle, transform.position, transform.rotation);

                    //대시 초기화
                    ResetDash();
                }
                
                
                break;
            }
            yield return null;

            if (!attackActivate) yield break;
        }

        //대시 종료
        ResetDash();


    }
    #endregion

    /// <summary>
    /// PlayerBehavior에서 OnAir상태에 Slide버튼을 누르면 부스트. 버튼을 눌렀을 때 Boost 가능여부 체크하고 가능하면 FixedUpdate에서 업데이트. 
    /// 불가능하면 실행하지 않는다. 
    /// </summary>
    /// <returns></returns>
    #region Boost
    public bool Boost()
    {
        if(boostCurGauge <= 0)
        {
            return false;
        }

        //부스트 최소, 최대 값
        float min = 1.0f;
        float max = 3.0f;
        //현재 속도와 aimDirection이 많이 차이날수록 부스터 파워가 커짐. 그렇지 않으면 약해짐.   
        float angle = GetAngle(rb.velocity, jumpVector);
        float lerp = Mathf.Lerp(min, max, Mathf.Abs(angle)/ 180f);
        //부스트
        rb.AddForce(jumpVector * boostForce * lerp, ForceMode2D.Force);

        
        //속도 제한
        Vector2 dir = rb.velocity.normalized;
        float speed = rb.velocity.magnitude;
        if(speed > speedLimit)
        {
            speed = speedLimit;
            rb.velocity = dir * speed;
        }

        return true;
    }

    // return : -180 ~ 180 degree (for unity)
    public static float GetAngle(Vector2 vStart, Vector2 vEnd)
    {
        Vector2 v = vEnd - vStart;

        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }
    #endregion
}
