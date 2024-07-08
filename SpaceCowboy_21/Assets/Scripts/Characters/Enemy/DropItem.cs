using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    /// <summary>
    /// ��� DropItem�� ������ ���� ������ Ammo�� ����� Ȯ���� �ִ�. 
    /// Ammo�� ������������ 1~4 �߿��� ��������. 
    /// �÷��̾ ������ Ammo�� ������ ��ӵǱ� �����Ѵ�. �������� ������ ��ӵ��� �ʴ´�. 
    /// Ammo����� Ȯ���� ��� �����ϴ�. 
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

    //    //1. ���� ������ ammo�� �� ī��Ʈ. PlayerManager���� WeaponInventory�� ��������, activate���� ���� ������� ���ؼ� ammoList�� �����. 
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

    //    //2. ����
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
                //�������� �����Ѵ�
                GameObject _item = GameManager.Instance.poolManager.GetItem(item);
                _item.transform.position = transform.position;

                //�������� �߻��Ѵ�
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


