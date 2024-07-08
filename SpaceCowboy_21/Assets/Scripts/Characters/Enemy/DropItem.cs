using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    /// <summary>
    /// 모든 DropItem을 보유한 적이 죽으면 Ammo를 드롭할 확률이 있다. 
    /// Ammo는 무조건적으로 1~4 중에서 떨어진다. 
    /// 플레이어가 부족한 Ammo가 있으면 드롭되기 시작한다. 부족하지 않으면 드롭되지 않는다. 
    /// Ammo드롭의 확률은 모두 동일하다. 
    /// </summary>
    
    [SerializeField] List<DropTable> dropTable = new List<DropTable>();
    //[SerializeField] DropTable ammoDropTable = new DropTable();

    [SerializeField] float resourceLaunchPowerMin = 3f;
    [SerializeField] float resourceLaunchPowerMax = 4f;
    //[SerializeField] float ammoLaunchPowerMin = 5f;
    //[SerializeField] float ammoLaunchPowerMax = 6f;
    private void Awake()
    {
        EnemyAction enemyAction = GetComponent<EnemyAction>();
        if(enemyAction != null )
            enemyAction.EnemyDieEvent += GenerateItem;
    }

    public void GenerateItem()
    {
        // resource Generate
        for (int i = 0; i < dropTable.Count; i++)
        {
            GenerateProcess(dropTable[i].dropChance, dropTable[i].item, dropTable[i].dropMin, dropTable[i].dropMax, resourceLaunchPowerMin, resourceLaunchPowerMax);
        }

    }

    //void AmmoGenerate()
    //{
    //    //ammo Generate

    //    //1. 현재 부족한 ammo의 수 카운트. PlayerManager에서 WeaponInventory를 가져오고, activate되지 않은 무기들을 구해서 ammoList를 만든다. 
    //    WeaponInventory[] w_Inventory = GameManager.Instance.playerManager.weaponInventory;
    //    List<GameObject> ammoList = new List<GameObject>();
    //    for (int h = 0; h < w_Inventory.Length; h++)
    //    {
    //        if (w_Inventory[h].activate == false)
    //        {
    //            GameObject ammoPrefab = w_Inventory[h].weaponData.AmmoPrefab;
    //            if (ammoPrefab != null)
    //                ammoList.Add(ammoPrefab);
    //        }
    //    }

    //    //2. 생성
    //    for (int j = 0; j < ammoList.Count; j++)
    //    {
    //        GenerateProcess(ammoDropTable.dropChance, ammoList[j], ammoDropTable.dropMin, ammoDropTable.dropMax, ammoLaunchPowerMin, ammoLaunchPowerMax);
    //    }
    //}

    void GenerateProcess(float dropChance, GameObject item, int dropMin, int dropMax, float launchPowerMin, float launchPowerMax)
    {
        float dropFloat = UnityEngine.Random.Range(0f, 1f);
        if (dropFloat < dropChance)
        {
            int randomInt = UnityEngine.Random.Range(dropMin, dropMax + 1);
            for (int j = 0; j < randomInt; j++)
            {
                //아이템을 생성한다
                GameObject _item = GameManager.Instance.poolManager.GetItem(item);
                _item.transform.position = transform.position;

                //아이템을 발사한다
                Vector2 randomUpDir = (transform.up + (transform.right * UnityEngine.Random.Range(-1, 1f))).normalized;
                float randomPow = UnityEngine.Random.Range(launchPowerMin, launchPowerMax);
                _item.GetComponent<Rigidbody2D>().AddForce(randomUpDir * randomPow, ForceMode2D.Impulse);
            }
        }
    }

}

[System.Serializable]
public struct DropTable
{
    [Range(0f, 1f)]
    public float dropChance;
    public GameObject item;
    public int dropMin;
    public int dropMax;

    public DropTable(float dropChance, GameObject item, int dropMin, int dropMax)
    {
        this.dropChance = dropChance;
        this.item = item;
        this.dropMin = dropMin;
        this.dropMax = dropMax;
    }
}


