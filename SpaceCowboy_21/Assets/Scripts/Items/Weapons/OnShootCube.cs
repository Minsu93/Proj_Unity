using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestCube", menuName = "Weapon/TechCube/OnShoot", order = int.MaxValue)]

public class OnShootCube : AlienTechCube
{
    public override void OnShootEvent()
    {
        Debug.Log("OnShootEvent");
    }
}
