using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArtifactWeapon : Artifact
{
    PlayerWeapon playerWeapon;

    //유물 생성 시 이벤트 등록
    //public virtual void CreateArtifactWeapon(PlayerWeapon weapon)
    //{
    //    playerWeapon = weapon;
    //    playerWeapon.weaponShoot += CastWhenShoot;
    //    playerWeapon.weaponImpact += CastWhenImpact;
    //}

    //유물 제거 시 이벤트 제거
    public virtual void RemoveArtifactWeapon()
    {
        //if(playerWeapon != null)
        //{
        //    playerWeapon.weaponShoot -= CastWhenShoot;
        //    playerWeapon.weaponImpact -= CastWhenImpact;
        //}
    }

    //총을 쐈을 때 효과
    public abstract void CastWhenShoot();


    //총알을 맞았을 때 효과
    public abstract void CastWhenImpact(Vector2 pos);



}
