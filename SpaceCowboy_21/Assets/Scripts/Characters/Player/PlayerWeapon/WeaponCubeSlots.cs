 using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponCubeSlots : MonoBehaviour
{
    public string[] combinationsA = new string[2];
    public string[] combinationsB = new string[2];
    public string[] combinationsC = new string[2];

    //ť�� ����


    //ť���� ���� ����
    //public WeaponStats UpdateStats(WeaponStats baseStats)
    //{
    //    WeaponStats stats = baseStats;
    //    WeaponStats totalBonus = new WeaponStats();

    //    foreach(var cube in cubeList)
    //    {
    //        WeaponStats bonus = cube.BonusStats;
    //        //ť���� ���� ���� 
    //        totalBonus.damage += bonus.damage;
    //        totalBonus.speed += bonus.speed;
    //        totalBonus.shootInterval += bonus.shootInterval;


    //        //�̺�Ʈ ����
    //        // OnShootEvent�� null�� �ƴϰ� ������ �����Ǿ����� Ȯ��
    //        if (cube.GetType().GetMethod("OnShootEvent").DeclaringType != typeof(AlienTechCube))
    //        {
    //            stats.weaponShoot += cube.OnShootEvent;
    //        }
    //        // OnImpactEvent�� null�� �ƴϰ� ������ �����Ǿ����� Ȯ��
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

    ////Ư�� ���� �� Ư��ȿ�� �ߵ�. Ư��ȿ���� WeaponType�� ��õǾ��ִ�. 
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
    //        //���� 1
    //        index = 1;
    //    }
    //    else if(IDs.Contains(combinationsB[0]) && IDs.Contains(combinationsB[1]))
    //    {
    //        //���� 2
    //        index = 2;
    //    }
    //    else if (IDs.Contains(combinationsC[0]) && IDs.Contains(combinationsC[1]))
    //    {
    //        //���� 3
    //        index = 3;
    //    }

    //    return index;
    //}
}
