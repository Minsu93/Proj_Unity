using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public bool activate;
    public string stageName = "stage0";

    [Header("Wave Property")]
    [SerializeField] float waveClearBonusSubtractTime = 5.0f;
    [SerializeField]
    [Range(0f,1f)] float waveClearRatio;
    float nextWaveTime; //웨이브 종료시까지 남은 시간
    float bossTime; //보스 스폰까지 남은 시간
    float gameTime; //게임 시간

    Stage stage;
    int stageIndex;
    Coroutine spawnRoutine;

    //웨이브
    int enemyLeftInWave;  //스테이지 스킵을 위해 현재 존재하는 적의 총 수, 잡을 때마다 줄어듬.
    int enemySpawnedInWave;   //스테이지 스킵을 위해 현재 존재하는 적의 총 수  
    //스테이지
    int enemyTotalSpawned;  //게임 시작부터 클리어까지 스폰된 적의 수. (필요x)
    int enemyRegularSpawned; //원래 계획대로면 스폰되었을 적의 수. 보너스 스폰의 수를 포함하지 않는다. 
    int enemyTotalKilled;   //전체 잡은 몬스터의 수. 보너스 포함. 
    //현재 
    List<GameObject> spawnedEnemyList = new List<GameObject>(); //현재 스폰되어있는 적의 리스트


    [Header("SpawnBox")]
    public float spawnRange = 5f;                  //랜덤 범위 넓이. 
    public float distanceUnitFromScreen = 5f;      //화면에서 떨어진 정도. 유닛 단위.
    float minAirDistance = 5f;  //플레이어 주변 최소 거리
    float maxAirDistance = 10f; //Air 타입 적의 공중 스폰시 플레이어 주변 최대 거리.


    float timer;
    float updateCycle = 0.5f;
    public Planet playerNearestPlanet { get; set; } //플레이어와 가장 가까운 행성을 추적한다. null이 나오지 않는 항시 가장 가까운 행성을 표시한다. 적들의 추적 용도로 사용한다. 


    //행성 관련
    [SerializeField] List<Planet> planetList = new List<Planet>();
     

    [Header("UI")]
    //스크립트.
    Dictionary<string, GameObject> monsterDict;

    [Header("Wave UI")]
    //Wave UI관련
    [SerializeField] Image waveProgressImg;
    [SerializeField] TextMeshProUGUI bossTimeLeftText;
    [SerializeField] TextMeshProUGUI totalEnemyKilledCountText;
    [SerializeField] TextMeshProUGUI regularSpawnEnemyCountText;
    [SerializeField] MinimapManager minimapManager;

    [SerializeField] GameObject MonsterIcon;
    [SerializeField] GameObject BonusIcon;
    [SerializeField] GameObject BossIcon;
    //List<GameObject> iconList = new List<GameObject>();

    //WaveUI관련 변수
    [SerializeField] RectTransform iconParent;
    [SerializeField] float iconSpawnTime;
    [SerializeField] float maxSpawnLimit = 60f;
    [SerializeField] float spawnPosMultiplier = 10f;
    int spawnIndex = 0;
    Dictionary<RectTransform, float> IconPairs = new Dictionary<RectTransform, float>();
    List<RectTransform> keyToRemove = new List<RectTransform>();


    //test(임시)
    //public float timeSpeed;

    public event System.Action StageClear;
    public event System.Action WaveClear;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Dictionary 불러오기
        monsterDict = GetComponent<MonsterDictonary>().monsterDictionary;

        //CreateWaveJson();
        LoadWaveFromJson(stageName);

        WaveStart();
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
    
    void LoadWaveFromJson(string filename)
    {
        string path = Path.Combine(Application.dataPath + "/Data/", filename + ".json");
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
        stageIndex = 0;
        spawnRoutine = null;
        bossTime = float.MaxValue;
        StopAllCoroutines();
    }

    public void WaveStart()
    {
        ResetWaveManager();
        bossTime = stage.bossSpawnTime;
        
        //소환될 적의 수 미리 계산
        for(int i = 0; i < stage.waves.Count; i++)
        {
            for(int j = 0; j < stage.waves[i].spawns.Count; j++)
            {
                enemyRegularSpawned += stage.waves[i].spawns[j].spawnCount;
            }
        }
        //UI에 적용
        regularSpawnEnemyCountText.text = enemyRegularSpawned.ToString();

        //보스 아이콘 추가
        GameObject bossobj = Instantiate(BossIcon, iconParent.transform);
        RectTransform bossRect = bossobj.GetComponent<RectTransform>();
        IconPairs.Add(bossRect, bossTime);

        activate = true;
        GetStagePlanets();
    }


    private void Update()
    {
        if (!activate) return;

        gameTime += Time.deltaTime;

        //보스 스폰까지 남은 시간 UI 표시
        //waveProgressImg.fillAmount = bossTimeLeft / stage.bossSpawnTime;
        bossTimeLeftText.text = Mathf.FloorToInt(bossTime - gameTime).ToString() + "s";

        //UI아이콘 관련 
        IconSpawner();
        IconMover();

        //보스 웨이브
        if (bossTime <= gameTime)
        {
            Debug.Log("보스 웨이브 시작!");
            activate = false;
            MonsterDisapper();
            SpawnWave(stage.bossWave);
            //bossTimeLeft = float.MaxValue;
            return;
        }

        //다음 웨이브로
        if (nextWaveTime <= gameTime)
        {
            MoveToNextWave();
            //waveTimeLeft = float.MaxValue;
            return;
        }


        //캐릭터 행성 업데이트? (몬스터 스폰 장소)
        if (activate)
        {
            if (timer < updateCycle)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0;
                playerNearestPlanet = SelectClosesetPlanetFromScreen(GameManager.Instance.player.position);
            }
        }
    }

    void IconSpawner()
    {
        //icon스폰
        if (iconSpawnTime - gameTime < maxSpawnLimit)
        {
            if (spawnIndex < stage.waves.Count)
            {
                //현재 시간에 아이콘 생성
                GameObject obj = Instantiate(MonsterIcon, iconParent.transform);
                RectTransform rect = obj.GetComponent<RectTransform>();
                IconPairs.Add(rect, iconSpawnTime);
                //다음 시간 측정
                iconSpawnTime += stage.waves[spawnIndex].totalTime;
                spawnIndex++;
            }
            else
            {
                //현재 시간에 아이콘 생성
                GameObject obj = Instantiate(BonusIcon, iconParent.transform);
                RectTransform rect = obj.GetComponent<RectTransform>();
                IconPairs.Add(rect, iconSpawnTime);
                //다음 시간 측정
                iconSpawnTime += stage.bonusWave.totalTime;
                spawnIndex++;
            }
        }

    }
    void IconMover()
    {
        foreach (KeyValuePair<RectTransform, float> icon in IconPairs)
        {
            if (icon.Value - gameTime < 0)
            {
                keyToRemove.Add(icon.Key);
            }

            icon.Key.anchoredPosition = new Vector2((icon.Value - gameTime) * spawnPosMultiplier, 0);
        }
        foreach (RectTransform rect in keyToRemove)
        {
            IconPairs.Remove(rect);
            rect.gameObject.SetActive(false);
        }

    }

    //몬스터가 죽을 때마다 실행. 
    public void CountEnemyLeft(GameObject obj)
    {
        if(spawnedEnemyList.Contains(obj))
        {
            spawnedEnemyList.Remove(obj);
            enemyLeftInWave--;
            enemyTotalKilled++;
            bossTime -= waveClearBonusSubtractTime;

            //UI 조절
            //leftEnemyCountText.text = enemyLeft.ToString();
            totalEnemyKilledCountText.text = enemyTotalKilled.ToString();
            minimapManager.RemoveMonster(obj);

            //적들이 비율밑으로 감소하면 웨이브 조기 클리어
            if ((float)enemyLeftInWave / enemySpawnedInWave < waveClearRatio)
            {
                gameTime = nextWaveTime;
                //MoveToNextWave();
            }
        }

    }

    //보스가 죽었을 때 실행
    public void BossKill()
    {

    }
    void MoveToNextWave()
    {
        Wave wave;
        //보너스 웨이브 생성
        if (stageIndex > stage.waves.Count - 1)
        {
            Debug.Log("보너스 웨이브 시작");
            wave = stage.bonusWave;
            SpawnWave(wave);
        }
        else
        {
            Debug.Log("기본 웨이브 시작");
            wave = stage.waves[stageIndex];
            SpawnWave(wave);
        }

        if(stageIndex > 0)
            WaveClearEvent();

        stageIndex++;
        nextWaveTime += wave.totalTime;
    }

    //웨이브 클리어 시 실행
    void WaveClearEvent()
    {
        if (WaveClear != null) WaveClear();
    }

    #region Spawns

    //소환된 몬스터들을 돌려보낸다. 
    void MonsterDisapper()
    {
        if (StageClear != null) StageClear();

        //UI 조절
        spawnedEnemyList.Clear();
        enemyLeftInWave = 0;
        //leftEnemyCountText.text = enemyLeft.ToString();
    }

    void SpawnWave(Wave wave)
    {
        //잡아야 할 총 적의 수를 새로 카운트 합니다. 
        //남아있던 적 + 새로 생성된 적.
        int newEnemyCount = 0;
        //남은 적의 수를 업데이트합니다. 
        foreach (Spawn spawn in wave.spawns)
        {
            newEnemyCount += spawn.spawnCount;
            enemyTotalSpawned += spawn.spawnCount;
        }
        enemySpawnedInWave = newEnemyCount + spawnedEnemyList.Count;
        enemyLeftInWave = enemySpawnedInWave;
        //leftEnemyCountText.text = enemyLeft.ToString();

        //UI를 업데이트 합니다.
        //preWaveTime = nextWaveTime;
        //waveIndexText.text = wave.waveIndex.ToString();

        //웨이브를 스폰합니다. 
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnInOrder(wave.spawns));


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
                EnemyAction act = prefab.GetComponent<EnemyAction>();

                Vector2 strikePos = Vector2.zero;

                switch (act.enemyType)
                {
                    case EnemyType.Ground:
                    case EnemyType.Orbit:
                        //적들을 생성지역 가장 가까이에 있는 행성으로 이동시킨다. 
                        Planet planet = SelectClosesetPlanetFromScreen(safePoint);
                        int strikePointIndex = planet.GetClosestIndex(transform.position);
                        strikePos = planet.worldPoints[strikePointIndex];
                        break;

                    case EnemyType.Air:
                        //적들을 캐릭터 주변 공중으로 이동시킨다. 
                        strikePos = GetRandomPointNearPlayer();
                        break;
                }
                //Strike시작
                prefab.GetComponent<EnemyAction>().EnemyStartStrike(strikePos);


                //소환된 적 리스트에 추가.
                spawnedEnemyList.Add(prefab);

                //소환된 적 UI에 추가
                minimapManager.AddMonster(prefab);
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
    public Planet SelectClosesetPlanetFromScreen(Vector2 point)
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


    //플레이어 주변의 랜덤한 공중 포인트를 불러온다. 
    Vector2 GetRandomPointNearPlayer()
    {
        Vector2 randomPoint = Vector2.zero;
        int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            //플레이어 위치에서 2~10 사이의 랜덤 포인트를 가져온다. 
            float radius = UnityEngine.Random.Range(minAirDistance, maxAirDistance);
            Vector2 dir = UnityEngine.Random.insideUnitCircle;
            Vector2 pos = (Vector2)transform.position + (dir * radius);

            //해당 위치가 행성과 겹치지 않는지 확인한다. 
            Collider2D coll = Physics2D.OverlapCircle(pos, 1f, LayerMask.GetMask("Planet"));
            if (coll == null)
            {
                randomPoint = pos;
                break;
            }
        }

        return randomPoint;
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



//몬스터의 이름과 몬스터 스폰 사이의 딜레이
[Serializable]
public class Enemy
{
    public string name;
    public float delay; //몬스터 스폰당 시간 차이.
}

//몬스터 웨이브가 얼마나 지속되는가. 
[Serializable]
public class Spawn
{
    public Enemy enemy;
    public int spawnCount;
    public float delayToNextWave; //다음 몬스터 무리 스폰 시간
}

//한번의 Wave는 여러개의 Spawn으로 구성됨 
[Serializable]
public class Wave
{
    public int waveIndex;
    public List<Spawn> spawns = new List<Spawn>();
    public float totalTime; //다음 웨이브 까지 시간 제한
}

[Serializable]
public class Stage
{
    public List<Wave> waves = new List<Wave>();
    public Wave bonusWave = new Wave();
    public Wave bossWave = new Wave();
    public float bossSpawnTime;
}

