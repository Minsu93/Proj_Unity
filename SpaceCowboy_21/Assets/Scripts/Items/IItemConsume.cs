using UnityEngine;

public interface IItemConsume 
{
    /// <summary>
    /// 아이템을 전부 사용했을 때. 보통은 경험치 획득 용도로 사용한다.
    /// </summary>
    public void ConsumeItem();
}
