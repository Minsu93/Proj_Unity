using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBuffs : MonoBehaviour
{
    //총기에 적용될 버프 스텟
    public WeaponStats weaponBuffStats { get; private set; }

    //움직임에 적용될 버프 스텟

    //점프에 적용될 스텟

    private void Awake()
    {
        weaponBuffStats = new WeaponStats();
    }


    void BuffStart()
    {
        //버프 추가
    }
    void BuffOver()
    {
        //버프 제거 - 시간이 지나거나 혹은 다른 버프가 들어온 경우
    }

    void ResetBuff()
    {
        //무기 버프 초기화
        GameManager.Instance.playerManager.playerWeapon.ResetBuff();
        //움직임 버프 초기화

        //점프 버프 초기화
    }

}
