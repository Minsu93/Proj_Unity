
using SpaceCowboy;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon Data", menuName ="Weapon/Weapon Data", order = int.MaxValue)]
public class WeaponData : ScriptableObject
{

    [Header("Weapon Property")]
    ///WeaponData 의 ID와, Data폴더의 itemData 아이디가 동일해야한다.
    [SerializeField]
    int itemID;
    public int ItemID { get { return itemID; } }

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
    GameObject projectilePrefab;    //총알의 종류
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }

    [SerializeField]
    float damage;  // 데미지
    public float Damage { get { return damage; } }

    [SerializeField]
    float shootInterval;    //발사 간격
    public float ShootInterval { get { return shootInterval; } }

    [SerializeField]
    float burstInterval;    //발사 간격
    public float BurstInterval { get { return burstInterval; } }

    [SerializeField]
    int numberOfProjectile; //발사당 총알 수 
    public int NumberOfProjectile { get { return numberOfProjectile; } }

    [SerializeField]
    int numberOfBurst; //버스트 횟수
    public int NumberOfBurst { get { return numberOfBurst; } }

    [SerializeField]
    float speed;        //총알의 속도
    public float Speed { get { return speed; } }

    [SerializeField]
    float lifeTime;        //총알의 지속시간
    public float LifeTime { get { return lifeTime; } }

    [SerializeField]
    float range;     //총알의 사정거리
    public float Range { get { return range; } }

    [SerializeField]
    bool showRange;
    public bool ShowRange { get { return showRange; } }

    [SerializeField]
    int maxAmmo;     //탄창의 개수, 0이면 무한.
    public int MaxAmmo { get { return maxAmmo; } }

    //[SerializeField]
    //int projPenetration; //총알 관통력
    //public int ProjPenetration { get {  return projPenetration; } }

    //[SerializeField]
    //int projReflection;  //총알 반사 횟수
    //public int ProjReflection { get { return projReflection; } }

    //[SerializeField]
    //int projGuide;   //유도력
    //public int ProjGuide { get { return projGuide; } }  

    [SerializeField]
    float projectileSpread; //총알 마다 떨어진 각도
    public float ProjectileSpread { get { return projectileSpread; } }

    [SerializeField]
    float randomSpreadAngle;    //총구 흔들림때문에 생기는 랜덤값
    public float RandomSpreadAngle { get { return randomSpreadAngle; } }

    [SerializeField]
    AudioClip shootSFX;   //총기 사운드
    public AudioClip ShootSFX { get { return shootSFX; } }

    //[SerializeField]
    //float recoil;
    //public float Recoil { get { return recoil; } }

    [SerializeField]
    GameObject weaponPrefab;
    public GameObject WeaponPrefab { get { return weaponPrefab; } }

    //[SerializeField]
    //GameObject ammoPrefab;
    //public GameObject AmmoPrefab { get { return ammoPrefab; } }

    [Header("Not use")]
    [SerializeField]
    Sprite weaponEnableSprite;
    public Sprite WeaponEnableSprite { get { return weaponEnableSprite; } }

    [SerializeField]
    Sprite weaponDisableSprite;
    public Sprite WeaponDisableSprite { get { return weaponDisableSprite; } }
}

