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
    float nextWaveTime; //���̺� ����ñ��� ���� �ð�
    float gameTime; //���� �ð�

    Stage stage;
    int waveIndex;
    bool activateStage;  //���̺� Ȱ��ȭ ����
    bool FinalWave = false;  //���� ���̺� �����߳���?
    Coroutine spawnRoutine;

    //���̺�
    int enemyLeftInWave;  //�������� ��ŵ�� ���� ���� �����ϴ� ���� �� ��, ���� ������ �پ��.
    int enemySpawnedInWave;   //�������� ��ŵ�� ���� ���� �����ϴ� ���� �� ��  
    //��������
    int enemyTotalSpawned;  //���� ���ۺ��� Ŭ������� ������ ���� ��. (�ʿ�x)
    int enemyRegularSpawned; //���� ��ȹ��θ� �����Ǿ��� ���� ��. ���ʽ� ������ ���� �������� �ʴ´�. 
    int enemyTotalKilled;   //��ü ���� ������ ��. ���ʽ� ����. 
    //���� 
    List<GameObject> spawnedEnemyList = new List<GameObject>(); //���� �����Ǿ��ִ� ���� ����Ʈ

    [Header("SpawnBox")]
    public float spawnRange = 5f;                  //���� ���� ����. 
    public float distanceUnitFromScreen = 5f;      //ȭ�鿡�� ������ ����. ���� ����.
    float minAirDistance = 5f;  //�÷��̾� �ֺ� �ּ� �Ÿ�
    float maxAirDistance = 10f; //Air Ÿ�� ���� ���� ������ �÷��̾� �ֺ� �ִ� �Ÿ�.

    float timer;
    float updateCycle = 0.5f;
    


    //�༺ ����
    [SerializeField] List<Planet> planetList = new List<Planet>();

    [Header("UI")]
    //��ũ��Ʈ.
    WaveObjectRespawner objectRespawner;

    [Header("Wave UI")]
    //Wave UI����
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

        //���̺� UI ����
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

        //�÷��̾� �༺ ������Ʈ -> �߰� �뵵
        PlayerPlanetUpdate();

        //Stage ������Ʈ ������
        objectRespawner.UpdateSpawner();

        //���� ���̺��� ��� ���Ĵ� ������Ʈ ���� �ʴ´�. 
        if (FinalWave || stage.isBossWave) return;

        //���̺� UI ������ (���� ���ſ���)
        waveProgress.IconSpawner(gameTime);

        //���� ���̺� �ð��� �Ǹ� ���̺� ��ȯ.
        if (nextWaveTime <= gameTime)
        {
            MoveToNextWave();
        }
    }

    #region WaveManager Function

    public void WaveStart(int index)
    {
        //�������� ����
        LoadWaveFromJson(index);

        //UI�ʱ�ȭ
        waveProgress.InitializeWaveProgress(stage, startTime);
        //���� ����
        ResetWaveManager();

        //�������� ���� UI ����. BossWave �� ���� ����.
        Invoke("StageOrderUIMovePlayerIcon", 0.01f);
        StartCoroutine(ShowWaveUIRoutine());


        if (stage.isBossWave) return;

        //��ȯ�� ���� �� �̸� ���
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
        string[] words = sceneName.Split('_');  // words[0] = Stage, words[1] = é��index, words[2] = �������� index

        string address = "Stage/" + words[0] + words[1] + "/" + stageIndex + ".json";
        stage = LoadManager.Load<Stage>(address);       
    }

    IEnumerator ShowWaveUIRoutine()
    {
        stageOrderUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        stageOrderUI.gameObject.SetActive(false);

        //���� ���̺��� ���
        if (stage.isBossWave)
        {
            yield return new WaitForSeconds(1f);

            //���� ���� 
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

            //UI ����
            GameManager.Instance.arrowManager.RemoveArrow(obj, 0);

            ////������ ���������� �����ϸ� ���̺� ���� Ŭ����
            //if ((float)enemyLeftInWave / enemySpawnedInWave < waveClearRatio)
            //{
            //    MoveToNextWave();
            //}

            //ī��Ʈ
            int index = waveIndex - 1;
            float clearTime = gameTime - pastWaveTime;
            float timeLeft = nextWaveTime - gameTime;
            if(enemyLeftInWave <= 0)
            {
                Debug.Log("�������� " + index + "Ŭ����Ÿ�� : " + clearTime + " , �ܿ��ð� : " + timeLeft);
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

    float pastWaveTime = 0f;    //����׿�
    private void MoveToNextWave()
    {
        if(waveIndex < stage.waves.Count - 1)
        {
            //�⺻ ���̺�
            Wave wave = stage.waves[waveIndex];
            SpawnWave(wave);

            pastWaveTime = nextWaveTime;
            waveIndex++;
            nextWaveTime += wave.totalTime;

        }
        else if(waveIndex == stage.waves.Count - 1)
        {
            //������ ���̺�
            Wave wave = stage.waves[waveIndex];
            SpawnWave(wave);

            pastWaveTime = nextWaveTime;
            waveIndex++;
            nextWaveTime += wave.totalTime;

            FinalWave = true;
        }
        else
        {
            //���̺� ����
            StageClear();
            return;
        }

    }

    void StageClear()
    {
        Debug.Log("�������� Ŭ����");
        activateStage = false;

        //���� ���������� �����ϴٸ�
        if (stageManager.IsNextStageAvailable())
        {
            //���� ���������� �̵�
            StartCoroutine(NextStageRoutine());
            Debug.Log("���� ���������� �̵�");
        }
    }



    IEnumerator NextStageRoutine()
    {
        yield return new WaitForSeconds(2f);
        stageManager.FinishStage();
    }

    #endregion



    #region Spawns

    //��ȯ�� ���͵��� ����������. 
    public void MonsterDisapper()
    {
        if (MonsterDisappearEvent != null) MonsterDisappearEvent();

        //UI ����
        spawnedEnemyList.Clear();
        enemyLeftInWave = 0;
        //leftEnemyCountText.text = enemyLeft.ToString();
    }

    void SpawnWave(Wave wave)
    {
        //��ƾ� �� �� ���� ���� ���� ī��Ʈ �մϴ�. 
        //�����ִ� �� + ���� ������ ��.
        int newEnemyCount = 0;
        //���� ���� ���� ������Ʈ�մϴ�. 
        foreach (Spawn spawn in wave.spawns)
        {
            newEnemyCount += spawn.spawnCount;
            enemyTotalSpawned += spawn.spawnCount;
        }
        enemySpawnedInWave = newEnemyCount + spawnedEnemyList.Count;
        enemyLeftInWave = enemySpawnedInWave;

        //���̺긦 �����մϴ�. 
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnInOrder(wave.spawns));
    }
   
    #endregion



    #region Spawn Logics 

    //Spawns�� ����ִ� Spawn���� ������� ��ȯ�Ѵ�. 
    IEnumerator SpawnInOrder(List<Spawn> spawns)
    {
        foreach(Spawn spawn in spawns)
        {
            StartCoroutine(SpawnObjects(spawn.enemy, spawn.spawnCount));
            yield return new WaitForSeconds(spawn.delayToNextWave);
        }
    }

    //enemies�� ����ִ� ������ ������� �����Ѵ�. 
    IEnumerator SpawnObjects(Enemy enemy, int count)
    {
        GameObject enemyPrefab = GameManager.Instance.monsterDictonary.monsDictionary[enemy.name];

        for (int i = 0; i < count; i++)
        {
            //������ ���� ����Ʈ ��ȯ
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
                    //������ �������� ���� �����̿� �ִ� �༺���� �̵���Ų��. 
                    Planet planet = SelectClosesetPlanetFromScreen(safePoint);
                    int strikePointIndex = planet.GetClosestIndex(transform.position);
                    strikePos = planet.worldPoints[strikePointIndex];
                    break;

                case EnemyType.Air:
                    //������ ĳ���� �ֺ� �������� �̵���Ų��. 
                    strikePos = GetRandomPointNearPlayer(minAirDistance, maxAirDistance);
                    break;
            }
            //Strike����
            monster.GetComponent<EnemyAction>().EnemyStartStrike(strikePos, true);


            //��ȯ�� �� ����Ʈ�� �߰�.
            spawnedEnemyList.Add(monster);

            //��ȯ�� �� UI�� �߰�
            GameManager.Instance.arrowManager.CreateArrow(monster, 0);


            yield return new WaitForSeconds(enemy.delay);
        }

    }

    //������ �� ��ȯ�� �� > ų ī��Ʈ�� ������ �ȵǰ� ��ȯ.
    public void SpawnObjects(string enemyName, int count)
    {
        GameObject enemyPrefab = GameManager.Instance.monsterDictonary.monsDictionary[enemyName];

        for (int i = 0; i < count; i++)
        {
            //������ ���� ����Ʈ ��ȯ
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
                    //������ �������� ���� �����̿� �ִ� �༺���� �̵���Ų��. 
                    Planet planet = SelectClosesetPlanetFromScreen(safePoint);
                    int strikePointIndex = planet.GetClosestIndex(transform.position);
                    strikePos = planet.worldPoints[strikePointIndex];
                    break;

                case EnemyType.Air:
                    //������ ĳ���� �ֺ� �������� �̵���Ų��. 
                    strikePos = GetRandomPointNearPlayer(minAirDistance, maxAirDistance);
                    break;
            }
            //Strike����
            monster.GetComponent<EnemyAction>().EnemyStartStrike(strikePos, false);

            //��ȯ�� �� UI�� �߰�
            GameManager.Instance.arrowManager.CreateArrow(monster, 0);

        }
    }

    public void GetSafePointFromOutsideScreen(out Vector2 safePoint)
    {
        //��ũ�� > ���� ��ǥ ����. z = 10 �� ī�޶���� �Ÿ�.
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

    //�÷��̾� �ֺ��� ������ ���� ����Ʈ�� �ҷ��´�. 
    public Vector2 GetRandomPointNearPlayer(float minDist, float maxDist)
    {
        //�÷��̾� ��ġ���� 2~10 ������ ���� ����Ʈ�� �����´�. 
        float radius = UnityEngine.Random.Range(minDist, maxDist);
        Vector2 dir = UnityEngine.Random.insideUnitCircle;
        Vector2 pos = (Vector2)GameManager.Instance.player.position + (dir.normalized * radius);
        return FindNearestNode(pos); 
    }

    Vector2 FindNearestNode(Vector2 point)
    {
        //���� ����� (���� �� �ִ�)��带 �����´�
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

    //���� ���� �� �� ��ü�� �༺���� �ҷ��´�
    public void AddStagePlanets(Transform parentTr)
    {
        Planet[] planets = parentTr.GetComponentsInChildren<Planet>();
        planetList.Clear();
        wayListFromXToY.Clear();


        foreach (Planet planet in planets)
        {
            planetList.Add(planet);
        }

        //�༺ �� �Ÿ� ������ �����Ѵ�. 
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

        //��� Planet������ way ����� �̸� �صд�.
        for( int i = 0; i < planetList.Count; i++)
        {
            wayListFromXToY.Add(Dijikstra(i));
        }
    }

    //from ������������� �ٸ� �༺���� �����̴� �̵� ���� ����� �̸� �صд�.
    int[] Dijikstra(int from)
    {
        int[] minDist = new int[planetList.Count];
        bool[] visited = new bool[planetList.Count];
        int[] parents = new int[planetList.Count];
        Array.Fill(parents, -1);

        //���� ��ġ ����
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

        //1. visited�� �ƴϰ� 0�� �ƴ� �ּҰ��� ���մϴ�
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

        //now�� -1�̸� ��� ����.
        if(now == -1)
        {
            Debug.LogWarning("No way to Jump");
            return parents;
        }
        //else
        //{
        //    parents[now] = startIndex;
        //}

        //2. ���� now�� �̵��մϴ�. 
        while (true)
        {
            int currIndex = now;
            now = -1;
            visited[currIndex] = true;
            int preDist = minDist[currIndex];

            for (int i = 0; i < planetList.Count; i++)
            {
                //�̹� ������ ��Ҹ� ����Ѵ�. 
                if (visited[i]) continue;
                //�ش� ��ҿ��� �� �� ���� index�� �����Ѵ�.
                if (planetEachDists[currIndex, i] == Int32.MaxValue) continue;

                //currIndex�� �����ϴ� �ּҰŸ��� ���� �Ÿ����� ���ٸ� �����Ѵ�. 
                if (minDist[i] > planetEachDists[currIndex, i] + preDist)
                {
                    minDist[i] = planetEachDists[currIndex, i] + preDist;
                    //�ּ� �Ÿ��� ������ �ȴٸ�, ���罽��(i)�� ������ġ�� �湮�� ���(currIndex)�̴�.
                    parents[i] = currIndex;
                }
            }

            //�� �� �ּҰ� ���ϱ�.
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

            //��� �湮������ ����.
            if (now == -1) break;
        }

        return parents;
    }

    //������ wayList�� ������� ������ Planet���� �ִ� �Ÿ��� Planet Stack�� �����. 
    public List<Planet> GetWays(Planet from, Planet to)
    {
        //���� �غ�
        List<Planet> wayList = new List<Planet>();
        Stack<Planet> wayStack = new Stack<Planet>();
        //string debugStr = "";
        int fromInt = planetList.IndexOf(from);
        int toInt = planetList.IndexOf(to);
        //int fromInt = planetList.FindIndex(planet => planet.gameObject == from.gameObject);
        //int toInt = planetList.FindIndex(planet => planet.gameObject == to.gameObject);

        //����Ʈ�� ���� �༺�̶�� false�� �����Ѵ�.
        if (fromInt < 0 || toInt < 0)
        {
            return wayList;
        }

        //wayStack�� Planet�� ��ǥ�༺���� �Ųٷ� ����ִ´�. (��ǥ�༺ > ���� �༺ > ����2 �༺...)
        int[] parents = wayListFromXToY[fromInt];
        //�̾��� ���� ���� �༺�� ��� 
        if (parents[toInt] == -1) return wayList;

        while (toInt != fromInt)
        {
            Debug.Log(toInt);
            wayStack.Push(planetList[toInt]);
            toInt = parents[toInt];
        }
        //��� �༺ �߰�
        if(toInt == fromInt)
            wayStack.Push(planetList[fromInt]);

        //waystack�� ������ pop�ؼ� wayList�� �ִ´�. (...����2 �༺ > ���� �༺ > ��ǥ �༺ (�ݴ� ����))
        int count = wayStack.Count;
        for (int i = 0; i < count; i++)
        {
            wayList.Add(wayStack.Pop());
        }

        return wayList;
    }

    //��ũ���� ���̴� �༺ �� �ϳ��� ����. 
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

    //��ũ���� ���̴��� �˻��Ѵ�. 
    public bool PlanetBoundIsInScreen(PolygonCollider2D polyCollider)
    {
        Bounds bounds = polyCollider.bounds;
        float margin = 150f;     //�༺ ������ ���� ��ũ�� ����


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


    //ĳ���� �༺ ������Ʈ? (���� ���� ���)
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



//������ �̸��� ���� ���� ������ ������
[Serializable]
public class Enemy
{
    public string name;
    public float delay; //���� ������ �ð� ����.
}

//���� ���̺갡 �󸶳� ���ӵǴ°�. 
[Serializable]
public class Spawn
{
    public Enemy enemy;
    public int spawnCount;
    public float delayToNextWave; //���� ���� ���� ���� �ð�
}

//�ѹ��� Wave�� �������� Spawn���� ������ 
[Serializable]
public class Wave
{
    public int waveIndex;
    public List<Spawn> spawns = new List<Spawn>();
    public float totalTime; //���� ���̺� ���� �ð� ����
}

[Serializable]
public class Stage
{
    public bool isBossWave;
    public List<Wave> waves = new List<Wave>();
}

