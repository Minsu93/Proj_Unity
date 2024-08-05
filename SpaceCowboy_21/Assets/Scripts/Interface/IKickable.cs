using UnityEngine;
/// <summary>
/// Kick으로 때렸을 때 반응하는 오브젝트에 달아준다. 
/// </summary>
public interface IKickable 
{

    void Kicked(Vector2 hitPos);
}
