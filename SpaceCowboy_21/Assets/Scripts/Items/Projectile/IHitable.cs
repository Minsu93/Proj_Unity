using UnityEngine;
public interface IHitable
{
    // ������, HitPoint �� ���� ���.(�������� ���� �Ѿ��� ���)
    public void DamageEvent(float damage, Vector2 hitPoint);

    // hitPos �� ���� ���, forceAmount �� �˹��� ����
    public void KnockBackEvent(Vector2 hitPos, float forceAmount);
}
