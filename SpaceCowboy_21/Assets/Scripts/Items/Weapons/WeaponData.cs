
using System;
using UnityEngine;
using static UnityEditor.Progress;


[CreateAssetMenu(fileName = "Weapon Data", menuName ="Data/WeaponData", order = int.MaxValue)]
public class WeaponData : ScriptableObject
{

    [Header("Weapon Property")]
    ///WeaponData 의 ID와, Data폴더의 itemData 아이디가 동일해야한다.
    //[SerializeField]
    //int itemID;
    //public int ItemID { get { return itemID; } }

    [SerializeField]
    string itemName;
    public string ItemName { get { return itemName; } }

    [SerializeField]
    GunStyle gunStyle;
    public GunStyle GunStyle { get {  return gunStyle; } }

    [SerializeField]
    Sprite icon;    // 집었을 때 화면 하단에 들어갈 퀵 슬롯 이미지
    public Sprite Icon { get { return icon; } }
    
    [SerializeField]
    Sprite bubbleIcon;    // Bubble_Weapon에 들어갈 이미지
    public Sprite BubbleIcon { get { return bubbleIcon; } }


    [Header("Projectile Property")]
    [SerializeField]
    ProjectileData[] projDatas = new ProjectileData[3];
    public ProjectileData[] ProjectDatas { get { return projDatas; } }

    [SerializeField]
    bool showRange;
    public bool ShowRange { get { return showRange; } }

    [SerializeField]
    int maxAmmo;     //탄창의 개수, 0이면 무한.
    public int MaxAmmo { get { return maxAmmo; } }

    [SerializeField]
    AudioClip shootSFX;   //총기 사운드
    public AudioClip ShootSFX { get { return shootSFX; } }

    [SerializeField]
    GameObject weaponPrefab;
    public GameObject WeaponPrefab { get { return weaponPrefab; } }

}

[Serializable]
public struct ProjectileData
{
    public int shapeID;
    public GameObject projectilePrefab;
    public GameObject secondProjectilePrefab; 
    public float damage;
    public float shootInterval;
    public float burstInterval; 
    public int numberOfProjectile; 
    public int numberOfBurst; 
    public float speed;     
    public float speedVariation;       
    public float lifeTime;      
    public float range;    
    public float projectileSpread; 
    public float randomSpreadAngle;

    public ProjectileData (int shapeID, GameObject projectilePrefab, GameObject secondProjectilePrefab, float damage, float shootInterval, float burstInterval, 
        int numberOfProjectile, int numberOfBurst, float speed, float speedVariation, float lifeTime, float range, float projectileSpread, float randomSpreadAngle)
    {
        this.shapeID = shapeID;
        this.projectilePrefab = projectilePrefab;
        this.secondProjectilePrefab = secondProjectilePrefab;
        this.damage = damage;
        this.shootInterval = shootInterval;
        this.burstInterval = burstInterval;
        this.numberOfProjectile = numberOfProjectile;
        this.numberOfBurst = numberOfBurst;
        this.speed = speed;
        this.speedVariation = speedVariation;
        this.lifeTime = lifeTime;
        this.range = range;
        this.projectileSpread = projectileSpread;
        this.randomSpreadAngle = randomSpreadAngle;

    }
}

