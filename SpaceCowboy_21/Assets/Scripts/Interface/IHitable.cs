using UnityEngine;

/// <summary>
/// �÷��̾� �Ѿ˷� ���� �� �ִ� ���� �����Ѵ�.
/// </summary>
public interface IHitable
{
    // ������, HitPoint �� ���� ���.(�������� ���� �Ѿ��� ���)
    void DamageEvent(float damage, Vector2 hitPoint);

}
