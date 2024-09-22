using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    //public string stageName = "stage0";
    //[SerializeField] private StagePortal portal;
    
    [Header("Wave Property")]
    [SerializeField] float waveClearBonusSubtractTime = 5.0f;
    float nextWaveTime; //���̺� ����ñ��� ���� �ð�
    float gameTime; //���� �ð�

    Stage stage;
    int stageIndex;
    bool activatedWave;  //���̺� Ȱ��ȭ ����
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
    public Planet playerNearestPlanet { get; set; } //�÷��̾�� ���� ����� �༺�� �����Ѵ�. null�� ������ �ʴ� �׽� ���� ����� �༺�� ǥ���Ѵ�. ������ ���� �뵵�� ����Ѵ�. 


    //�༺ ����
    [SerializeField] List<Planet> planetList = new List<Planet>();
     

    [Header("UI")]
    //��ũ��Ʈ.
    Dictionary<string, GameObject> monsterDict;
    WaveObjectRespawner objectRespawner;

    [Header("Wave UI")]
    //Wave UI����
    [SerializeField] Image waveProgressImg;
    [SerializeField] TextMeshProUGUI currentWaveText;
    [SerializeField] TextMeshProUGUI totalWaveText;
    [SerializeField] ArrowIndicatorManager minimapManager;
    [SerializeField] GameObject MonsterIcon;
    [SerializeField] GameObject BossIcon;

    //WaveUI���� ����
    [SerializeField] RectTransform iconParent;
    [SerializeField] float iconSpawnTime;
    [SerializeField] float maxSpawnLimit = 60f;
    [SerializeField] float spawnPosMultiplier = 10f;
    int spawnIndex = 0;
    Dictionary<RectTransform, float> IconPairs = new Dictionary<RectTransform, float>();
    List<RectTransform> keyToRemove = new List<RectTransform>();

    public event System.Action MonsterDisappearEvent;
    public event System.Action WaveClear;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Dictionary �ҷ�����
        monsterDict = GameManager.Instance.monsterDictonary.monsDictionary;
        objectRespawner = GetComponent<WaveObjectRespawner>();  

        //�������� ����
        LoadWaveFromJson();
    }

    void LoadWaveFromJson()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        string[] words = sceneName.Split('_');  // words[0] = Stage, words[1] = é��index, words[2] = �������� index

        string address = "Stage/" + words[0] + words[1] + "/" + words[2] + ".json";
        stage = LoadManager.Load<Stage>(address);

        //string path = Path.Combine(Application.dataPath + "/Data/Stage/", filename + ".json");
        //Debug.Log(path);
        //string loadJson = File.ReadAllText(path);
        //stage = JsonUtility.FromJson<Stage>(loadJson);
    }

    void ResetWaveManager()
    {
        gameTime = 0;
        nextWaveTime = stage.startTime;
        iconSpawnTime = stage.startTime;
        stageIndex = 0;
        spawnRoutine = null;

        StopAllCoroutines();
    }

    public void WaveStart()
    {
        ResetWaveManager();

        //��ȯ�� ���� �� �̸� ���
        for (int i = 0; i < stage.waves.Count; i++)
        {
            for (int j = 0; j < stage.waves[i].spawns.Count; j++)
            {
                enemyRegularSpawned += stage.waves[i].spawns[j].spawnCount;
            }
        }
        ////UI�� ����
        currentWaveText.text = "0";
        totalWaveText.text = stage.waves.Count.ToString();

        activatedWave = true;
        GetStagePlanets();
    }

    private void Update()
    {
        if (!activatedWave) return;

        gameTime += Time.deltaTime;

        //UI������ ���� 
        IconSpawner();
        IconMover();

        //���� ���̺��
        if (nextWaveTime <= gameTime)
        {
            MoveToNextWave();
        }

        PlayerPlanetUpdate();

        //Stage ������Ʈ ������
        objectRespawner.UpdateSpawner();
    }

    #region ���̺� ������ ���� 
    void IconSpawner()
    {
        //icon����
        if (iconSpawnTime - gameTime < maxSpawnLimit)
        {
            if (spawnIndex < stage.waves.Count)
            {
                //���� �ð��� ������ ����
                GameObject obj = Instantiate(MonsterIcon, iconParent.transform);
                RectTransform rect = obj.GetComponent<RectTransform>();
                IconPairs.Add(rect, iconSpawnTime);
                //���� �ð� ����
                iconSpawnTime += stage.waves[spawnIndex].totalTime;
                spawnIndex++;
            }
            else
            {
                //���� ������ �߰�
                GameObject bossobj = Instantiate(BossIcon, iconParent.transform);
                RectTransform bossRect = bossobj.GetComponent<RectTransform>();
                IconPairs.Add(bossRect, iconSpawnTime);
                iconSpawnTime = float.MaxValue; //���̻� �������� ���ϵ���
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

    #endregion

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
            playerNearestPlanet = SelectClosesetPlanetFromScreen(GameManager.Instance.player.position);
        }
    }

    //���Ͱ� ���� ������ ����. (����� ����x -> �ð������� �ΰ� ���� ó���ϰ� ��.)
    public void CountEnemyLeft(GameObject obj)
    {
        if(spawnedEnemyList.Contains(obj))
        {
            spawnedEnemyList.Remove(obj);
            enemyLeftInWave--;
            enemyTotalKilled++;

            //UI ����
            minimapManager.RemoveMonster(obj);

            ////������ ���������� �����ϸ� ���̺� ���� Ŭ����
            //if ((float)enemyLeftInWave / enemySpawnedInWave < waveClearRatio)
            //{
            //    MoveToNextWave();
            //}

            //ī��Ʈ
            int index = stageIndex - 1;
            float clearTime = gameTime - pastWaveTime;
            float timeLeft = nextWaveTime - gameTime;
            if(enemyLeftInWave <= 0)
            {
                Debug.Log("�������� " + index + "Ŭ����Ÿ�� : " + clearTime + " , �ܿ��ð� : " + timeLeft);
                if (FinalWave)
                {
                    StageClear();
                }
                else
                {
                    //���� �ð��� 4�ʷ� ����.
                    //MoveToNextWave();
                    if (nextWaveTime - gameTime > 4.0f)
                    {
                        gameTime = nextWaveTime - 4.0f;
                    }
                }
                
            }
        }
    }


    float pastWaveTime = 0f;
    //���̺�(�Ϲ�, ����)�� ������ �����ϸ� true ��ȯ, ������ ���̺갡 ������ false ��ȯ.
    private void MoveToNextWave()
    {
        if(stageIndex < stage.waves.Count)
        {
            //�⺻ ���̺�
            Wave wave = stage.waves[stageIndex];
            SpawnWave(wave);
            pastWaveTime = nextWaveTime;
            gameTime = nextWaveTime;
            //Debug.Log("���̺� " + stageIndex  + "���۽ð� : "  + gameTime);
            stageIndex++;
            nextWaveTime += wave.totalTime;
        }
        else
        {
            //���� ���̺�
            if (stage.hasBossWave)
            {
                SpawnWave(stage.bossWave);
                FinalWave = true;
                activatedWave = false;
            }
        }


        //���� ���̺� üũ
        if (stageIndex >= stage.waves.Count)
        {
            if(stage.hasBossWave)
            {
                //���� �ƴ�
            }
            else
            {
                FinalWave = true;
                activatedWave = false;
            }
        }


        //UI����
        currentWaveText.text = stageIndex.ToString();
    }

    //���̺� Ŭ���� �� ����
    void WaveClearEvent()
    {
        if (WaveClear != null) WaveClear();
    }

    //�� �������� Ŭ���� �� ����. ������ ��� ���, Ȥ�� (������ ���� ��쿡��) �ʿ� �ִ� ��� ���͸� �������� �� ����.
    void StageClear()
    {
        Debug.Log("�������� Ŭ����");

        //���� ���������� �����ϴٸ�
        if (GameManager.Instance.IsNextStageAvailable())
        {
            //���� ���������� �̵�
            GameManager.Instance.MoveToNextStage();
        }
        else
        {
            //é�� Ŭ����! 
            StartCoroutine(ClearRoutine());
        }
        
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

    //��ȯ�� ���͵��� ����������. 
    void MonsterDisapper()
    {
        Debug.Log("���� ����");
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
                        //������ �������� ���� �����̿� �ִ� �༺���� �̵���Ų��. 
                        Planet planet = SelectClosesetPlanetFromScreen(safePoint);
                        int strikePointIndex = planet.GetClosestIndex(transform.position);
                        strikePos = planet.worldPoints[strikePointIndex];
                        break;

                    case EnemyType.Air:
                        //������ ĳ���� �ֺ� �������� �̵���Ų��. 
                        strikePos = GetRandomPointNearPlayer();
                        break;
                }
                //Strike����
                prefab.GetComponent<EnemyAction>().EnemyStartStrike(strikePos);


                //��ȯ�� �� ����Ʈ�� �߰�.
                spawnedEnemyList.Add(prefab);

                //��ȯ�� �� UI�� �߰�
                minimapManager.AddMonster(prefab);
            }
            yield return new WaitForSeconds(enemy.delay);
        }

    }


    public bool GetSafePointFromOutsideScreen(out Vector2 safePoint)
    {
        //��ũ�� > ���� ��ǥ ����. z = 10 �� ī�޶���� �Ÿ�.
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
    //���� ���� �� �� ��ü�� �༺���� �ҷ��´�
    void GetStagePlanets()
    {
        Planet[] planets = GameObject.FindObjectsOfType<Planet>();
        foreach (Planet planet in planets)
        {
            planetList.Add(planet);
        }
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

    #endregion


    //�÷��̾� �ֺ��� ������ ���� ����Ʈ�� �ҷ��´�. 
    Vector2 GetRandomPointNearPlayer()
    {
        Vector2 randomPoint = Vector2.zero;
        int maxAttempts = 20;

        for (int i = 0; i < maxAttempts; i++)
        {
            //�÷��̾� ��ġ���� 2~10 ������ ���� ����Ʈ�� �����´�. 
            float radius = UnityEngine.Random.Range(minAirDistance, maxAirDistance);
            Vector2 dir = UnityEngine.Random.insideUnitCircle;
            Vector2 pos = (Vector2)transform.position + (dir * radius);

            //�ش� ��ġ�� �༺�� ��ġ�� �ʴ��� Ȯ���Ѵ�. 
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
    public float startTime; //���� ���� ~ ���� ������� ���ð�
    public bool hasBossWave;
    public List<Wave> waves = new List<Wave>();
    public Wave bossWave = new Wave();
}

