using UnityEngine;

public interface IEnemyHitable
{
    ///�� ������ ���ظ� �� �� �ִ� ������Ʈ
    ///

    void DamageEvent(float damage, Vector2 hitPoint);

    void KnockBackEvent(Vector2 hitPos, float forceAmount);
}
