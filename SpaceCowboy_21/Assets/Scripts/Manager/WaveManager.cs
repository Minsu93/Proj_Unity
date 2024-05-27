using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public bool activate;
    public string stageName = "stage0";

    [Header("Wave Property")]
    public float totalWaveTime; //마지막 웨이브까지의 시간
    public float gameTime { get; set; } //현재 스테이지 시간
    float nextWaveTime; //다음 웨이브가 시작될 시간
    float preWaveTime;  //이전 웨이브가 시작했던 시간

    Stage stage;
    int stageIndex;
    Coroutine spawnRoutine;
    public int enemyLeft { get; set; }
    List<GameObject> spawnedEnemyList = new List<GameObject>();


    [Header("SpawnBox")]
    public float spawnRange = 5f;                  //랜덤 범위 넓이. 
    public float distanceUnitFromScreen = 5f;      //화면에서 떨어진 정도. 유닛 단위.

    //행성 관련
    [SerializeField] List<Planet> planetList = new List<Planet>();

    //스크립트.
    Dictionary<string, GameObject> monsterDict;
    [SerializeField] MinimapIndicator minimapIndicator;

    //Wave UI관련
    [SerializeField] TextMeshProUGUI waveIndexText;
    [SerializeField] Image waveProgressImg;
    [SerializeField] TextMeshProUGUI leftEnemyCountText;

    //test(임시)
    public float timeSpeed;

    public event System.Action StageClear;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Dictionary 불러오기
        monsterDict = GetComponent<MonsterDictonary>().monsterDictionary;

        //CreateWaveJson();
        LoadWaveFromJson();

        WaveStart();
    }

    private void LateUpdate()
    {
        if (!activate) return;

        waveProgressImg.fillAmount = (gameTime - preWaveTime) / (nextWaveTime - preWaveTime);
    }

    void CreateWaveJson()
    {
        //json테스트용

        string path = Path.Combine(Application.dataPath + "/Data/", stageName + ".json");

        Enemy enemy_A = new Enemy();
        enemy_A.name = "cannon";
        enemy_A.delay = 0;

        Enemy enemy_B = new Enemy();
        enemy_B.name = "shooter";
        enemy_B.delay = 0;

        Spawn spawn_A = new Spawn();
        spawn_A.enemy = enemy_A;
        spawn_A.spawnCount = 5;
        spawn_A.delayToNextWave = 5.0f;

        Spawn spawn_B = new Spawn();
        spawn_B.enemy = enemy_B;
        spawn_B.spawnCount = 8;
        spawn_B.delayToNextWave = 5.0f;

        Wave wave_A = new Wave();
        wave_A.waveIndex = 0;
        wave_A.spawns.Add(spawn_A);
        wave_A.spawns.Add(spawn_B);
        wave_A.totalTime = 60f;

        Wave wave_B = new Wave();
        wave_B.waveIndex = 0;
        wave_B.spawns.Add(spawn_A);
        wave_B.spawns.Add(spawn_B);
        wave_B.totalTime = 120f;

        Stage stage = new Stage();
        stage.waves.Add(wave_A);
        stage.waves.Add(wave_B);

        string str = JsonUtility.ToJson(stage, true);
        File.WriteAllText(path, str);
    }
    
    void LoadWaveFromJson()
    {
        string path = Path.Combine(Application.dataPath + "/Data/", stageName + ".json");
        string loadJson = File.ReadAllText(path);
        stage = JsonUtility.FromJson<Stage>(loadJson);
    }

    //public void UpdateWaveManager(int invasionMinute, AnimationCurve waveCurve, EnemyTeam[] enemies)
    //{
    //    this.invasionMinute = invasionMinute;
    //    this.waveTimeCurve = waveCurve;
    //    this.enemyTeams = enemies;

    //    ResetWaveManager();

    //    waveTime = waveTimeCurve.Evaluate(waveIndex);   //웨이브Curve 의 0번째 시간, 즉 첫 웨이브 시작시간 업데이트
    //    if (waveUI != null) waveUI.UpdateProgressBar(waveTime, gameTime, waveIndex);    
    //    if (invasionUI != null) invasionUI.UpdateInvasionUI(InvasionTime);
    //}

    void ResetWaveManager()
    {
        gameTime = 0;
        nextWaveTime = 0;
        preWaveTime = 0;
        stageIndex = 0;
        spawnRoutine = null;
        StopAllCoroutines();
    }

    public void WaveStart()
    {
        ResetWaveManager();
        activate = true;
        GetStagePlanets();
    }


    private void Update()
    {
        if (!activate) return;


        gameTime += Time.deltaTime * timeSpeed;

        //다음 웨이브로
        if (gameTime > nextWaveTime)
        {
            MoveToNextWave();
        }
    }

    public void CountEnemyLeft(GameObject obj)
    {
        if(spawnedEnemyList.Contains(obj))
        {
            spawnedEnemyList.Remove(obj);
            enemyLeft--;

            //UI 조절
            leftEnemyCountText.text = enemyLeft.ToString();

            //적들이 다 없어지면
            if(enemyLeft == 0)
            {
                //남은 시간 변경
                if(nextWaveTime - gameTime > 5.0f)
                {
                    gameTime = nextWaveTime - 5.0f;
                }
            }
        }

    }

    void MoveToNextWave()
    {
        //이전 웨이브 종료 및 보스 생성.
        if (stageIndex > stage.waves.Count - 1)
        {
            Debug.Log("웨이브 종료");
            activate = false;
            MonsterDisapper();
            SpawnBoss();
            return;
        }

        Debug.Log("다음 웨이브 시작 : " + stage.waves[stageIndex].waveIndex);
        Wave curWave = stage.waves[stageIndex];

        //남은 적의 수를 업데이트합니다. 
        foreach(Spawn spawn in curWave.spawns)
        {
            enemyLeft += spawn.spawnCount;
        }
        leftEnemyCountText.text = enemyLeft.ToString();

        //UI를 업데이트 합니다.
        preWaveTime = nextWaveTime;
        waveIndexText.text = curWave.waveIndex.ToString();

        //웨이브를 스폰합니다. 
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnInOrder(curWave.spawns));

        //인덱스를 다음으로 조절합니다.
        stageIndex++;
        nextWaveTime += curWave.totalTime;
    }

    #region Spawns

    //소환된 몬스터들을 돌려보낸다. 
    void MonsterDisapper()
    {
        if (StageClear != null) StageClear();

        //UI 조절
        spawnedEnemyList.Clear();
        enemyLeft = 0;
        leftEnemyCountText.text = enemyLeft.ToString();
    }

    //보스타임 시작
    void SpawnBoss()
    {

    }

    //유물 스폰
    void SpawnArtifact()
    {
        //minimapIndicator에서 표시되게 적용하기.
    }

    #endregion

    #region Spawn Logics 

    //Spawns에 들어있는 Spawn들을 순서대로 소환한다. 
    IEnumerator SpawnInOrder(List<Spawn> spawns)
    {
        foreach(Spawn spawn in spawns)
        {
            StartCoroutine(SpawnObjects(spawn.enemy, spawn.spawnCount));
            yield return new WaitForSeconds(spawn.delayToNextWave);
        }
    }

    //enemies에 들어있는 스폰을 순서대로 실행한다. 
    IEnumerator SpawnObjects(Enemy enemy, int count)
    {
        GameObject enemyPrefab = monsterDict[enemy.name];

        for (int i = 0; i < count; i++)
        {
            Vector2 safePoint;
            if (GetSafePointFromOutsideScreen(out safePoint))
            {
                GameObject prefab = GameManager.Instance.poolManager.GetEnemy(enemyPrefab);
                prefab.transform.position = safePoint;
                prefab.transform.rotation = Quaternion.identity;
                prefab.GetComponent<EnemyAction>().EnemyStartStrike(SelectClosesetPlanetFromScreen(safePoint));
                
                //소환된 적 리스트에 추가.
                spawnedEnemyList.Add(prefab);
            }
            yield return new WaitForSeconds(enemy.delay);
        }

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
            switch (UnityEngine.Random.Range(0, 2))
            {
                case 0:
                    vec.x = UnityEngine.Random.Range(-spawnRange, spawnRange);
                    vec.y = UnityEngine.Random.Range(screenSizeMin.y - distanceUnitFromScreen - spawnRange, screenSizeMax.y + distanceUnitFromScreen + spawnRange);
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
                    vec.x = UnityEngine.Random.Range(screenSizeMin.x - distanceUnitFromScreen - spawnRange, screenSizeMax.x + distanceUnitFromScreen + spawnRange);
                    vec.y = UnityEngine.Random.Range(-spawnRange, spawnRange);
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

    #endregion

    #region Planet
    //게임 시작 시 맵 전체의 행성들을 불러온다
    void GetStagePlanets()
    {
        Planet[] planets = GameObject.FindObjectsOfType<Planet>();
        foreach (Planet planet in planets)
        {
            planetList.Add(planet);
        }
    }

    //스크린에 보이는 행성 중 하나를 고른다. 
    Planet SelectClosesetPlanetFromScreen(Vector2 point)
    {
        Planet planet = null;
        List<Planet> visiblePlanet = new List<Planet>(); 
        for(int i = 0; i < planetList.Count; i++)
        {
            if (PlanetBoundIsInScreen(planetList[i].polyCollider)) 
            {
                visiblePlanet.Add(planetList[i]);
            }
        }

        float minDist = float.MaxValue;
        foreach(Planet vp in visiblePlanet)
        {
            float betweenDist = vp.GetClosestDistance(point);
            if (minDist > betweenDist)
            {
                minDist = betweenDist;
                planet = vp;
            }
        }
        //planet = visiblePlanet[UnityEngine.Random.Range(0, visiblePlanet.Count)];
        return planet;
    }

    //스크린에 보이는지 검사한다. 
    public bool PlanetBoundIsInScreen(PolygonCollider2D polyCollider)
    {
        Bounds bounds = polyCollider.bounds;
        float margin = 150f;     //행성 감지용 여유 스크린 넓이


        Vector3[] corners = new Vector3[4];
        corners[0] = new Vector3(bounds.min.x, bounds.min.y, 0); // Bottom-left
        corners[1] = new Vector3(bounds.max.x, bounds.min.y, 0); // Bottom-right
        corners[2] = new Vector3(bounds.max.x, bounds.max.y, 0); // Top-right
        corners[3] = new Vector3(bounds.min.x, bounds.max.y, 0); // Top-left

        foreach (var corner in corners)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(corner);

            if (screenPoint.x > 0 - margin && screenPoint.x < Screen.width + margin)
            {
                if (screenPoint.y > 0 - margin && screenPoint.y < Screen.height + margin)
                {
                    return true; // One of the corners is outside the screen
                }
            }
        }

        return false; // All corners are inside the screen
    }

    #endregion

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



//몬스터의 이름과 몬스터 스폰 사이의 딜레이
[Serializable]
public class Enemy
{
    public string name;
    public float delay;
}

//몬스터 웨이브가 얼마나 지속되는가. 
[Serializable]
public class Spawn
{
    public Enemy enemy;
    public int spawnCount;
    public float delayToNextWave; 
}

//한번의 Wave는 여러개의 Spawn으로 구성됨 
[Serializable]
public class Wave
{
    public int waveIndex;
    public List<Spawn> spawns = new List<Spawn>();
    public float totalTime;
}

[Serializable]
public class Stage
{
    public List<Wave> waves = new List<Wave>();
}

