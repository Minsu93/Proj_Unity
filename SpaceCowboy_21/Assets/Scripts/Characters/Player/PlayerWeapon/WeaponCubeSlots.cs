 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// OnShootEvent가 null이 아니고 실제로 구현되었는지 확인하는 이벤트 있음.
/// </summary>
/// 

public class WeaponCubeSlots : MonoBehaviour
{
    //public string[] combinationsA = new string[2];
    //public string[] combinationsB = new string[2];
    //public string[] combinationsC = new string[2];

    //큐브 장착


    //큐브의 스텟 적용
    //public WeaponStats UpdateStats(WeaponStats baseStats)
    //{
    //    WeaponStats stats = baseStats;
    //    WeaponStats totalBonus = new WeaponStats();

    //    foreach(var cube in cubeList)
    //    {
    //        WeaponStats bonus = cube.BonusStats;
    //        //큐브의 스텟 적용 
    //        totalBonus.damage += bonus.damage;
    //        totalBonus.speed += bonus.speed;
    //        totalBonus.shootInterval += bonus.shootInterval;


    //        //이벤트 적용
    //        // OnShootEvent가 null이 아니고 실제로 구현되었는지 확인
    //        if (cube.GetType().GetMethod("OnShootEvent").DeclaringType != typeof(AlienTechCube))
    //        {
    //            stats.weaponShoot += cube.OnShootEvent;
    //        }
    //        // OnImpactEvent가 null이 아니고 실제로 구현되었는지 확인
    //        if (cube.GetType().GetMethod("OnImpactEvent").DeclaringType != typeof(AlienTechCube))
    //        {
    //            stats.weaponImpact += cube.OnImpactEvent;
    //        }
    //    }

    //    stats.damage *= (100 + totalBonus.damage) / 100;
    //    stats.speed *= (100 + totalBonus.speed) / 100;
    //    stats.shootInterval *= 100 / (100 + totalBonus.shootInterval);




    //    return stats;
    //}

    ////특수 조합 시 특수효과 발동. 특수효과는 WeaponType에 명시되어있다. 
    //public int CheckSpecialCombinations()
    //{
    //    List<string> IDs = new List<string>();
    //    foreach(var cube in cubeList)
    //    {
    //        IDs.Add(cube.CubeID);
    //    }

    //    int index = 0;
    //    if (IDs.Contains(combinationsA[0]) && IDs.Contains(combinationsA[1]))
    //    {
    //        //조합 1
    //        index = 1;
    //    }
    //    else if(IDs.Contains(combinationsB[0]) && IDs.Contains(combinationsB[1]))
    //    {
    //        //조합 2
    //        index = 2;
    //    }
    //    else if (IDs.Contains(combinationsC[0]) && IDs.Contains(combinationsC[1]))
    //    {
    //        //조합 3
    //        index = 3;
    //    }

    //    return index;
    //}
}
