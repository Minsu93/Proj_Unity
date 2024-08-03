public interface ISwitchable
{
    //Generator로 인해서 켜질 때 작동하는 부분
    public void ActivateObject();

    //Generator로 인해서 꺼질 때 작동하는 부분
    public void DeactivateObject();
}