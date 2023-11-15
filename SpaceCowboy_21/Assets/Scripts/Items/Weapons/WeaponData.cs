
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon Data", menuName ="Weapon/Weapon Data", order = int.MaxValue)]
public class WeaponData : ScriptableObject
{

    [Header("Weapon Property")]
    [SerializeField]
    int skinIndex; //� ��Ų�� ����ϴ���
    public int SkinIndex { get { return skinIndex; } }


    [SerializeField]
    bool oneHand ; //�Ѽ� ���ΰ���?
    public bool OneHand { get { return oneHand; } }

    [SerializeField]
    bool singleShot; //�ܹ� ���ΰ���?
    public bool SingleShot { get { return singleShot; } }

    [SerializeField]
    int burstNumber; //�ѹ� ���� �� ���� �߻� ��
    public int BurstNumer { get { return burstNumber; } }

    [SerializeField]
    float burstInterval;    //���� �߻� �� ���� �ð�
    public float BurstInterval { get { return burstInterval; } }

    [SerializeField]
    int numberOfProjectiles;    //�ѹ� ���� �� ��Ƽ�� ��
    public int NumberOfProjectiles { get { return numberOfProjectiles; } }

    [SerializeField]
    float shootInterval;    //�߻� ��Ÿ��
    public float ShootInterval { get { return shootInterval; } }

    [SerializeField]
    float projectileSpread; //�Ѿ� ���� ������ ����
    public float ProjectileSpread { get { return projectileSpread; } }

    [SerializeField]
    float randomSpreadAngle;    //�ѱ� ��鸲������ ����� ������
    public float RandomSpreadAngle { get { return randomSpreadAngle; } }

    [SerializeField]
    float recoil;   //�ѱ� �ݵ�
    public float Recoil { get { return recoil; } }

    [SerializeField]
    AudioClip shootSFX;   //�ѱ� ����
    public AudioClip ShootSFX { get { return shootSFX; } }



    [Header ("Power Property")]

    [SerializeField]
    float artifactLifetime;   //�ѱ� ����
    public float ArtifaceLifetime { get { return artifactLifetime; } }


    [SerializeField]
    float powerCost;   //����ϴ� �Ŀ� ������
    public float PowerCost { get { return powerCost; } }

    [SerializeField]
    bool isAutoShoot;   //�ڵ� �߻� �����ΰ���?
    public bool IsAutoShoot { get { return isAutoShoot; } }


    [Header("Projectile Property")]

    [SerializeField]
    GameObject projectilePrefab;    //�Ѿ��� ����
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }

    [SerializeField]
    int maxAmmo;     //źâ�� ����
    public int MaxAmmo { get { return maxAmmo; } }

    [SerializeField]
    float reloadTime;  // ������
    public float ReloadTime { get { return reloadTime; } }

    [SerializeField]
    float damage;  // ������
    public float Damage { get { return damage; } }

    [SerializeField]
    float lifeTime;     //�����ֱ�
    public float LifeTime { get { return lifeTime; } }

    [SerializeField]
    float speed;        //����ü �ӵ�
    public float Speed { get { return speed; } }

    [SerializeField]
    float knockBackForce;   //�˹� ��
    public float KnockBackForce { get { return knockBackForce; } }

    [SerializeField]
    int reflectionCount;   //ƨ��� Ƚ��
    public int ReflectionCount { get { return reflectionCount; } }

}
