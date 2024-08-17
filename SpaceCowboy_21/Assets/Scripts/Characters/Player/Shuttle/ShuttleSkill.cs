using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ʋ ��ų�� �θ� 
/// </summary>
public class ShuttleSkill : MonoBehaviour, ITarget, IHitable, IKickable
{
    public float skillCoolTime; // ��Ʋ ��ų ���� ��� �ð�. ������ ����� ���� ���ĺ��� ���� ��� ������ ���� �����Ѵ�. 
    [SerializeField] CircleCollider2D coll;

    //�ʱ�ȭ �� 
    void ShuttleInitialize()
    {
        coll.enabled = false;
    }


    //��ų ���� 
    public void PressSkillButton(Rigidbody2D rb ,float maxDistance, float minSmoothTime, float maxSmoothTime)
    {
        //���콺 ��ġ�� �����´�. 
        Vector2 mousePos = GameManager.Instance.playerManager.playerBehavior.mousePos;
        //��ġ�� �̵���Ų��. 
        MoveToTargetPosition(rb, mousePos, maxDistance, minSmoothTime,maxSmoothTime);
    }

    Vector2 velocity = Vector2.zero;

    //Ư�� ��ġ�� �̵�
    void MoveToTargetPosition(Rigidbody2D rb,Vector2 targetPos, float maxDistance, float minSmoothTime, float maxSmoothTime)
    {
        Vector2 targetPosition = targetPos;

        // ���� ��ġ�� ��ǥ ��ġ ���� �Ÿ�
        float distance = Vector2.Distance(transform.position, targetPosition) / maxDistance;

        // �Ÿ� ������� smoothTime ����
        float smoothTime = Mathf.Lerp(minSmoothTime, maxSmoothTime, distance);

        // �ε巴�� �̵� (SmoothDamp)
        rb.MovePosition(Vector2.SmoothDamp(rb.position, targetPosition, ref velocity, smoothTime));
    }



    //����
    void TransformMethod()
    {
        coll.enabled = true;
    }

    //�÷��̾� �ѿ� �¾��� ��
    void HitByPlayerProj()
    {

    }

    //�÷��̾� �����⿡ �¾��� �� 
    void HitByPlayerKick()
    {

    }

    //���ð� ī��Ʈ
    void CountWaitTimer()
    {

    }

    //���ð� ī��Ʈ
    void CountUseTimer()
    {

    }

    //���� ����
    void CompleteMethod()
    {

    }

    public Collider2D GetCollider()
    {
        throw new System.NotImplementedException();
    }

    public void DamageEvent(float damage, Vector2 hitPoint)
    {
        throw new System.NotImplementedException();
    }

    public void Kicked(Vector2 hitPos)
    {
        throw new System.NotImplementedException();
    }
}
