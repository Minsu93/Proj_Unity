using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DropItem : MonoBehaviour
{
   
    [SerializeField] List<DropTable> dropTable = new List<DropTable>();

    /// <summary>
    /// 드롭하고 싶은 아이템(id)과 그 확률을 설정하자. 
    /// </summary>
    [SerializeField] List<ItemTable> itemTables = new List<ItemTable>();
    [Range(0f, 1f)]
    public float itemDropChance = 0.05f;


    [SerializeField] float resourceLaunchPowerMin = 3f;
    [SerializeField] float resourceLaunchPowerMax = 4f;

    private void Awake()
    {
        //EnemyAction enemyAction = GetComponent<EnemyAction>();
        //if(enemyAction != null )
        //    enemyAction.EnemyDieEvent += GenerateItem;
    }

    public void GenerateItem()
    {
        // resource Generate
        for (int i = 0; i < dropTable.Count; i++)
        {
            GenerateResource(dropTable[i].dropChance, dropTable[i].item, dropTable[i].dropMin, dropTable[i].dropMax, resourceLaunchPowerMin, resourceLaunchPowerMax);
        }

        //popperGenerate
        GameManager.Instance.popperManager.GiveWeaponToPlayer(this.transform);

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

    void GenerateResource(float dropChance, GameObject item, int dropMin, int dropMax, float launchPowerMin, float launchPowerMax)
    {
        float dropFloat = UnityEngine.Random.Range(0f, 1f);
        if (dropFloat < dropChance)
        {
            int randomInt = UnityEngine.Random.Range(dropMin, dropMax + 1);
            for (int j = 0; j < randomInt; j++)
            {
                //아이템을 생성한다
                GameObject _item = GameManager.Instance.poolManager.GetPoolObj(item, 2);
                _item.transform.position = transform.position;
                if(_item.TryGetComponent<ResourceDrop>(out var drop))
                {
                    drop.InitializeResource(0.6f, 0.8f);
                }
                

                //아이템을 발사한다
                Vector2 randomUpDir = (transform.up + (transform.right * UnityEngine.Random.Range(-1, 1f))).normalized;
                float randomPow = UnityEngine.Random.Range(launchPowerMin, launchPowerMax);
                _item.GetComponent<Rigidbody2D>().AddForce(randomUpDir * randomPow, ForceMode2D.Impulse);
            }
        }
    }

    //void GenerateWeapon()
    //{
    //    //Item 생성
    //    float dropFloat = UnityEngine.Random.Range(0f, 1f);
    //    if (dropFloat > itemDropChance)
    //        return;

    //    //drop할 아이템의 ID를 가져온다.
    //    if (itemTables.Count == 0) return;
    //    int id = itemTables[Choose(itemTables)].itemID;

    //    //아이템이 해금되어있는지 검사한다. 
    //    if (GameManager.Instance.techDocument.GetItemState(id) > 0)
    //    {
    //        //아이템을 생성한다
    //        GameObject _item = GameManager.Instance.poolManager.GetPoolObj(GameManager.Instance.techDocument.GetPrefab(id), 2);
    //        _item.transform.position = transform.position;
    //        _item.transform.rotation = transform.rotation;

    //        //아이템을 발사한다
    //        Vector2 randomUpDir = (transform.up + (transform.right * UnityEngine.Random.Range(-1, 1f))).normalized;
    //        float randomPow = UnityEngine.Random.Range(resourceLaunchPowerMin, resourceLaunchPowerMax);
    //        _item.GetComponent<Rigidbody2D>().AddForce(randomUpDir * randomPow, ForceMode2D.Impulse);
    //    }
    //}

    int Choose(List<ItemTable> probs)
    {

        float total = 0;

        foreach (var elem in probs)
        {
            total += elem.dropChance;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < probs.Count; i++)
        {
            if (randomPoint < probs[i].dropChance)
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i].dropChance;
            }
        }
        return probs.Count - 1;
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

[System.Serializable]

public class ItemTable
{
    public int itemID;
    public float dropChance;
}

