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
    public float totalWaveTime; //������ ���̺������ �ð�
    public float gameTime { get; set; } //���� �������� �ð�
    float nextWaveTime; //���� ���̺갡 ���۵� �ð�
    float preWaveTime;  //���� ���̺갡 �����ߴ� �ð�

    Stage stage;
    int stageIndex;
    Coroutine spawnRoutine;
    public int enemyLeft { get; set; }
    List<GameObject> spawnedEnemyList = new List<GameObject>();


    [Header("SpawnBox")]
    public float spawnRange = 5f;                  //���� ���� ����. 
    public float distanceUnitFromScreen = 5f;      //ȭ�鿡�� ������ ����. ���� ����.

    //�༺ ����
    [SerializeField] List<Planet> planetList = new List<Planet>();

    //��ũ��Ʈ.
    Dictionary<string, GameObject> monsterDict;
    [SerializeField] MinimapIndicator minimapIndicator;

    //Wave UI����
    [SerializeField] TextMeshProUGUI waveIndexText;
    [SerializeField] Image waveProgressImg;
    [SerializeField] TextMeshProUGUI leftEnemyCountText;

    //test(�ӽ�)
    public float timeSpeed;

    public event System.Action StageClear;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //Dictionary �ҷ�����
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
        //json�׽�Ʈ��

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

    //    waveTime = waveTimeCurve.Evaluate(waveIndex);   //���̺�Curve �� 0��° �ð�, �� ù ���̺� ���۽ð� ������Ʈ
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

        //���� ���̺��
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

            //UI ����
            leftEnemyCountText.text = enemyLeft.ToString();

            //������ �� ��������
            if(enemyLeft == 0)
            {
                //���� �ð� ����
                if(nextWaveTime - gameTime > 5.0f)
                {
                    gameTime = nextWaveTime - 5.0f;
                }
            }
        }

    }

    void MoveToNextWave()
    {
        //���� ���̺� ���� �� ���� ����.
        if (stageIndex > stage.waves.Count - 1)
        {
            Debug.Log("���̺� ����");
            activate = false;
            MonsterDisapper();
            SpawnBoss();
            return;
        }

        Debug.Log("���� ���̺� ���� : " + stage.waves[stageIndex].waveIndex);
        Wave curWave = stage.waves[stageIndex];

        //���� ���� ���� ������Ʈ�մϴ�. 
        foreach(Spawn spawn in curWave.spawns)
        {
            enemyLeft += spawn.spawnCount;
        }
        leftEnemyCountText.text = enemyLeft.ToString();

        //UI�� ������Ʈ �մϴ�.
        preWaveTime = nextWaveTime;
        waveIndexText.text = curWave.waveIndex.ToString();

        //���̺긦 �����մϴ�. 
        if (spawnRoutine != null) StopCoroutine(spawnRoutine);
        spawnRoutine = StartCoroutine(SpawnInOrder(curWave.spawns));

        //�ε����� �������� �����մϴ�.
        stageIndex++;
        nextWaveTime += curWave.totalTime;
    }

    #region Spawns

    //��ȯ�� ���͵��� ����������. 
    void MonsterDisapper()
    {
        if (StageClear != null) StageClear();

        //UI ����
        spawnedEnemyList.Clear();
        enemyLeft = 0;
        leftEnemyCountText.text = enemyLeft.ToString();
    }

    //����Ÿ�� ����
    void SpawnBoss()
    {

    }

    //���� ����
    void SpawnArtifact()
    {
        //minimapIndicator���� ǥ�õǰ� �����ϱ�.
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
                GameObject prefab = GameManager.Instance.poolManager.GetEnemy(enemyPrefab);
                prefab.transform.position = safePoint;
                prefab.transform.rotation = Quaternion.identity;
                prefab.GetComponent<EnemyAction>().EnemyStartStrike(SelectClosesetPlanetFromScreen(safePoint));
                
                //��ȯ�� �� ����Ʈ�� �߰�.
                spawnedEnemyList.Add(prefab);
            }
            yield return new WaitForSeconds(enemy.delay);
        }

    }


    bool GetSafePointFromOutsideScreen(out Vector2 safePoint)
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
    public float delay;
}

//���� ���̺갡 �󸶳� ���ӵǴ°�. 
[Serializable]
public class Spawn
{
    public Enemy enemy;
    public int spawnCount;
    public float delayToNextWave; 
}

//�ѹ��� Wave�� �������� Spawn���� ������ 
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

