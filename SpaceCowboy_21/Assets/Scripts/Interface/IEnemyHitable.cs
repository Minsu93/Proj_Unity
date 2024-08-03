using UnityEngine;

public interface IEnemyHitable
{
    ///적 유닛이 피해를 줄 수 있는 오브젝트
    ///

    void DamageEvent(float damage, Vector2 hitPoint);

    void KnockBackEvent(Vector2 hitPos, float forceAmount);
}
