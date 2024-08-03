using UnityEngine;
public interface IHitable
{
    // ������, HitPoint �� ���� ���.(�������� ���� �Ѿ��� ���)
    void DamageEvent(float damage, Vector2 hitPoint);

    // hitPos �� ���� ���, forceAmount �� �˹��� ����
    void KnockBackEvent(Vector2 hitPos, float forceAmount);
}
