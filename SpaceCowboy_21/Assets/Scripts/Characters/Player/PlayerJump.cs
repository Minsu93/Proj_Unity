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
    [SerializeField] float dashTime = 0.5f; //��� ���� �� ���� �ð� (��� �۵� �ð�)
    //[SerializeField] float dashDamage = 10f;    //������
    //[SerializeField] float dashKnockBackPower = 5f; //�˹� �Ŀ�
    [SerializeField] float unHittableTime = 1f;//��� ���� ���� �� �����ð�
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

    public bool doingDash;    //��������
    int dashCount = 0;
    Vector2 jumpVector;     //���� ���� ���⺤��
    //ctor2 preVelocity;    //KnockBack �ӵ� ���뵵.
    bool usingBoost;
    public bool UsingBoost
    {
        get { return usingBoost; } 
        set
        {
            if(value)
            {
                //true�� �� 
            }
            else
            {
                //����� �ߴ����� �� 
                rechargeTimer = 0;
            }
            usingBoost = value;
        }
    }

    //���� Trail ����
    public TrailRenderer sJumpTrail;    //�������� Ʈ����
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
    /// �ν�Ʈ�� ������� �ʰ� ���� �ð�(rechargeTimer)�� ������ ������ ���۵ȴ�. 
    /// �����̵����� �ν�Ʈ ���� ������ ���ҵ� ���⼭. 
    /// ���������� UI�� ������Ʈ�Ѵ�.
    /// </summary>
    private void Update()
    {
        if(!usingBoost)
        {
            if(rechargeTimer < waitSecondToRecharge)
            {
                rechargeTimer += Time.deltaTime;
            }
            //���� ����
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
            //�ν��� ��� �� ������ ����
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

    //������ ���Ǵ� ���� JumpVector�� ������Ʈ�Ѵ�.
    public void UpdateJumpVector()
    {
        //���� ���� ����
        if (!playerBehavior.OnAir)
        {
            //float force = jumpForce;
            Vector2 upVec = transform.up;
            float angle = Vector2.SignedAngle(upVec, playerBehavior.aimDirection);

            //if (Mathf.Abs(angle) > maxAngleOnLand)
            //{
            //    //���� ������ ���ϸ� �������� ��������. 
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

    //�Ϲ� ����
    public void Jump()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(jumpVector * jumpForce, ForceMode2D.Impulse);

        //���� ���� ���
        //AudioManager.instance.PlaySfx(AudioManager.Sfx.Jump);
    }

    //���� ����
    public void ResetJump()
    {
        dashCount = 0;

        //������϶�.
        if (doingDash)
        {
            ResetDash();
        }
    }

    #endregion

    #region Dash
    //������ ����
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
        //��� ����
        float timer = 0;
        bool attackActivate = true;
        playerBehavior.PlayerInputControl(false);
        playerBehavior.PlayerIgnoreProjectile(true);
        int targetLayer = 1 << LayerMask.NameToLayer("Enemy") | 1 << LayerMask.NameToLayer("Planet") | 1 << LayerMask.NameToLayer("StageObject");

        while(doingDash && timer < dashTime)
        {
            timer += Time.deltaTime;
            
            //�༺ �� �� �ε�ħ ����
            Collider2D hit = Physics2D.OverlapCircle(transform.position, dashRadius,targetLayer);
            if(hit != null)
            {
                if(hit.TryGetComponent<ITarget>(out ITarget target))
                {

                    float force;
                    ParticleSystem kickParticle;

                    //�� ������ & �˹�
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

                    //��� �ʱ�ȭ
                    ResetDash();
                }
                
                
                break;
            }
            yield return null;

            if (!attackActivate) yield break;
        }

        //��� ����
        ResetDash();


    }
    #endregion

    /// <summary>
    /// PlayerBehavior���� OnAir���¿� Slide��ư�� ������ �ν�Ʈ. ��ư�� ������ �� Boost ���ɿ��� üũ�ϰ� �����ϸ� FixedUpdate���� ������Ʈ. 
    /// �Ұ����ϸ� �������� �ʴ´�. 
    /// </summary>
    /// <returns></returns>
    #region Boost
    public bool Boost()
    {
        if(boostCurGauge <= 0)
        {
            return false;
        }

        //�ν�Ʈ �ּ�, �ִ� ��
        float min = 1.0f;
        float max = 3.0f;
        //���� �ӵ��� aimDirection�� ���� ���̳����� �ν��� �Ŀ��� Ŀ��. �׷��� ������ ������.   
        float angle = GetAngle(rb.velocity, jumpVector);
        float lerp = Mathf.Lerp(min, max, Mathf.Abs(angle)/ 180f);
        //�ν�Ʈ
        rb.AddForce(jumpVector * boostForce * lerp, ForceMode2D.Force);

        
        //�ӵ� ����
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
