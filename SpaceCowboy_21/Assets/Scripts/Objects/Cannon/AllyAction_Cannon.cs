using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class AllyAction_Cannon : AllyAction
{
    [SerializeField] float cannonDamage;
    [SerializeField] float cannonSpeed;
    [SerializeField] float cannonLifeTime;
    [SerializeField] int cannonPenetrateCount;
    [SerializeField] int cannonReflectCount;
    [SerializeField] int cannonGuideAmount;


    public override void Attack(Vector3 pos, Quaternion rot)
    {
        _onAttackCool = true;   //ÄðÅ¸ÀÓ ½ÃÀÛ

        ////·£´ý °¢µµ Ãß°¡
        //float randomAngle = Random.Range(-projectileStructs[projIndex].spreadAngle * 0.5f, projectileStructs[projIndex].spreadAngle * 0.5f);
        //Quaternion randomRotation = Quaternion.Euler(0, 0, randomAngle);

        //ÃÑ¾Ë »ý¼º
        GameObject projectile = GameManager.Instance.poolManager.Get(projectilePrefab);
        projectile.transform.position = pos;
        projectile.transform.rotation = rot;
        //projectile.GetComponent<Projectile_Player>().Init(cannonDamage, cannonSpeed, cannonLifeTime,cannonPenetrateCount, cannonReflectCount,cannonGuideAmount);

        //AudioManager.instance.PlaySfx(shootSFX);
    }
}
