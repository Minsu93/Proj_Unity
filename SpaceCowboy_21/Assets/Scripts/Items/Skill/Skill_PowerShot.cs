using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_PowerShot : SkillArtifact
{
    //생성하는 오브젝트 
    [SerializeField] private GameObject projPrefab;
    public float damage, speed, lifeTime;

    public override void SkillOperation(Vector3 pos, Vector2 dir)
    {
        //스킬 생성
        Debug.Log("스킬 발동!");


        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //총알 생성
        GameObject projectile = PoolManager.instance.Get(projPrefab);
        projectile.transform.position = pos;
        projectile.transform.rotation = targetRotation;
        projectile.GetComponent<Projectile>().Init(damage, speed, lifeTime);


        //AudioManager.instance.PlaySfx(shootSFX);
    }
}
