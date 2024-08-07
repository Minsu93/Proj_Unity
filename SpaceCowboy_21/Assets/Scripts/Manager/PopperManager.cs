using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;
using static UnityEditor.Progress;

public class PopperManager : MonoBehaviour
{

    //�߻� ���� ����
    [SerializeField] GameObject popperPrefab;
    [SerializeField] private float gapBetweenOrbs = 3.0f;
    [SerializeField] private float launchHeight = 5.0f;
    [SerializeField] private float launchRadius = 5.0f;
    [SerializeField] private float launchInterval = 0.3f;   //�߻� ���� ���� 
    [SerializeField] private AnimationCurve fireworkCurve;  //�߻� ������ 
    [SerializeField] private float launchTimer = 0.5f;  //�߻� �̵� �ð�

    //���� ��������
    [SerializeField] GameObject weaponBubble;
    [SerializeField] List<WeaponData> equippedWeaponDatas;    //������ ���� ���

    //��� Ȯ�� ���� ����
    List<EquippedWeapon> equippedWeapons; // ��� ������ ������ ���
    [SerializeField] float dropChanceIncrement = 0.1f; // ��� ���� �� Ȯ�� ������
    [SerializeField] float dropChanceDecrement = 0.05f; // ��� ���� �� Ȯ�� ���ҷ�
    [SerializeField] float totalDropChance = 0.1f;  // Bubble ��� Ȯ�� 
    [SerializeField] float dropCoolTime = 5f;   //��� �� ���� ���� ��ӱ��� ���� �ּ� �ð�
    bool dropReady = true;
    float timer = 0f;

    public void CreatePopper(Transform targetTr)
    {
        if (!dropReady) return;

        EquippedWeapon equip = TryDropItem();
        if(equip != null)
        {

            // ���� �������� ������ ��ġ�� �����մϴ�.
            GameObject firework = GameManager.Instance.poolManager.GetPoolObj(popperPrefab, 1);
            firework.transform.position = targetTr.position;
            firework.transform.rotation = targetTr.rotation;
            Vector2 targetPoint = GetEmptySpace(targetTr, 0)[0];

            StartCoroutine(MoveAndExplode(firework, targetTr.position, targetPoint, equip.weaponData));

            dropReady = false;
            totalDropChance = 0.1f;     //�ʱ�ȭ
        }
    }
    
    ////������ �߻��ϴ� ����
    //IEnumerator LaunchFireWork(Transform targetTr, GameObject[] items)
    //{
    //    List<Vector2> targetPoints = GetEmptySpace(targetTr,items.Length);

    //    for(int i = 0; i < items.Length; i++)
    //    {
    //        // ���� �������� ������ ��ġ�� �����մϴ�.
    //        GameObject firework = GameManager.Instance.poolManager.GetPoolObj(popperPrefab, 1);
    //        firework.transform.position = targetTr.position;
    //        firework.transform.rotation = targetTr.rotation;
    //        StartCoroutine(MoveAndExplode(firework, targetTr.position, targetPoints[i], items[i]));

    //        // ���� ���� �߻���� �����̸� �ݴϴ�.
    //        yield return new WaitForSeconds(launchInterval);
    //    }

    //    yield return null;
    //}

    //Ÿ�� ���κ� �ֺ��� ����Ʈ ������. �༺�� ���� ���. 
    List<Vector2> GetEmptySpace(Transform targetTr, int number)
    {
        int maxAttempts = 20;
        List<Vector2> points = new List<Vector2>();


        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = UnityEngine.Random.Range(0, Mathf.PI * 2); // 0���� 2�� ������ ������ ������ �����մϴ�.
            float distance = Mathf.Sqrt(UnityEngine.Random.Range(0, Mathf.Pow(launchRadius, 2))); // ���� ���ο��� ������ �Ÿ��� �����մϴ�. �̶� ��Ʈ�� ����� ������ �� ������ �� ������ ������ Ȯ���� ����Ʈ�� �����ϱ� �����Դϴ�.
            Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // �� ������ �Ÿ��� ����Ͽ� �� ������ ������ ����Ʈ�� ����մϴ�.
            randomPoint += (Vector2)targetTr.position + (Vector2)targetTr.up * launchHeight;

            Collider2D coll = Physics2D.OverlapCircle(randomPoint, 1f, LayerMask.GetMask("Planet"));
            if (coll == null)
            {
                //������ �߰��� ����Ʈ��� ������ �Ÿ��� ������ �ִ��� �˻��Ѵ�. 
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

        //���� ������ ��� �߰��ؼ� �ִ� ���� �����ش�. 
        if (points.Count < number)
        {
            int dificientNumbers = number - points.Count;
            for (int i = 0; i < dificientNumbers; i++)
            {
                float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
                float distance = Mathf.Sqrt(UnityEngine.Random.Range(0, Mathf.Pow(1, 2)));
                Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // �� ������ �Ÿ��� ����Ͽ� �� ������ ������ ����Ʈ�� ����մϴ�.
                randomPoint += (Vector2)targetTr.position + (Vector2)targetTr.up;
                points.Add(randomPoint);
            }
        }
        return points;
    }

    IEnumerator MoveAndExplode(GameObject firework,Vector2 startPos, Vector2 targetPos, WeaponData w_Data)
    {
        float time = 0f;

        // ������ ��ǥ ��ġ�� ������ ������ �̵��մϴ�.
        while (time <= launchTimer)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPos, fireworkCurve.Evaluate(time / launchTimer));
            firework.transform.position = pos;
            yield return null;
        }

        // ���� ����.
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
        //��� Ȯ�� ����
        float random = UnityEngine.Random.Range(0, 1.0f); 
        if (random < totalDropChance)
        {
            Debug.Log("Item dropped");

            //��� ���� ���� 
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

    //�� �����۵��� ��� ���� ����. �̹� ���� �������� ���� Ȯ���� ���� �پ���, �ٸ� �������� ���� Ȯ���� ��������.
    private void AdjustDropChances(EquippedWeapon droppedItem, bool isDropped)
    {
        if (isDropped)
        {
            foreach (var item in equippedWeapons)
            {
                if (item == droppedItem)
                {
                    item.dropChance = Mathf.Max(item.dropChance - dropChanceDecrement, 0.01f); // �ּ� Ȯ�� ����
                }
                else
                {
                    item.dropChance = Mathf.Min(item.dropChance + dropChanceIncrement, 1.0f); // �ִ� Ȯ�� ����
                }
            }
        }
        else
        {
            foreach (var item in equippedWeapons)
            {
                item.dropChance = Mathf.Min(item.dropChance + dropChanceIncrement, 1.0f); // �ִ� Ȯ�� ����
            }
        }

        //����ȭ
        ResizeDropChance();
    }

    // ��� Ȯ���� �ٽ� ����ȭ (total = 1�� Ȯ���� ��й�)
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