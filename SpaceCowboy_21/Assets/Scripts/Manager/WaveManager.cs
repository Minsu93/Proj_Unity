using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public float gameTime { get; set; } //���� �ð�
    public float waveTime { get; set; } //���� ���̺갡 ���۵� �ð�
    [SerializeField] private AnimationCurve waveTimeCurve;
    public int waveIndex { get; set; }   //���� ��ȣ 0 ~ 3

    public EnemyTeam[] enemyTeams;  //�� ���̺긶�� ������ ������ ����.
    //test
    public GameObject testObj;

    private void Awake()
    {
        instance = this; 
    }

    private void Update()
    {
        gameTime += Time.deltaTime;

        if(gameTime >= waveTime)
        {
            gameTime = 0f;
            SpawnEnemy();
            NextLevel();

        }
    }

    void NextLevel()
    {
        waveIndex ++;
        waveTime = waveTimeCurve.Evaluate(waveIndex);
        
    }

    void SpawnEnemy()
    {
        //���� ��ġ�� �����ɴϴ�. 
        Vector2 randomPoint = GetPointFromOutsideScreen();

        //���� ��ġ�� �ٸ� �༺�� �־� ������ ���� ��쿡��?

        Instantiate(testObj, randomPoint, Quaternion.identity);

        Debug.Log("Wave : " + waveIndex);
    }

    float spawnRange = 5f;
    float distanceFromScreen = 5f;

    Vector2 GetPointFromOutsideScreen()
    {
        Vector3 screenSizeMin = new Vector3(0, 0, 10);
        Vector3 screenSizeMax = new Vector3(Camera.main.pixelWidth, Camera.main.pixelHeight,10);
        screenSizeMin = Camera.main.ScreenToWorldPoint(screenSizeMin);
        screenSizeMax = Camera.main.ScreenToWorldPoint(screenSizeMax);
        float x, y;

        x = Random.Range(-spawnRange, spawnRange);
        if(x>0)
        {
            x = x + screenSizeMax.x + distanceFromScreen;
        }
        else
        {
            x = x + screenSizeMin.x - distanceFromScreen;
        }

        y = Random.Range(-spawnRange, spawnRange);
        if(y > 0)
        {
            y = y + screenSizeMax.y + distanceFromScreen;
        }
        else
        {
            y = y + screenSizeMin.y - distanceFromScreen;
        }

        return new Vector2(x, y); 

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