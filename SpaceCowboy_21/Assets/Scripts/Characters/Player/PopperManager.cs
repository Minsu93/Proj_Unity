using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class PopperManager : MonoBehaviour
{
    //���̺� Ŭ����� ����
    [SerializeField] WaveManager waveManager;


    //�߻� ���� ����
    [SerializeField] GameObject fireworkPrefab;
    [SerializeField] private float gapBetweenOrbs = 3.0f;
    [SerializeField] private float launchHeight = 5.0f;
    [SerializeField] private float launchRadius = 5.0f;
    [SerializeField] private float launchInterval = 0.3f;
    [SerializeField] private AnimationCurve fireworkCurve;
    [SerializeField] private float launchTimer = 0.5f;

    //������ ������ ���
    [SerializeField]
    GameObject[] waveClearPops;

    //�÷��̾� ��ġ
    //Transform playerTr;

    private void Start()
    {
        if (waveClearPops.Length > 0)
        {
            waveManager.WaveClear += CreatePopper;
        }
    }

    //���� ��ü
    public void CreatePopper()
    {
        StartCoroutine(LaunchFireWork(waveClearPops));
    }

    //���� ����
    IEnumerator LaunchFireWork(GameObject[] items)
    {
        List<Vector2> targetPoints = GetEmptySpace(items.Length);
        Transform playerTr = GameManager.Instance.player;

        for(int i = 0; i < items.Length; i++)
        {
            // ���� �������� ������ ��ġ�� �����մϴ�.
            GameObject firework = GameManager.Instance.poolManager.GetItem(fireworkPrefab);
            firework.transform.position = playerTr.position;
            firework.transform.rotation = playerTr.rotation;
            StartCoroutine(MoveAndExplode(firework, playerTr.position, targetPoints[i], items[i]));

            // ���� ���� �߻���� �����̸� �ݴϴ�.
            yield return new WaitForSeconds(launchInterval);
        }
    }



    //�÷��̾� ���κ� �ֺ��� ����Ʈ ������. �༺�� ���� ���. 
    List<Vector2> GetEmptySpace(int number)
    {
        int maxAttempts = 20;
        List<Vector2> points = new List<Vector2>();
        Transform playerTr = GameManager.Instance.player;


        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0, Mathf.PI * 2); // 0���� 2�� ������ ������ ������ �����մϴ�.
            float distance = Mathf.Sqrt(Random.Range(0, Mathf.Pow(launchRadius, 2))); // ���� ���ο��� ������ �Ÿ��� �����մϴ�. �̶� ��Ʈ�� ����� ������ �� ������ �� ������ ������ Ȯ���� ����Ʈ�� �����ϱ� �����Դϴ�.
            Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // �� ������ �Ÿ��� ����Ͽ� �� ������ ������ ����Ʈ�� ����մϴ�.
            randomPoint += (Vector2)playerTr.position + (Vector2)playerTr.up * launchHeight;

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
                float angle = Random.Range(0, Mathf.PI * 2);
                float distance = Mathf.Sqrt(Random.Range(0, Mathf.Pow(1, 2)));
                Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // �� ������ �Ÿ��� ����Ͽ� �� ������ ������ ����Ʈ�� ����մϴ�.
                randomPoint += (Vector2)playerTr.position + (Vector2)playerTr.up;
                points.Add(randomPoint);
            }
        }
        return points;
    }


    IEnumerator MoveAndExplode(GameObject firework,Vector2 startPos, Vector2 targetPos, GameObject item)
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
        GameObject newOrb = GameManager.Instance.poolManager.GetItem(item);
        newOrb.transform.position = targetPos;
        newOrb.transform.rotation = Quaternion.identity;
        firework.SetActive(false);
    }
}
