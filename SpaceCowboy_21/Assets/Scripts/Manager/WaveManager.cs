using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.VFX;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public bool activate;
    public string stageName = "stage0";

    [Header("Wave Property")]
    public float totalWaveTime; //������ ���̺������ �ð�
    public float gameTime { get; set; } //���� �������� �ð�
    float nextWaveTime; //���� ���̺갡 ���۵� �ð�

    Stage stage;
    int stageIndex;
    Coroutine spawnRoutine;


    [Header("SpawnBox")]
    public float spawnRange = 5f;                  //���� ���� ����. 
    public float distanceUnitFromScreen = 5f;      //ȭ�鿡�� ������ ����. ���� ����.


    //��ũ��Ʈ.
    public WaveUI waveUI;
    public InvasionUI invasionUI;
    Dictionary<string, GameObject> monsterDict;
    
    //test(�ӽ�)
    public GameObject testObj;
    public GameObject testObj2;
    public float timeSpeed;

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
        spawn_A.enemies.Add(enemy_A);
        spawn_A.enemies.Add((enemy_B));
        spawn_A.delayToNextWave = 5.0f;

        Spawn spawn_B = new Spawn();
        spawn_B.enemies.Add(enemy_A);
        spawn_B.enemies.Add((enemy_B));
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
        stageIndex = 0;
        spawnRoutine = null;
        StopAllCoroutines();

    }

    public void WaveStart(bool start)
    {
        ResetWaveManager();
        activate = start;
    }



    private void Update()
    {
        if (!activate) return; 

        gameTime += Time.deltaTime * timeSpeed;


        if(gameTime > nextWaveTime)
        {
            Debug.Log("���� ���̺� ����");
            //���̺긦 �����մϴ�. 
            nextWaveTime += stage.waves[stageIndex].totalTime;

            if(spawnRoutine!=null) StopCoroutine(spawnRoutine);
            spawnRoutine = StartCoroutine(RepeatSpawn(stage.waves[stageIndex].spawns));

            stageIndex++;
            if(stageIndex > stage.waves.Count) stageIndex = stage.waves.Count;

        }

    }

    //Spawns�� ����ִ� Spawn���� ������ �ݺ��Ѵ�. 
    IEnumerator RepeatSpawn(List<Spawn> spawns)
    {
        int spawnIndex = 0;
        while (true)
        {
            Debug.Log("�ݺ� ������ : " +  spawnIndex);   
            StartCoroutine(SpawnObjects(spawns[spawnIndex].enemies));
            yield return new WaitForSeconds(spawns[spawnIndex].delayToNextWave);
            spawnIndex = (spawnIndex + 1) % spawns.Count;
        }
    }

    //enemies�� ����ִ� ������ ������� �����Ѵ�. 
    IEnumerator SpawnObjects(List<Enemy> enemies)
    {
        foreach(Enemy enemy in enemies)
        {
            Debug.Log("�� ����");
            GameObject enemyPrefab = monsterDict[enemy.name];
            Vector2 safePoint;
            if (GetSafePointFromOutsideScreen(out safePoint))
            {
                GameObject prefab = PoolManager.instance.GetEnemy(enemyPrefab);
                prefab.transform.position = safePoint;
                prefab.transform.rotation = Quaternion.identity;
                prefab.GetComponent<EnemyBrain>().ResetEnemyBrain(EnemyState.Strike);
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
    public List<Enemy> enemies = new List<Enemy>();
    public float delayToNextWave; 
}

//�ѹ��� Stage�� ���� ���̺��� �ݺ����� ������ 
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

