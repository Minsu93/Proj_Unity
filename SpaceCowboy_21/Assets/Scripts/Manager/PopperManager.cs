using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using static UnityEditor.Progress;

public class PopperManager : MonoBehaviour
{

    //발사 사이 간격
    [SerializeField] GameObject popperPrefab;
    [SerializeField] private float gapBetweenOrbs = 3.0f;
    [SerializeField] private float launchHeight = 5.0f;
    [SerializeField] private float launchRadius = 5.0f;
    [SerializeField] private float launchInterval = 0.3f;   //발사 사이 간격 
    [SerializeField] private AnimationCurve fireworkCurve;  //발사 움직임 
    [SerializeField] private float launchTimer = 0.5f;  //발사 이동 시간

    //무기 생성관련
    [SerializeField] GameObject weaponBubble;
    [SerializeField] List<WeaponData> equippedWeaponDatas;    //생성될 무기 목록

    //드롭 확률 조절 관련
    List<EquippedWeapon> equippedWeapons; // 드롭 가능한 아이템 목록
    [SerializeField] float dropChanceIncrement = 0.1f; // 드롭 실패 시 확률 증가량
    [SerializeField] float dropChanceDecrement = 0.05f; // 드롭 성공 시 확률 감소량
    [SerializeField] float totalDropChance = 0.1f;  // Bubble 드롭 확률 
    [SerializeField] float dropCoolTime = 5f;   //드롭 된 이후 다음 드롭까지 제한 최소 시간
    bool dropReady = true;
    float timer = 0f;

    public void CreatePopper(Transform targetTr)
    {
        if (!dropReady) return;

        EquippedWeapon equip = TryDropItem();
        if(equip != null)
        {

            // 폭죽 프리팹을 랜덤한 위치에 생성합니다.
            GameObject firework = GameManager.Instance.poolManager.GetPoolObj(popperPrefab, 1);
            firework.transform.position = targetTr.position;
            firework.transform.rotation = targetTr.rotation;
            Vector2 targetPoint = GetEmptySpace(targetTr, 0)[0];

            StartCoroutine(MoveAndExplode(firework, targetTr.position, targetPoint, equip.weaponData));

            dropReady = false;
            totalDropChance = 0.1f;     //초기화
        }
    }
    
    ////여러개 발사하는 로직
    //IEnumerator LaunchFireWork(Transform targetTr, GameObject[] items)
    //{
    //    List<Vector2> targetPoints = GetEmptySpace(targetTr,items.Length);

    //    for(int i = 0; i < items.Length; i++)
    //    {
    //        // 폭죽 프리팹을 랜덤한 위치에 생성합니다.
    //        GameObject firework = GameManager.Instance.poolManager.GetPoolObj(popperPrefab, 1);
    //        firework.transform.position = targetTr.position;
    //        firework.transform.rotation = targetTr.rotation;
    //        StartCoroutine(MoveAndExplode(firework, targetTr.position, targetPoints[i], items[i]));

    //        // 다음 폭죽 발사까지 딜레이를 줍니다.
    //        yield return new WaitForSeconds(launchInterval);
    //    }

    //    yield return null;
    //}

    //타겟 윗부분 주변의 포인트 가져옴. 행성이 없는 장소. 
    List<Vector2> GetEmptySpace(Transform targetTr, int number)
    {
        int maxAttempts = 20;
        List<Vector2> points = new List<Vector2>();


        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = UnityEngine.Random.Range(0, Mathf.PI * 2); // 0부터 2π 사이의 랜덤한 각도를 생성합니다.
            float distance = Mathf.Sqrt(UnityEngine.Random.Range(0, Mathf.Pow(launchRadius, 2))); // 원의 내부에서 랜덤한 거리를 생성합니다. 이때 루트를 씌우는 이유는 원 내부의 각 영역에 동일한 확률로 포인트를 생성하기 위함입니다.
            Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // 이 각도와 거리를 사용하여 원 내부의 랜덤한 포인트를 계산합니다.
            randomPoint += (Vector2)targetTr.position + (Vector2)targetTr.up * launchHeight;

            Collider2D coll = Physics2D.OverlapCircle(randomPoint, 1f, LayerMask.GetMask("Planet"));
            if (coll == null)
            {
                //이전에 추가된 포인트들과 적당히 거리가 떨어져 있는지 검사한다. 
                bool close = false;
                foreach (Vector2 vec in points)
                {
                    if (Vector2.Distance(vec, randomPoint) < gapBetweenOrbs)
                    {
                        close = true;
                    }
                }
                if (close) continue;

                points.Add(randomPoint);
                if (points.Count >= number) break;
            }
        }

