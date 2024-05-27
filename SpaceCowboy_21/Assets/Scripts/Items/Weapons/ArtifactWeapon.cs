using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArtifactWeapon : Artifact
{
    PlayerWeapon playerWeapon;

    //���� ���� �� �̺�Ʈ ���
    //public virtual void CreateArtifactWeapon(PlayerWeapon weapon)
    //{
    //    playerWeapon = weapon;
    //    playerWeapon.weaponShoot += CastWhenShoot;
    //    playerWeapon.weaponImpact += CastWhenImpact;
    //}

    //���� ���� �� �̺�Ʈ ����
    public virtual void RemoveArtifactWeapon()
    {
        //if(playerWeapon != null)
        //{
        //    playerWeapon.weaponShoot -= CastWhenShoot;
        //    playerWeapon.weaponImpact -= CastWhenImpact;
        //}
    }

    //���� ���� �� ȿ��
    public abstract void CastWhenShoot();


    //�Ѿ��� �¾��� �� ȿ��
    public abstract void CastWhenImpact(Vector2 pos);



}
