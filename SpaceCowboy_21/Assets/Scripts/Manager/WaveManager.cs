using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    [SerializeField] StageManager stageManager;

    [Header("Wave Property")]
    [SerializeField] float startTime = 5.0f;
    float nextWaveTime; //웨이브 종료시까지 남은 시간
    float gameTime; //게임 시간

    Stage stage;
    int waveIndex;
    bool activateStage;  //웨이브 활성화 여부
    bool FinalWave = false;  //보스 웨이브 시작했나요?
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
    WaveObjectRespawner objectRespawner;
    [SerializeField] GameObject arrowCanvas;

    [Header("Wave UI")]
    //Wave UI관련
    [SerializeField] GameObject waveCanvas;
    WaveProgress waveProgress;
    WavePanelUI wavePanelUI;
    [SerializeField] int totalStageCount = 5;
    ArrowIndicatorManager minimapManager;




    public event System.Action MonsterDisappearEvent;
    public event System.Action WaveClear;


    private void Awake()
    {
        instance = this;

        //웨이브 UI 생성
        GameObject waveUIObj =  Instantiate(waveCanvas);
        wavePanelUI = waveUIObj.transform.GetComponentInChildren<WavePanelUI>();
        wavePanelUI.SetStageIcons(stageManager.MaxStage);
        wavePanelUI.gameObject.SetActive(false);

        waveProgress = waveUIObj.GetComponentInChildren<WaveProgress>();

        Instantiate(arrowCanvas);
        minimapManager = Instantiate(arrowCanvas).GetComponent<ArrowIndicatorManager>();
    }

    private void Start()
    {
        //Dictionary 불러오기
        monsterDict = GameManager.Instance.monsterDictonary.monsDictionary;
        objectRespawner = GetComponent<WaveObjectRespawner>();  

    }

    private void Update()
    {
        if (!activateStage) return;

        gameTime += Time.deltaTime;

        PlayerPlanetUpdate();

        //Stage 오브젝트 리스폰
        objectRespawner.UpdateSpawner();

        //최종 웨이브인 경우 이후는 업데이트 하지 않는다. 
        if (FinalWave) return;

        waveProgress.IconSpawner(gameTime);

        if (nextWaveTime <= gameTime)
        {
            MoveToNextWave();
        }

    }


    public void WaveStart(int index)
    {
        //스테이지 시작
        LoadWaveFromJson(index);

        //UI초기화
        waveProgress.InitializeWaveProgress(stage, startTime);
        Invoke("MovePlayerIcon", 0.01f);
        StartCoroutine(ShowWaveUIRoutine());

        //변수 리셋
        ResetWaveManager();

        //소환될 적의 수 미리 계산
        for (int i = 0; i < stage.waves.Count; i++)
        {
            for (int j = 0; j < stage.waves[i].spawns.Count; j++)
            {
                enemyRegularSpawned += stage.waves[i].spawns[j].spawnCount;
            }
        }

    }


    void LoadWaveFromJson(int stageIndex)
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string[] words = sceneName.Split('_');  // words[0] = Stage, words[1] = 챕터index, words[2] = 스테이지 index

        string address = "Stage/" + words[0] + words[1] + "/" + stageIndex + ".json";
        stage = LoadManager.Load<Stage>(address);       
    }

    IEnumerator ShowWaveUIRoutine()
    {
        wavePanelUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        wavePanelUI.gameObject.SetActive(false);
    }

    void MovePlayerIcon()
    {
        wavePanelUI.MovePlayericon(stageManager.CurrStageIndex);
    }
    void ResetWaveManager()
    {
        gameTime = 0;
        nextWaveTime = startTime;
        waveIndex = 0;
        spawnRoutine = null;
        activateStage = true;
        FinalWave = false;
    }


    //캐릭터 행성 업데이트? (몬스터 스폰 장소)
    void PlayerPlanetUpdate()
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

    //몬스터가 죽을 때마다 실행. (현재는 실행x -> 시간제한을 두고 적을 처리하게 함.)
    public void CountEnemyLeft(GameObject obj)
    {
        if(spawnedEnemyList.Contains(obj))
        {
            spawnedEnemyList.Remove(obj);
            enemyLeftInWave--;
            enemyTotalKilled++;

            //UI 조절
            minimapManager.RemoveMonster(obj);

            ////적들이 비율밑으로 감소하면 웨이브 조기 클리어
            //if ((float)enemyLeftInWave / enemySpawnedInWave < waveClearRatio)
            //{
            //    MoveToNextWave();
            //}

            //카운트
            int index = waveIndex - 1;
            float clearTime = gameTime - pastWaveTime;
            float timeLeft = nextWaveTime - gameTime;
            if(enemyLeftInWave <= 0)
            {
                Debug.Log("스테이지 " + index + "클리어타임 : " + clearTime + " , 잔여시간 : " + timeLeft);
                if (FinalWave)
                {
                    MoveToNextWave();
                }
                else if (nextWaveTime - gameTime > 4.0f)
                {
                    gameTime = nextWaveTime - 4.0f;
                }
            }
        }
    }


    float pastWaveTime = 0f;
    //웨이브(일반, 보스)를 무사히 스폰하면 true 반환, 스폰할 웨이브가 없음면 false 반환.
    private void MoveToNextWave()
    {
        Debug.Log("소환 시작 : " + waveIndex.ToString());

        if(waveIndex < stage.waves.Count - 1)
        {
            //기본 웨이브
            Wave wave = stage.waves[waveIndex];
            SpawnWave(wave);

            pastWaveTime = nextWaveTime;
            waveIndex++;
            nextWaveTime += wave.totalTime;
            Debug.Log("기본 웨이브");

        }
        else if(waveIndex == stage.waves.Count - 1)
        {
            //마지막 웨이브
            Wave wave = stage.waves[waveIndex];
            SpawnWave(wave);

            pastWaveTime = nextWaveTime;
            waveIndex++;
            nextWaveTime += wave.totalTime;

            FinalWave = true;
            Debug.Log("마지막 웨이브");
        }
        else
        {
            //웨이브 종료
            StageClear();
            return;
        }

    }

    //이 스테이지 클리어 시 실행. 보스를 잡는 경우, 혹은 (보스가 없는 경우에는) 맵에 있는 모든 몬스터를 제거했을 때 실행.
    void StageClear()
    {
        Debug.Log("스테이지 클리어");
        activateStage = false;

        //다음 스테이지가 가능하다면
        if (stageManager.IsNextStageAvailable())
        {
            //다음 스테이지로 이동
            StartCoroutine(NextStageRoutine());
            Debug.Log("다음 스테이지로 이동");

        }
        else
        {
            //챕터 클리어! 
            StartCoroutine(ClearRoutine());
            Debug.Log("챕터 클리어");

        }

    }

    IEnumerator NextStageRoutine()
    {
        yield return new WaitForSeconds(2f);
        stageManager.FinishStage();
    }

    IEnumerator ClearRoutine()
    {
        MonsterDisapper();
        StartCoroutine(GameManager.Instance.ShowStageEndUI(2f, 3f));
        yield return new WaitForSeconds(5f);
        GameManager.Instance.TransitionFadeOut(true);
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.ChapterClear();
    }

    #region Spawns

    //소환된 몬스터들을 돌려보낸다. 
    void MonsterDisapper()
    {
        Debug.Log("몬스터 제거");
        if (MonsterDisappearEvent != null) MonsterDisappearEvent();

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
                GameObject prefab = GameManager.Instance.poolManager.GetPoolObj(enemyPrefab,3);
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


    public bool GetSafePointFromOutsideScreen(out Vector2 safePoint)
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
    public void GetStagePlanets(Transform parentTr)
    {
        Planet[] planets = parentTr.GetComponentsInChildren<Planet>();
        planetList.Clear();
        
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
    public bool isBossWave;
    public List<Wave> waves = new List<Wave>();
}

