using UnityEngine;

/// <summary>
/// 뭔가에 부딪힐 때 그곳의 콜라이더를 불러오기 위해서. 
/// </summary>
public interface ITarget 
{
    Collider2D GetCollider();
}
