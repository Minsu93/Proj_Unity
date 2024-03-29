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

    //�ڿ� �Ҹ� ��û. �Ҹ��� �ڿ��� �����ϸ� false�� ����. 

    public bool PayMoney(int amount)
    {
        if (amount > alienResource) return false;

        alienResource -= amount;
        return true; 
    }
}
