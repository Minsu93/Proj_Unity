using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
    


    //행성 관련
    [SerializeField] List<Planet> planetList = new List<Planet>();

    [Header("UI")]
    //스크립트.
    WaveObjectRespawner objectRespawner;

    [Header("Wave UI")]
    //Wave UI관련
    [SerializeField] GameObject waveCanvas;
    WaveProgress waveProgress;
    WavePanelUI stageOrderUI;
    [SerializeField] int totalStageCount = 5;


    BossSpawner bossSpawner;

    public event System.Action MonsterDisappearEvent;
    public event System.Action WaveClear;


    private void Awake()
    {
        instance = this;

        bossSpawner = GetComponent<BossSpawner>();
        objectRespawner = GetComponent<WaveObjectRespawner>();

        //웨이브 UI 생성
        GameObject waveUIObj =  Instantiate(waveCanvas);
        stageOrderUI = waveUIObj.transform.GetComponentInChildren<WavePanelUI>();
        stageOrderUI.SetStageIcons(stageManager.MaxStage);
        stageOrderUI.gameObject.SetActive(false);

        waveProgress = waveUIObj.GetComponentInChildren<WaveProgress>();
    }



    private void Update()
    {
        if (!activateStage) return;

        gameTime += Time.deltaTime;

        //플레이어 행성 업데이트 -> 추격 용도
        PlayerPlanetUpdate();

        //Stage 오브젝트 리스폰
        objectRespawner.UpdateSpawner();

        //최종 웨이브인 경우 이후는 업데이트 하지 않는다. 
        if (FinalWave || stage.isBossWave) return;

        //웨이브 UI 아이콘 (추후 제거예정)
        waveProgress.IconSpawner(gameTime);

        //다음 웨이브 시간이 되면 웨이브 소환.
        if (nextWaveTime <= gameTime)
        {
            MoveToNextWave();
        }
    }

    #region WaveManager Function

    public void WaveStart(int index)
    {
        //스테이지 시작
        LoadWaveFromJson(index);

        //UI초기화
        waveProgress.InitializeWaveProgress(stage, startTime);
        //변수 리셋
        ResetWaveManager();

        //스테이지 순서 UI 실행. BossWave 시 보스 스폰.
        Invoke("StageOrderUIMovePlayerIcon", 0.01f);
        StartCoroutine(ShowWaveUIRoutine());


        if (stage.isBossWave) return;

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
        stageOrderUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        stageOrderUI.gameObject.SetActive(false);

        //보스 웨이브인 경우
        if (stage.isBossWave)
        {
            yield return new WaitForSeconds(1f);

            //보스 생성 
            string bossName = stage.waves[0].spawns[0].enemy.name;
            bossSpawner.SpawnBoss(bossName);
        }
    }

    void StageOrderUIMovePlayerIcon()
    {
        stageOrderUI.MovePlayericon(stageManager.CurrStageIndex);
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


    public void CountEnemyLeft(GameObject obj)
    {
        if(spawnedEnemyList.Contains(obj))
        {
            spawnedEnemyList.Remove(obj);
            enemyLeftInWave--;
            enemyTotalKilled++;

            //UI 조절
            GameManager.Instance.arrowManager.RemoveArrow(obj, 0);

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

    float pastWaveTime = 0f;    //디버그용
    private void MoveToNextWave()
    {
        if(waveIndex < stage.waves.Count - 1)
        {
            //기본 웨이브
            Wave wave = stage.waves[waveIndex];
            SpawnWave(wave);

            pastWaveTime = nextWaveTime;
            waveIndex++;
            nextWaveTime += wave.totalTime;

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
        }
        else
        {
            //웨이브 종료
            StageClear();
            return;
        }

    }

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
    }



    IEnumerator NextStageRoutine()
    {
        yield return new WaitForSeconds(2f);
        stageManager.FinishStage();
    }

    #endregion



    #region Spawns

    //소환된 몬스터들을 돌려보낸다. 
    public void MonsterDisapper()
    {
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
        GameObject enemyPrefab = GameManager.Instance.monsterDictonary.monsDictionary[enemy.name];

        for (int i = 0; i < count; i++)
        {
            //안전한 스폰 포인트 반환
            GetSafePointFromOutsideScreen(out Vector2 safePoint);

            GameObject monster = GameManager.Instance.poolManager.GetPoolObj(enemyPrefab, 3);
            monster.transform.position = safePoint;
            monster.transform.rotation = Quaternion.identity;
            EnemyAction act = monster.GetComponent<EnemyAction>();

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
                    strikePos = GetRandomPointNearPlayer(minAirDistance, maxAirDistance);
                    break;
            }
            //Strike시작
            monster.GetComponent<EnemyAction>().EnemyStartStrike(strikePos, true);


            //소환된 적 리스트에 추가.
            spawnedEnemyList.Add(monster);

            //소환된 적 UI에 추가
            GameManager.Instance.arrowManager.CreateArrow(monster, 0);


            yield return new WaitForSeconds(enemy.delay);
        }

    }

    //보스가 적 소환할 때 > 킬 카운트에 포함이 안되게 소환.
    public void SpawnObjects(string enemyName, int count)
    {
        GameObject enemyPrefab = GameManager.Instance.monsterDictonary.monsDictionary[enemyName];

        for (int i = 0; i < count; i++)
        {
            //안전한 스폰 포인트 반환
            GetSafePointFromOutsideScreen(out Vector2 safePoint);

            GameObject monster = GameManager.Instance.poolManager.GetPoolObj(enemyPrefab, 3);
            monster.transform.position = safePoint;
            monster.transform.rotation = Quaternion.identity;
            EnemyAction act = monster.GetComponent<EnemyAction>();

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
                    strikePos = GetRandomPointNearPlayer(minAirDistance, maxAirDistance);
                    break;
            }
            //Strike시작
            monster.GetComponent<EnemyAction>().EnemyStartStrike(strikePos, false);

            //소환된 적 UI에 추가
            GameManager.Instance.arrowManager.CreateArrow(monster, 0);

        }
    }

    public void GetSafePointFromOutsideScreen(out Vector2 safePoint)
    {
        //스크린 > 월드 좌표 측정. z = 10 은 카메라와의 거리.
        Vector3 screenSizeMin = new Vector3(0, 0, 10);
        Vector3 screenSizeMax = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight,10);
        screenSizeMin = Camera.main.ScreenToWorldPoint(screenSizeMin);
        screenSizeMax = Camera.main.ScreenToWorldPoint(screenSizeMax);

        safePoint = Vector2.zero;
        Vector2 vec;

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

        safePoint =  FindNearestNode(vec);
    }

    //private bool IsLocationSafe(Vector2 position)
    //{
    //    return Physics2D.OverlapCircle(position, 2f, LayerMask.GetMask("Planet")) == null;
    //}

    //플레이어 주변의 랜덤한 공중 포인트를 불러온다. 
    public Vector2 GetRandomPointNearPlayer(float minDist, float maxDist)
    {
        //플레이어 위치에서 2~10 사이의 랜덤 포인트를 가져온다. 
        float radius = UnityEngine.Random.Range(minDist, maxDist);
        Vector2 dir = UnityEngine.Random.insideUnitCircle;
        Vector2 pos = (Vector2)GameManager.Instance.player.position + (dir.normalized * radius);
        return FindNearestNode(pos); 
    }

    Vector2 FindNearestNode(Vector2 point)
    {
        //가장 가까운 (걸을 수 있는)노드를 가져온다
        var constraint = NNConstraint.None;

        // Constrain the search to walkable nodes only
        constraint.constrainWalkability = true;
        constraint.walkable = true;

        GraphNode node = AstarPath.active.GetNearest((Vector3)point, constraint).node;
        if (node != null)
        {
            return (Vector3)node.position;
        }
        else return Vector2.zero;
    }

    #endregion



    #region Planet
    int[,] planetEachDists;
    List<int[]> wayListFromXToY = new List<int[]>();

    //게임 시작 시 맵 전체의 행성들을 불러온다
    public void AddStagePlanets(Transform parentTr)
    {
        Planet[] planets = parentTr.GetComponentsInChildren<Planet>();
        planetList.Clear();
        wayListFromXToY.Clear();


        foreach (Planet planet in planets)
        {
            planetList.Add(planet);
        }

        //행성 별 거리 집합을 생성한다. 
        planetEachDists = new int[planetList.Count, planetList.Count];
        for(int i = 0; i < planetList.Count; i++)
        {

            for(int j = 0; j<planetList.Count; j++)
            {
                planetEachDists[i, j] = Int32.MaxValue;

                if (i == j)
                    planetEachDists[i, j] = 0;
            }

            List<PlanetBridge> bridges = planetList[i].linkedPlanetList;
            foreach(PlanetBridge pb in bridges)
            {
                int index = planetList.FindIndex(planet => planet == pb.planet);
                planetEachDists[i, index] = pb.planetDistance;
            }
        }

        //모든 Planet에서의 way 계산을 미리 해둔다.
        for( int i = 0; i < planetList.Count; i++)
        {
            wayListFromXToY.Add(Dijikstra(i));
        }
    }

    //from 출발지에서부터 다른 행성으로 움직이는 이동 방향 계산을 미리 해둔다.
    int[] Dijikstra(int from)
    {
        int[] minDist = new int[planetList.Count];
        bool[] visited = new bool[planetList.Count];
        int[] parents = new int[planetList.Count];
        Array.Fill(parents, -1);

        //시작 위치 세팅
        int startIndex = from;
        for(int i = 0; i < planetList.Count; i++)
        {
            minDist[i] = planetEachDists[startIndex, i];
            if (minDist[i] != 0 || minDist[i] != Int32.MaxValue)
            {
                parents[i] = startIndex;
            }
        }
        visited[startIndex] = true;
        parents[startIndex] = startIndex;

        //1. visited도 아니고 0이 아닌 최소값을 구합니다
        int min= Int32.MaxValue;
        int now = -1;
        for(int j = 0; j < planetList.Count; j++)
        {
            if (visited[j]) continue;
            if (minDist[j] < min)
            {
                min = minDist[j];
                now = j;
            }
        }

        //now가 -1이면 모두 실패.
        if(now == -1)
        {
            Debug.LogWarning("No way to Jump");
            return parents;
        }
        //else
        //{
        //    parents[now] = startIndex;
        //}

        //2. 다음 now로 이동합니다. 
        while (true)
        {
            int currIndex = now;
            now = -1;
            visited[currIndex] = true;
            int preDist = minDist[currIndex];

            for (int i = 0; i < planetList.Count; i++)
            {
                //이미 지나온 장소면 취소한다. 
                if (visited[i]) continue;
                //해당 장소에서 갈 수 없는 index는 제외한다.
                if (planetEachDists[currIndex, i] == Int32.MaxValue) continue;

                //currIndex를 경유하는 최소거리가 기존 거리보다 낮다면 갱신한다. 
                if (minDist[i] > planetEachDists[currIndex, i] + preDist)
                {
                    minDist[i] = planetEachDists[currIndex, i] + preDist;
                    //최소 거리가 갱신이 된다면, 현재슬롯(i)의 갱신위치는 방문한 장소(currIndex)이다.
                    parents[i] = currIndex;
                }
            }

            //값 중 최소값 구하기.
            min = Int32.MaxValue;
            for (int i = 0; i<planetList.Count; i++)
            {
                if (visited[i]) continue;
                if (minDist[i] < min)
                {
                    min = minDist[i];
                    now = i;
                }
            }

            //모두 방문했으면 종료.
            if (now == -1) break;
        }

        return parents;
    }

    //만들어둔 wayList를 기반으로 목적지 Planet까지 최단 거리의 Planet Stack을 만든다. 
    public List<Planet> GetWays(Planet from, Planet to)
    {
        //변수 준비
        List<Planet> wayList = new List<Planet>();
        Stack<Planet> wayStack = new Stack<Planet>();
        //string debugStr = "";
        int fromInt = planetList.IndexOf(from);
        int toInt = planetList.IndexOf(to);
        //int fromInt = planetList.FindIndex(planet => planet.gameObject == from.gameObject);
        //int toInt = planetList.FindIndex(planet => planet.gameObject == to.gameObject);

        //리스트에 없는 행성이라면 false를 리턴한다.
        if (fromInt < 0 || toInt < 0)
        {
            return wayList;
        }

        //wayStack에 Planet을 목표행성부터 거꾸로 집어넣는다. (목표행성 > 이전 행성 > 이전2 행성...)
        int[] parents = wayListFromXToY[fromInt];
        //이어져 있지 않은 행성인 경우 
        if (parents[toInt] == -1) return wayList;

        while (toInt != fromInt)
        {
            Debug.Log(toInt);
            wayStack.Push(planetList[toInt]);
            toInt = parents[toInt];
        }
        //출발 행성 추가
        if(toInt == fromInt)
            wayStack.Push(planetList[fromInt]);

        //waystack의 내용을 pop해서 wayList에 넣는다. (...이전2 행성 > 이전 행성 > 목표 행성 (반대 순서))
        int count = wayStack.Count;
        for (int i = 0; i < count; i++)
        {
            wayList.Add(wayStack.Pop());
        }

        return wayList;
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
            GameManager.Instance.playerManager.playerNearestPlanet = SelectClosesetPlanetFromScreen(GameManager.Instance.player.position);
        }
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
    private void OnDrawGizmos()
    {
        if (planetList.Count > 0)
        {
            for(int i =0; i< planetList.Count; i++)
            {
                Gizmos.color = Color.green;
                Handles.Label(planetList[i].transform.position, i.ToString());
            }
        }
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

