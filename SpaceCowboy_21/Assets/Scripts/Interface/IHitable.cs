using UnityEngine;

/// <summary>
/// 플레이어 총알로 때릴 수 있는 곳에 장착한다.
/// </summary>
public interface IHitable
{
    // 데미지, HitPoint 는 맞은 장소.(데미지를 입힌 총알의 장소)
    void DamageEvent(float damage, Vector2 hitPoint);

}
