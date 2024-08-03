using SpaceCowboy;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Spine.Unity.Editor.SkeletonBaker.BoneWeightContainer;

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
    [SerializeField] float dashDamage = 10f;    //데미지
    [SerializeField] float dashKnockBackPower = 5f; //넉백 파워
    [SerializeField] float unHittableTime = 1f;//대시 공격 성공 후 무적시간


    public bool doingDash;    //슈퍼점프
    int dashCount = 0;
    Vector2 jumpVector;     //실제 점프 방향벡터
    //ctor2 preVelocity;    //KnockBack 속도 계산용도.

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

    }
    private void Start()
    {
        GameManager.Instance.PlayerTeleportStart += ClearTrail;
        GameManager.Instance.PlayerTeleportEnd += ShowTrail;
        playerBehavior.PlayerHitEvent += ResetDash;
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

    public void RemoveJumpArrow()
    {
        jumpArrowViewer.gameObject.SetActive(false);
    }

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

    //공격형 점프
    public void Dash()
    {
        if (dashCount < dashMaxCount) dashCount++;
        else return;

        doingDash = true;
        StartCoroutine(DashAttackRoutine());

        rb.velocity = Vector2.zero;
        rb.AddForce(jumpVector * dashForce, ForceMode2D.Impulse);

        sJumpTrail.emitting = true;
        sJumpTrail.Clear();

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
                if (hit.TryGetComponent<ITarget>(out ITarget target))
                {
                    float force = playerBehavior.knockBackForce;
                    //캐릭터 넉백
                    //playerBehavior.KnockBackEvent(hit.transform.position, playerBehavior.knockBackForce);

                    //적 데미지 & 넉백
                    if(hit.TryGetComponent<IHitable>(out IHitable hitable))
                    {
                        attackActivate = false;
                        hitable.DamageEvent(dashDamage, transform.position);
                        hitable.KnockBackEvent(transform.position, dashKnockBackPower);

                        playerBehavior.KnockBackEvent(hit.transform.position, force * 2f);

                        if (impactParticle != null)
                            GameManager.Instance.particleManager.GetParticle(impactParticle, transform.position, transform.rotation);

                        playerBehavior.PlayerUnhittable(unHittableTime);
                    }
                    else
                    {
                        playerBehavior.KnockBackEvent(hit.transform.position, force);

                        if (noImpactParticle != null)
                            GameManager.Instance.particleManager.GetParticle(noImpactParticle, transform.position, transform.rotation);
                    }
                }
                //대시 초기화
                ResetDash();
                
                break;
            }
            yield return null;

            if (!attackActivate) yield break;
        }

        //대시 종료
        ResetDash();


    }
}
