using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestCube", menuName = "Weapon/TechCube/OnImpact", order = int.MaxValue)]
public class OnImpactCube : AlienTechCube
{
    //큐브는AlienTechCube를 상속받고,
    //AssetMenu(생성)
    //ID, 능력치
    //타격,이동,충격 시 이벤트만 신경쓰면 된다. 

    //구현하지 않은 이벤트는 추가되지 않는다. 

    //타격 시
    public override void OnImpactEvent()
    {
        Debug.Log("C");
    }
}
