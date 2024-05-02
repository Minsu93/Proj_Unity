using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_PowerShot : SkillArtifact
{
    //�����ϴ� ������Ʈ 
    [SerializeField] private GameObject projPrefab;
    public float damage, speed, lifeTime;

    public override void SkillOperation(Vector3 pos, Vector2 dir)
    {
        //��ų ����
        Debug.Log("��ų �ߵ�!");


        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //�Ѿ� ����
        GameObject projectile = PoolManager.instance.Get(projPrefab);
        projectile.transform.position = pos;
        projectile.transform.rotation = targetRotation;
        projectile.GetComponent<Projectile>().Init(damage, speed, lifeTime);


        //AudioManager.instance.PlaySfx(shootSFX);
    }
}
