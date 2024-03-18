
using SpaceCowboy;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon Data", menuName ="Weapon/Weapon Data", order = int.MaxValue)]
public class WeaponData : ScriptableObject
{

    [Header("Weapon Property")]
    //[SerializeField]
    //int skinIndex; //어떤 스킨을 사용하느냐
    //public int SkinIndex { get { return skinIndex; } }

    [SerializeField]
    GunType gunType;    //외형을 결정
    public GunType GunType { get {  return gunType; } }

    [SerializeField]
    float chargeGauge; //발사하기 위해 필요한 에너지 량
    public float ChargeGauge { get { return chargeGauge; } }

    //[SerializeField]
    //bool oneHand ; //한손 총인가요?
    //public bool OneHand { get { return oneHand; } }

    //[SerializeField]
    //bool singleShot; //단발 총인가요?   >> 충전형 무기로 설정
    //public bool SingleShot { get { return singleShot; } }

    [SerializeField]
    int burstNumber; //한번 누를 때 연속 발사 수
    public int BurstNumer { get { return burstNumber; } }

    [SerializeField]
    float burstInterval;    //연속 발사 시 사이 시간
    public float BurstInterval { get { return burstInterval; } }

    [SerializeField]
    int numberOfProjectiles;    //한번 누를 때 멀티샷 수
    public int NumberOfProjectiles { get { return numberOfProjectiles; } }

    [SerializeField]
    float shootInterval;    //발사 쿨타임
    public float ShootInterval { get { return shootInterval; } }

    [SerializeField]
    float projectileSpread; //총알 마다 떨어진 각도
    public float ProjectileSpread { get { return projectileSpread; } }

    [SerializeField]
    float randomSpreadAngle;    //총구 흔들림때문에 생기는 랜덤값
    public float RandomSpreadAngle { get { return randomSpreadAngle; } }

    //[SerializeField]
    //float recoil;   //총기 반동
    //public float Recoil { get { return recoil; } }

    [SerializeField]
    AudioClip shootSFX;   //총기 사운드
    public AudioClip ShootSFX { get { return shootSFX; } }



    //[Header ("Power Property")]

    //[SerializeField]
    //float artifactLifetime;   //총기 수명
    //public float ArtifaceLifetime { get { return artifactLifetime; } }

    //[SerializeField]
    //float powerCost;   //사용하는 파워 게이지
    //public float PowerCost { get { return powerCost; } }

    //[SerializeField]
    //bool isAutoShoot;   //자동 발사 무기인가요?
    //public bool IsAutoShoot { get { return isAutoShoot; } }


    [Header("Projectile Property")]

    [SerializeField]
    GameObject projectilePrefab;    //총알의 종류
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }

    [SerializeField]
    float damage;  // 데미지
    public float Damage { get { return damage; } }

    [SerializeField]
    int maxAmmo;     //탄창의 개수
    public int MaxAmmo { get { return maxAmmo; } }

    //[SerializeField]
    //float reloadTime;  // 재장전에 필요한 시간
    //public float ReloadTime { get { return reloadTime; } }

    [SerializeField]
    float speed;        //총알의 속도
    public float Speed { get { return speed; } }

    [SerializeField]
    float range;        //총알의 최대 사거리
    public float Range { get { return range; } }


    //[SerializeField]
    //float knockBackForce;   //넉백 힘
    //public float KnockBackForce { get { return knockBackForce; } }


    [SerializeField]
    float lifeTime;     //생명주기
    public float LifeTime { get { return lifeTime; } }


    [SerializeField]
    int reflectionCount;   //튕기는 횟수
    public int ReflectionCount { get { return reflectionCount; } }

}
