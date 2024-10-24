using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DropManager : MonoBehaviour
{
    [Header("Exp")]
    [SerializeField] float dropChance = 0.1f;
    [SerializeField] float baseLaunchPow = 3f;
    [SerializeField] List<ItemTable> itemTables = new List<ItemTable>();

    [Header("Money")]
    [SerializeField] ItemTable defaultMoney;
    [SerializeField] int minCount;
    [SerializeField] int maxCount;
    public void GenerateDrops(Transform tr)
    {
        int index = Choose(itemTables);
        GenerateResource(itemTables[index].item, dropChance,1,1, tr, baseLaunchPow);
        GenerateResource(defaultMoney.item, defaultMoney.dropChance, minCount, maxCount, tr, baseLaunchPow);
    }

    void GenerateResource(GameObject item, float dropChance, int minCount, int maxCount, Transform tr, float pow)
    {
        float dropFloat = UnityEngine.Random.Range(0f, 1f);
        if (dropFloat > dropChance) return;
        
        int count = UnityEngine.Random.Range(minCount, maxCount+1);
        for(int i =0; i < count; i++)
        {
            //아이템을 생성한다
            GameObject _item = GameManager.Instance.poolManager.GetPoolObj(item, 2);
            _item.transform.position = tr.position;

            //아이템을 발사한다
            float randomAngle = UnityEngine.Random.Range(-45 - 90, 45 - 90);
            Vector2 randomUpDir = Quaternion.Euler(0, 0, randomAngle) * tr.up;

            _item.GetComponent<Rigidbody2D>().AddForce(randomUpDir * pow, ForceMode2D.Impulse);
        }

    }


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

public class ItemTable
{
    public GameObject item;
    public float dropChance;
}
