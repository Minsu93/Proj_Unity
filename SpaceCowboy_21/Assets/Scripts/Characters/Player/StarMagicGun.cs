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

        //총 발사 주기
        if (Time.time - lastShootTime < shootInterval)
        {
            return;
        }

        Shoot();
    }

    void Shoot()  //이벤트 발생을 위해서 > PlayerWeapon, PlayerView 의 ShootEvent를 발생
    {
        currAmmo -= 1;
        lastShootTime = Time.time;

        //Vector3 dir = gunTip.right; //발사 각도
        Vector3 gunTipPos = playerBehavior.gunTipPos;
        Vector3 dir = playerBehavior.aimDirection;

        Vector3 rotatedVectorToTarget = Quaternion.Euler(0, 0, 90f) * dir;       // 첫 발사 방향을 구한다. 
        Quaternion targetRotation = Quaternion.LookRotation(forward: Vector3.forward, upwards: rotatedVectorToTarget);     //쿼터니언 값으로 변환        

        //총알 생성
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

        //PlayerView 의 애니메이션 실행 
        playerBehavior.TryShootEvent();

    }


}
