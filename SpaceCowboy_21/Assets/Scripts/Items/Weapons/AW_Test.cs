using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AW_Test : ArtifactWeapon
{
    public override void CastWhenImpact(Vector2 pos)
    {
        Debug.Log("�¾Ҵ� : " + pos);
    }

    public override void CastWhenShoot()
    {
        Debug.Log("����.");
    }


}
