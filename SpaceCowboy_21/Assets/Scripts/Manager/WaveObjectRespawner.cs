using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트를 리스폰하는 스크립트.
/// 리스폰 종류 : 행성에 붙어서 생성되는 오브젝트, 우주공간에 생성되는 오브젝트, 화면 밖에서 떨어지는 오브젝트.
/// 생성되는 오브젝트의 수는 min -> max 수치로 점점 늘어난다. 
/// 
/// </summary>
/// 



public class WaveObjectRespawner : MonoBehaviour
{
    public StageObject[] stageObjects;


    public void UpdateSpawner()
    {
        foreach(var stageObj in stageObjects)
        {
            stageObj.timer += Time.deltaTime;

            if(stageObj.timer >= stageObj.spawnInterval)
            {
                stageObj.timer = 0;
                for (int i = 0; i < stageObj.spawnCountAtOnce; i++)
                {
                    SpawnObjectOutsideScreen(stageObj.spawnPrefab);
                }
            }
        }
    }

    void SpawnObjectOutsideScreen(GameObject spawnObj)
    {
        //오브젝트를 화면 밖에서 생성한다. 
        if(WaveManager.instance.GetSafePointFromOutsideScreen(out Vector2 safePoint))
        {
            GameObject prefab = GameManager.Instance.poolManager.GetPoolObj(spawnObj, 4);
            prefab.transform.position = safePoint;
            if (prefab.TryGetComponent<ExplodeAsteroid>(out ExplodeAsteroid asteroid))
            {
                asteroid.InitializeObject();
                //생성한 오브젝트는 화면 내부(중앙 지역)로 움직이게 만든다. 
                asteroid.MoveToTargetPoint(GetRandomPointNearPlayer());
            }
        }
        
        //경계를 벗어나면 사라진다. 
    }


    Vector2 GetRandomPointNearPlayer()
    {
        float min = 2;
        float max = 5;
        //플레이어 위치에서 2~10 사이의 랜덤 포인트를 가져온다. 
        float radius = UnityEngine.Random.Range(min, max);
        Vector2 dir = UnityEngine.Random.insideUnitCircle;
        Vector2 pos = (Vector2)transform.position + (dir * radius);
        return pos;
    }




}

[System.Serializable]
public class StageObject
{
    public GameObject spawnPrefab;    //생성 오브젝트
    public int spawnCountAtOnce = 1;  //한번에 생겨나는 수
    public float spawnInterval = 10.0f;   //업데이트 간격
    [HideInInspector] public float timer = 0f;
}