        //갯수 부족인 경우 추가해서 최대 수를 맞춰준다. 
        if (points.Count < number)
        {
            int dificientNumbers = number - points.Count;
            for (int i = 0; i < dificientNumbers; i++)
            {
                float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
                float distance = Mathf.Sqrt(UnityEngine.Random.Range(0, Mathf.Pow(1, 2)));
                Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // 이 각도와 거리를 사용하여 원 내부의 랜덤한 포인트를 계산합니다.
                randomPoint += (Vector2)targetTr.position + (Vector2)targetTr.up;
                points.Add(randomPoint);
            }
        }
        return points;
    }

    IEnumerator MoveAndExplode(GameObject firework,Vector2 startPos, Vector2 targetPos, WeaponData w_Data)
    {
        float time = 0f;

        // 폭죽이 목표 위치에 도달할 때까지 이동합니다.
        while (time <= launchTimer)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPos, fireworkCurve.Evaluate(time / launchTimer));
            firework.transform.position = pos;
            yield return null;
        }

        // 오브 생성.
        GameObject newOrb = GameManager.Instance.poolManager.GetPoolObj(weaponBubble, 2);
        newOrb.transform.position = targetPos;
        newOrb.transform.rotation = Quaternion.identity;
        Bubble_Weapon bubble = newOrb.GetComponent<Bubble_Weapon>();
        bubble.SetBubble(w_Data);

        firework.SetActive(false);

    }



    void Start()
    {
        equippedWeapons = new List<EquippedWeapon> (equippedWeaponDatas.Count );
        foreach(WeaponData prefab  in equippedWeaponDatas)
        {
            equippedWeapons.Add(new EquippedWeapon(prefab, 0.1f));
        }
        ResizeDropChance();
    }

    private void Update()
    {
        if (!dropReady)
        {
            timer += Time.deltaTime;
            if(timer > dropCoolTime)
            {
                dropReady = true;
            }
        }
    }

    public EquippedWeapon TryDropItem()
    {
        //드랍 확률 선택
        float random = UnityEngine.Random.Range(0, 1.0f); 
        if (random < totalDropChance)
        {
            Debug.Log("Item dropped");

            //드랍 종류 선택 
            float roll = UnityEngine.Random.Range(0, 1.0f); 
            float cumulativeChance = 0.0f;

            foreach (var item in equippedWeapons)
            {
                cumulativeChance += item.dropChance;

                if (roll < cumulativeChance)
                {
                    AdjustDropChances(item, true);
                    totalDropChance = Mathf.Max(totalDropChance - dropChanceDecrement, 0.01f);
                    return item;
                }
            }
        }
        AdjustDropChances(null, false);
        totalDropChance = Mathf.Min(totalDropChance + dropChanceIncrement, 1.0f);
        Debug.Log("No item dropped");
        return null;

    }

    //각 아이템들의 드롭 찬스 변경. 이미 나온 아이템이 나올 확률은 점점 줄어들고, 다른 아이템이 나올 확률은 높아진다.
    private void AdjustDropChances(EquippedWeapon droppedItem, bool isDropped)
    {
        if (isDropped)
        {
            foreach (var item in equippedWeapons)
            {
                if (item == droppedItem)
                {
                    item.dropChance = Mathf.Max(item.dropChance - dropChanceDecrement, 0.01f); // 최소 확률 제한
                }
                else
                {
                    item.dropChance = Mathf.Min(item.dropChance + dropChanceIncrement, 1.0f); // 최대 확률 제한
                }
            }
        }
        else
        {
            foreach (var item in equippedWeapons)
            {
                item.dropChance = Mathf.Min(item.dropChance + dropChanceIncrement, 1.0f); // 최대 확률 제한
            }
        }

        //정규화
        ResizeDropChance();
    }

    // 드롭 확률을 다시 정규화 (total = 1로 확률을 재분배)
    void ResizeDropChance()
    {
        float totalChance = 0.0f;
        foreach (var item in equippedWeapons)
        {
            totalChance += item.dropChance;
        }

        for (int i = 0; i < equippedWeapons.Count; i++)
        {
            equippedWeapons[i].dropChance /= totalChance;
        }
    }
}


[Serializable]
public class EquippedWeapon
{
    public WeaponData weaponData;
    public float dropChance;

    public EquippedWeapon(WeaponData bubble, float chance)
    {
        weaponData = bubble;
        dropChance = chance;
    }
}