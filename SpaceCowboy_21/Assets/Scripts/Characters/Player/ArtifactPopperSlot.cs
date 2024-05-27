using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class ArtifactPopperSlot : MonoBehaviour
{
    /// 사용할 오브
    /// 오브에서 정보를 불러와서 
    /// 적을 잡거나 하는 등의 카운트를 통해 에너지를 획득하고 
    /// 에너지가 다 차면 오브를 생성한다. 
    /// 오브는 마치 폭죽처럼 삐용~ 팡! 하면서 공중에 생성되는데 
    /// 공중 생성 포인트에는 행성이 없어야 한다. 
    /// 

    //사용할 오브
    public GameObject usingPopper { get; set; }

    //생성할 오브의 수 
    int popperNumbers;

    [Space]

    //발사 사이 간격
    [SerializeField] GameObject fireworkPrefab;
    [SerializeField] private float gapBetweenOrbs = 3.0f;
    [SerializeField] private float launchHeight = 5.0f;
    [SerializeField] private float launchRadius = 5.0f;
    [SerializeField] private float launchInterval = 0.3f;
    [SerializeField] private AnimationCurve fireworkCurve;
    [SerializeField] private float launchTimer = 0.5f;
    //오브 생성 조건 
    int energyCost;
    //현재 에너지
    float currentEnergy;

    private void Awake()
    {
        currentEnergy = 0f;
    }

    //오브 교체
    public void ChangePopper(GameObject changePopper)
    {
        usingPopper = changePopper;
        Orb currPopper = usingPopper.GetComponent<Orb>();
        popperNumbers = currPopper.orbNumbers;
        energyCost = currPopper.energyNeeded;
    }

    //에너지 상승 
    public void EnergyIncrease(float amount)
    {
        currentEnergy += amount;    
        if(currentEnergy > energyCost)
        {
            currentEnergy -= energyCost;
            StartCoroutine(LaunchFireWork());
        }
    }

    //플레이어 윗부분 주변의 포인트 가져옴. 행성이 없는 장소. 
    List<Vector2> GetEmptySpace()
    {
        int maxAttempts = 20;
        List<Vector2> points = new List<Vector2>();

        for (int i = 0; i < maxAttempts; i++)
        {
            float angle = Random.Range(0, Mathf.PI * 2); // 0부터 2π 사이의 랜덤한 각도를 생성합니다.
            float distance = Mathf.Sqrt(Random.Range(0, Mathf.Pow(launchRadius, 2))); // 원의 내부에서 랜덤한 거리를 생성합니다. 이때 루트를 씌우는 이유는 원 내부의 각 영역에 동일한 확률로 포인트를 생성하기 위함입니다.
            Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // 이 각도와 거리를 사용하여 원 내부의 랜덤한 포인트를 계산합니다.
            randomPoint += (Vector2)transform.position + (Vector2)transform.up * launchHeight;

            Collider2D coll = Physics2D.OverlapCircle(randomPoint, 1f, LayerMask.GetMask("Planet"));
            if (coll == null)
            {
                //이전에 추가된 포인트들과 적당히 거리가 떨어져 있는지 검사한다. 
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

        //갯수 부족인 경우 추가해서 최대 수를 맞춰준다. 
        if(points.Count < popperNumbers)
        {
            int dificientNumbers = popperNumbers - points.Count;
            for (int i  = 0; i < dificientNumbers; i++)
            {
                float angle = Random.Range(0, Mathf.PI * 2);
                float distance = Mathf.Sqrt(Random.Range(0, Mathf.Pow(1, 2)));
                Vector2 randomPoint = new Vector2(distance * Mathf.Cos(angle), distance * Mathf.Sin(angle)); // 이 각도와 거리를 사용하여 원 내부의 랜덤한 포인트를 계산합니다.
                randomPoint += (Vector2)transform.position + (Vector2)transform.up;
                points.Add(randomPoint) ;
            }
        }
        return points;
    }

    //오브 가동
    IEnumerator LaunchFireWork()
    {
        List<Vector2> targetPoints = GetEmptySpace();

        for(int i = 0; i < popperNumbers; i++)
        {
            // 폭죽 프리팹을 랜덤한 위치에 생성합니다.
            GameObject firework = GameManager.Instance.poolManager.GetItem(fireworkPrefab);
            firework.transform.position = transform.position;
            firework.transform.rotation = transform.rotation;
            StartCoroutine(MoveAndExplode(firework, targetPoints[i]));

            // 다음 폭죽 발사까지 딜레이를 줍니다.
            yield return new WaitForSeconds(launchInterval);
        }
    }

    IEnumerator MoveAndExplode(GameObject firework, Vector2 targetPos)
    {
        Vector2 startPos = transform.position;
        Vector3 targetPosition = targetPos;
        float time = 0f;

        // 폭죽이 목표 위치에 도달할 때까지 이동합니다.
        while (time <= launchTimer)
        {
            time += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPosition, fireworkCurve.Evaluate(time / launchTimer));
            firework.transform.position = pos;
            yield return null;
        }

        // 오브 생성.
        GameObject newOrb = GameManager.Instance.poolManager.GetItem(usingPopper);
        newOrb.transform.position = targetPosition;
        newOrb.transform.rotation = Quaternion.identity;
        firework.SetActive(false);
    }
}
