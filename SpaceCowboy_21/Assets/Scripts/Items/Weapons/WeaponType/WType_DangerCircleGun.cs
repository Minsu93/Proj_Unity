using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WType_DangerCircleGun : WeaponType
{
    /// <summary>
    /// ���� ���� �Ÿ��� DangerCircle�� ���ܳ���.
    /// ��ư�� ������ �ش� Circle������ ���鿡�� ���ظ� �ش�. 
    /// </summary>
    /// 

    [SerializeField] float dangerCircleRadius = 1.0f;
    [SerializeField] float dangerCircleDistance = 5.0f;
    [SerializeField] Sprite circleSprite;
    [SerializeField] Material circleMat;
    GameObject dangerCircleObj;

    public override void Initialize(WeaponData weaponData, Vector3 gunTipLocalPos)
    {
        base.Initialize(weaponData, gunTipLocalPos);

        dangerCircleObj = new GameObject();

        SpriteRenderer circleSpr = dangerCircleObj.AddComponent<SpriteRenderer>();
        circleSpr.sprite = circleSprite;
        circleSpr.material = circleMat;
        circleSpr.sortingLayerName = "Effect";

        dangerCircleObj.transform.parent = this.transform;
        dangerCircleObj.transform.localPosition = gunTipLocalPos + (Vector3.right * dangerCircleDistance);
        dangerCircleObj.transform.localRotation = Quaternion.identity;
    }

    public override void ShootButtonDown(Vector2 pos, Vector3 dir)
    {
        throw new System.NotImplementedException();
    }

    public override void ShootButtonUp(Vector2 pos, Vector3 dir)
    {
        throw new System.NotImplementedException();
    }
}
