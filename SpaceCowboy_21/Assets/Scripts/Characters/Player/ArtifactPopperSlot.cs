using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class ArtifactPopperSlot : MonoBehaviour
{
    /// ����� ����
    /// ���꿡�� ������ �ҷ��ͼ� 
    /// ���� ��ų� �ϴ� ���� ī��Ʈ�� ���� �������� ȹ���ϰ� 
    /// �������� �� ���� ���긦 �����Ѵ�. 
    /// ����� ��ġ ����ó�� �߿�~ ��! �ϸ鼭 ���߿� �����Ǵµ� 
    /// ���� ���� ����Ʈ���� �༺�� ����� �Ѵ�. 
    /// 

    //����� ����
    public GameObject usingPopper { get; set; }

    //������ ������ �� 
    int popperNumbers;

    [Space]

    //�߻� ���� ����
    [SerializeField] GameObject fireworkPrefab;
    [SerializeField] private float gapBetweenOrbs = 3.0f;
    [SerializeField] private float launchHeight = 5.0f;
    [SerializeField] private float launchRadius = 5.0f;
    [SerializeField] private float launchInterval = 0.3f;
    [SerializeField] private AnimationCurve fireworkCurve;
    [SerializeField] private float launchTimer = 0.5f;
    //���� ���� ���� 
    int energyCost;
    //���� ������
    float currentEnergy;

    private void Awake()
    {
        currentEnergy = 0f;
    }

    //���� ��ü
    public void ChangePopper(GameObject changePopper)
    {
        usingPopper = changePopper;
        Orb currPopper = usingPopper.GetComponent<Orb>();
        popperNumbers = currPopper.orbNumbers;
        energyCost = currPopper.energyNeeded;
    }

    //������ ��� 
    public void EnergyIncrease(float amount)
    {
        currentEnergy += amount;    
        if(currentEnergy > energyCost)
        {
            currentEnergy -= energyCost;
            StartCoroutine(LaunchFireWork());
        }
    }

    //�÷��̾� ���κ� �ֺ��� ����Ʈ ������. �༺�� ���� ���. 
    List<Vector2> GetEmptySpace()
    {
        int maxAttempts = 20;
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0, Mathf.PI * 2); // 0���� 2�� ������ ������ ������ �����մϴ�.
            float distance = Mathf.Sqrt(Random.Range(0, Mathf.Pow(launchRadius, 2))); // ���� ���ο��� ������ �Ÿ��� �����մϴ�. �̶� ��Ʈ�� ����� ������ �� ������ �� ������ ������ Ȯ���� ����Ʈ�� �����ϱ� �����Դϴ�.
            Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // �� ������ �Ÿ��� ����Ͽ� �� ������ ������ ����Ʈ�� ����մϴ�.
            randomPoint += (Vector2)transform.position + (Vector2)transform.up * launchHeight;

            Collider2D coll = Physics2D.OverlapCircle(randomPoint, 1f, LayerMask.GetMask("Planet"));
            if (coll == null)
            {
                //������ �߰��� ����Ʈ��� ������ �Ÿ��� ������ �ִ��� �˻��Ѵ�. 
                bool close = false;
                foreach(Vector2 vec in points)
                {
                    if (Vector2.Distance(vec, randomPoint) < gapBetweenOrbs)
                    {
                        close = true; 
                    }
                }
                if (close) continue;

                points.Add(randomPoint);
                if (points.Count >= popperNumbers) break;
            }
        }

        //���� ������ ��� �߰��ؼ� �ִ� ���� �����ش�. 
        if(points.Count < popperNumbers)
        {
            int dificientNumbers = popperNumbers - points.Count;
            for (int i  = 0; i < dificientNumbers; i++)
            {
                float angle = Random.Range(0, Mathf.PI * 2);
                float distance = Mathf.Sqrt(Random.Range(0, Mathf.Pow(1, 2)));
                Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // �� ������ �Ÿ��� ����Ͽ� �� ������ ������ ����Ʈ�� ����մϴ�.
                randomPoint += (Vector2)transform.position + (Vector2)transform.up;
                points.Add(randomPoint) ;
            }
        }
        return points;
    }

    //���� ����
    IEnumerator LaunchFireWork()
    {
        List<Vector2> targetPoints = GetEmptySpace();

        for(int i = 0; i < popperNumbers; i++)
        {
            // ���� �������� ������ ��ġ�� �����մϴ�.
            GameObject firework = GameManager.Instance.poolManager.GetItem(fireworkPrefab);
            firework.transform.position = transform.position;
            firework.transform.rotation = transform.rotation;
            StartCoroutine(MoveAndExplode(firework, targetPoints[i]));

            // ���� ���� �߻���� �����̸� �ݴϴ�.
            yield return new WaitForSeconds(launchInterval);
        }
    }

    IEnumerator MoveAndExplode(GameObject firework, Vector2 targetPos)
    {
        Vector2 startPos = transform.position;
        Vector3 targetPosition = targetPos;
        float time = 0f;

        // ������ ��ǥ ��ġ�� ������ ������ �̵��մϴ�.
        while (time <= launchTimer)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPosition, fireworkCurve.Evaluate(time / launchTimer));
            firework.transform.position = pos;
            yield return null;
        }

        // ���� ����.
        GameObject newOrb = GameManager.Instance.poolManager.GetItem(usingPopper);
        newOrb.transform.position = targetPosition;
        newOrb.transform.rotation = Quaternion.identity;
        firework.SetActive(false);
    }
}
