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
    [SerializeField] float dashTime = 0.5f; //��� ���� �� ���� �ð� (��� �۵� �ð�)
    [SerializeField] float dashDamage = 10f;    //������
    [SerializeField] float dashKnockBackPower = 5f; //�˹� �Ŀ�
    [SerializeField] float unHittableTime = 1f;//��� ���� ���� �� �����ð�


    public bool doingDash;    //��������
    int dashCount = 0;
    Vector2 jumpVector;     //���� ���� ���⺤��
    //ctor2 preVelocity;    //KnockBack �ӵ� ���뵵.

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

    //������ ����
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
                if (hit.TryGetComponent<ITarget>(out ITarget target))
                {
                    float force = playerBehavior.knockBackForce;
                    //ĳ���� �˹�
                    //playerBehavior.KnockBackEvent(hit.transform.position, playerBehavior.knockBackForce);

                    //�� ������ & �˹�
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
                //��� �ʱ�ȭ
                ResetDash();
                
                break;
            }
            yield return null;

            if (!attackActivate) yield break;
        }

        //��� ����
        ResetDash();


    }
}
