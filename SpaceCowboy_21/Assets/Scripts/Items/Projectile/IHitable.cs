using UnityEngine;
public interface IHitable
{
    // 데미지, HitPoint 는 맞은 장소.(데미지를 입힌 총알의 장소)
    public void DamageEvent(float damage, Vector2 hitPoint);

    // hitPos 는 맞은 장소, forceAmount 는 넉백의 강도
    public void KnockBackEvent(Vector2 hitPos, float forceAmount);
}
