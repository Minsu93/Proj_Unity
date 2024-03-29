using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;
    public int alienResource { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void AddMoney(int amount)
    {
        alienResource += amount;
    }

    //자원 소모를 요청. 소모할 자원이 부족하면 false를 리턴. 

    public bool PayMoney(int amount)
    {
        if (amount > alienResource) return false;

        alienResource -= amount;
        return true; 
    }
}
