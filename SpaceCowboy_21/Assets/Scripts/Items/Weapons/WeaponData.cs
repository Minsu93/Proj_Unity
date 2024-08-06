
using SpaceCowboy;
using UnityEngine;


[CreateAssetMenu(fileName = "Weapon Data", menuName ="Weapon/Weapon Data", order = int.MaxValue)]
public class WeaponData : ScriptableObject
{

    [Header("Weapon Property")]
    ///WeaponData �� ID��, Data������ itemData ���̵� �����ؾ��Ѵ�.
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
    Sprite icon;    // ������ �� ȭ�� �ϴܿ� �� �� ���� �̹���
    public Sprite Icon { get { return icon; } }
    
    [SerializeField]
    Sprite bubbleIcon;    // Bubble_Weapon�� �� �̹���
    public Sprite BubbleIcon { get { return bubbleIcon; } }

    [Header("Projectile Property")]

    [SerializeField]
    GameObject projectilePrefab;    //�Ѿ��� ����
    public GameObject ProjectilePrefab { get { return projectilePrefab; } }

    [SerializeField]
    float damage;  // ������
    public float Damage { get { return damage; } }

    [SerializeField]
    float shootInterval;    //�߻� ����
    public float ShootInterval { get { return shootInterval; } }

    [SerializeField]
    float burstInterval;    //�߻� ����
    public float BurstInterval { get { return burstInterval; } }

    [SerializeField]
    int numberOfProjectile; //�߻�� �Ѿ� �� 
    public int NumberOfProjectile { get { return numberOfProjectile; } }

    [SerializeField]
    int numberOfBurst; //����Ʈ Ƚ��
    public int NumberOfBurst { get { return numberOfBurst; } }

    [SerializeField]
    float speed;        //�Ѿ��� �ӵ�
    public float Speed { get { return speed; } }

    [SerializeField]
    float lifeTime;        //�Ѿ��� ���ӽð�
    public float LifeTime { get { return lifeTime; } }

    [SerializeField]
    float range;     //�Ѿ��� �����Ÿ�
    public float Range { get { return range; } }

    [SerializeField]
    bool showRange;
    public bool ShowRange { get { return showRange; } }

    [SerializeField]
    int maxAmmo;     //źâ�� ����, 0�̸� ����.
    public int MaxAmmo { get { return maxAmmo; } }

    //[SerializeField]
    //int projPenetration; //�Ѿ� �����
    //public int ProjPenetration { get {  return projPenetration; } }

    //[SerializeField]
    //int projReflection;  //�Ѿ� �ݻ� Ƚ��
    //public int ProjReflection { get { return projReflection; } }

    //[SerializeField]
    //int projGuide;   //������
    //public int ProjGuide { get { return projGuide; } }  

    [SerializeField]
    float projectileSpread; //�Ѿ� ���� ������ ����
    public float ProjectileSpread { get { return projectileSpread; } }

    [SerializeField]
    float randomSpreadAngle;    //�ѱ� ��鸲������ ����� ������
    public float RandomSpreadAngle { get { return randomSpreadAngle; } }

    [SerializeField]
    AudioClip shootSFX;   //�ѱ� ����
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

