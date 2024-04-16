using UnityEngine;
public interface IHitable
{
    public void DamageEvent(float damage, Vector2 hitPoint);
}
