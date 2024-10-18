using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    public WeaponData[] SpawnList;
    public float distance = 6f;
    [SerializeField] GameObject weaponBubble;

    Dictionary<WeaponData, Vector2> keyValuePairs = new Dictionary<WeaponData, Vector2>();
    int count;
    ///
    /// 1. SpawnList 수 만큼의 Bubble을 행성 주변에 생성한다
    /// 2. 생성한 SpawnBubble의 ConsumeEvent에 몇초 후 재생성을 등록한다.  
    /// 

    private void Start()
    {
        CalculateSpace();

        for(int i =0; i < count; i++)
        {
            SpawnBubbles(SpawnList[i], keyValuePairs[SpawnList[i]]);
        }
    }


    void CalculateSpace()
    {
        count = SpawnList.Length;
        if (count == 0) return;
        float angle = 360 / count;

        for(int i =0; i < count; i++)
        {
            Vector2 pos = Quaternion.Euler(0, 0, angle * i) * Vector2.right * distance;
            pos += (Vector2) transform.position;
            keyValuePairs.Add(SpawnList[i], pos);
        }
    }

    void SpawnBubbles(WeaponData data,Vector2 pos)
    {
        GameObject newOrb = GameManager.Instance.poolManager.GetPoolObj(weaponBubble, 2);
        newOrb.transform.position = pos;
        newOrb.transform.rotation = Quaternion.identity;
        Bubble_Weapon bubble = newOrb.GetComponent<Bubble_Weapon>();
        bubble.SetBubble(data);
        bubble.WeaponConsumeEvent -= RegenerateBubble;
        bubble.WeaponConsumeEvent += RegenerateBubble;
    }

    public void RegenerateBubble(WeaponData data)
    {
        Vector2 spawnPos = keyValuePairs[data];
        StartCoroutine(RegenRoutine(data, spawnPos));
    }

    IEnumerator RegenRoutine(WeaponData data, Vector2 pos)
    {
        yield return new WaitForSeconds(3f);
        SpawnBubbles(data, pos);
    }

    

}
