using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public bool activate;

    public int invasionMinute = 1;
    public float InvasionTime { get { return invasionMinute * 60; } }    //침공 완료까지 전체 시간 

    public float gameTime { get; set; } //현재 스테이지 시간

    float waveTime; //다음 웨이브가 시작될 시간
    AnimationCurve waveTimeCurve;
    int waveIndex;  //레벨 번호 0 ~ 3

    public float spawnRange = 5f;                  //랜덤 범위 넓이. 
    public float distanceUnitFromScreen = 5f;      //화면에서 떨어진 정도. 유닛 단위.

    //스크립트.
    public EnemyTeam[] enemyTeams;  //각 웨이브마다 스폰될 적들의 정보.
    public WaveUI waveUI;
    public InvasionUI invasionUI;
    
    //test(임시)
    public GameObject testObj;
    public GameObject testObj2;
    public float timeSpeed;

    private void Awake()
    {
        instance = this;
       
    }

    public void UpdateWaveManager(int invasionMinute, AnimationCurve waveCurve, EnemyTeam[] enemies)
    {
        this.invasionMinute = invasionMinute;
        this.waveTimeCurve = waveCurve;
        this.enemyTeams = enemies;

        ResetWaveManager();

        waveTime = waveTimeCurve.Evaluate(waveIndex);   //웨이브Curve 의 0번째 시간, 즉 첫 웨이브 시작시간 업데이트
        if (waveUI != null) waveUI.UpdateProgressBar(waveTime, gameTime, waveIndex);    
        if (invasionUI != null) invasionUI.UpdateInvasionUI(InvasionTime);
    }

    void ResetWaveManager()
    {
        waveIndex = 0;  //초기화
        gameTime = 0;
        waveTime = 0;
    }

    public void WaveStart(bool start)
    {
        activate = start;
    }



    private void Update()
    {
        if (!activate) return; 

        gameTime += Time.deltaTime * timeSpeed;

        if(gameTime >= waveTime)
        {
            //스폰 횟수
            int spawnRepeat = Mathf.FloorToInt((waveIndex + 10) / 10);
            spawnRepeat = Mathf.Clamp(spawnRepeat, 1, 3);
            //스폰 종류
            GameObject spawnObj;
            if(waveIndex < 10)
            {
                spawnObj = testObj;
            }
            else
            {
                switch(Random.Range(0, 2))
                {
                    case 0: spawnObj = testObj; break;
                    default: spawnObj = testObj2; break;
                }
            }
            //스폰
            for(int i = 0; i < spawnRepeat; i++)
            {
                SpawnEnemy(spawnObj);

            }
            NextLevel();
        }
    }

    void NextLevel()
    {
        waveIndex ++;
        waveTime += waveTimeCurve.Evaluate(waveIndex);
        if (waveUI != null) waveUI.UpdateProgressBar(waveTime,gameTime, waveIndex);
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        //랜덤 위치를 가져옵니다. 
        Vector2 safePoint;
        if (GetSafePointFromOutsideScreen(out safePoint))
        {
            GameObject enemy =  PoolManager.instance.GetEnemy(enemyPrefab);
            enemy.transform.position = safePoint;
            enemy.transform.rotation = Quaternion.identity;
            enemy.GetComponent<EnemyBrain>().ResetEnemyBrain(EnemyState.Strike);
        }

        Debug.Log("Wave : " + waveIndex);
    }



    bool GetSafePointFromOutsideScreen(out Vector2 safePoint)
    {
        //스크린 > 월드 좌표 측정. z = 10 은 카메라와의 거리.
        Vector3 screenSizeMin = new Vector3(0, 0, 10);
        Vector3 screenSizeMax = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight,10);
        screenSizeMin = Camera.main.ScreenToWorldPoint(screenSizeMin);
        screenSizeMax = Camera.main.ScreenToWorldPoint(screenSizeMax);

        bool findSafePoint = false;

        int saftyCounter = 0;
        int maxSaftyCounter = 10;

        safePoint = Vector2.zero;
        Vector2 vec;

        while(!findSafePoint && saftyCounter < maxSaftyCounter)
        {
            switch (Random.Range(0, 2))
            {
                case 0:
                    vec.x = Random.Range(-spawnRange, spawnRange);
                    vec.y = Random.Range(screenSizeMin.y - distanceUnitFromScreen - spawnRange, screenSizeMax.y + distanceUnitFromScreen + spawnRange);
                    if (vec.x > 0)
                    {
                        vec.x = vec.x + screenSizeMax.x + distanceUnitFromScreen;
                    }
                    else
                    {
                        vec.x = vec.x + screenSizeMin.x - distanceUnitFromScreen;
                    }
                    break;
                
                default:
                    vec.x = Random.Range(screenSizeMin.x - distanceUnitFromScreen - spawnRange, screenSizeMax.x + distanceUnitFromScreen + spawnRange);
                    vec.y = Random.Range(-spawnRange, spawnRange);
                    if (vec.y > 0)
                    {
                        vec.y = vec.y + screenSizeMax.y + distanceUnitFromScreen;
                    }
                    else
                    {
                        vec.y = vec.y + screenSizeMin.y - distanceUnitFromScreen;
                    }
                    break;
            }
            
            if (IsLocationSafe(vec))
            {
                safePoint = vec;
                findSafePoint = true;
            }
            saftyCounter++;
        }

        return findSafePoint;
    }

    private bool IsLocationSafe(Vector2 position)
    {
        return Physics2D.OverlapCircle(position, 2f, LayerMask.GetMask("Planet")) == null;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 screenSizeMin = new Vector3(0, 0, 10);
        Vector3 screenSizeMax = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight, 10);
        Vector2 min = Camera.main.ScreenToWorldPoint(screenSizeMin);
        Vector2 max = Camera.main.ScreenToWorldPoint(screenSizeMax);
        min.x -= distanceUnitFromScreen;
        min.y -= distanceUnitFromScreen;
        max.x += distanceUnitFromScreen;
        max.y += distanceUnitFromScreen;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(min.x, min.y), new Vector2(min.x, max.y));
        Gizmos.DrawLine(new Vector2(min.x, max.y), new Vector2(max.x, max.y));
        Gizmos.DrawLine(new Vector2(min.x, min.y), new Vector2(max.x, min.y));
        Gizmos.DrawLine(new Vector2(max.x, min.y), new Vector2(max.x, max.y));

        min.x -= spawnRange;
        min.y -= spawnRange;
        max.x += spawnRange;
        max.y += spawnRange;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector2(min.x, min.y), new Vector2(min.x, max.y));
        Gizmos.DrawLine(new Vector2(min.x, max.y), new Vector2(max.x, max.y));
        Gizmos.DrawLine(new Vector2(min.x, min.y), new Vector2(max.x, min.y));
        Gizmos.DrawLine(new Vector2(max.x, min.y), new Vector2(max.x, max.y));
    }
}

[System.Serializable]
public class EnemyPrefab
{
    public GameObject prefab;   
    public int count;   
}

[System.Serializable]
public class EnemyTeam
{
    public EnemyPrefab[] enemyPrefabs;
}