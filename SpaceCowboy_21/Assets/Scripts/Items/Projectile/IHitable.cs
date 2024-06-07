using UnityEngine;
public interface IHitable
{
    public void DamageEvent(float damage, Vector2 hitPoint);

    public void KnockBackEvent(Vector2 hitPos, float forceAmount);
}
