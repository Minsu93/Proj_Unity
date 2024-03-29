using SpaceCowboy;
using SpaceEnemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarMagicGun : MonoBehaviour
{
    public int maxAmmo = 6;
    int currAmmo;

    public float shootInterval;
    float lastShootTime;

    public ParticleSystem magicBullet;
    

    PlayerBehavior playerBehavior;

    private void Awake()
    {
        playerBehavior = GetComponent<PlayerBehavior>();
        currAmmo = maxAmmo;
    }

    public void MagicShoot()
    {
        if (!playerBehavior.activate) return;
        if (currAmmo < 1)
        {
            return;
        }

        //�� �߻� �ֱ�
        if (Time.time - lastShootTime < shootInterval)
        {
            return;
        }

        Shoot();
    }

    void Shoot()  //�̺�Ʈ �߻��� ���ؼ� > PlayerWeapon, PlayerView �� ShootEvent�� �߻�
    {
        currAmmo -= 1;
        lastShootTime = Time.time;

        //Vector3 dir = gunTip.right; //�߻� ����
        Vector3 gunTipPos = playerBehavior.gunTipPos;
        Vector3 dir = playerBehavior.aimDirection;

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90f) * dir;       // ù �߻� ������ ���Ѵ�. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //���ʹϾ� ������ ��ȯ        

        //�Ѿ� ����
        //GameObject projectile = PoolManager.instance.Get(projectilePrefab);
        //projectile.transform.position = gunTipPos;
        //projectile.transform.rotation = targetRotation;
        //projectile.GetComponent<Projectile_Player>().init(damage, speed, range, lifeTime);

        ParticleHolder.instance.GetParticle(magicBullet, gunTipPos, targetRotation);

        RaycastHit2D hit = Physics2D.Raycast(gunTipPos, dir, 10f, LayerMask.GetMask("Enemy"));
        if(hit.collider != null)
        {

        }

        //AudioManager.instance.PlaySfx(shootSFX);

        //PlayerView �� �ִϸ��̼� ���� 
        playerBehavior.TryShootEvent();

    }


}
