using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ʈ�� �������ϴ� ��ũ��Ʈ.
/// ������ ���� : �༺�� �پ �����Ǵ� ������Ʈ, ���ְ����� �����Ǵ� ������Ʈ, ȭ�� �ۿ��� �������� ������Ʈ.
/// �����Ǵ� ������Ʈ�� ���� min -> max ��ġ�� ���� �þ��. 
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
        //������Ʈ�� ȭ�� �ۿ��� �����Ѵ�. 
        if(WaveManager.instance.GetSafePointFromOutsideScreen(out Vector2 safePoint))
        {
            GameObject prefab = GameManager.Instance.poolManager.GetPoolObj(spawnObj, 4);
            prefab.transform.position = safePoint;
            if (prefab.TryGetComponent<ExplodeAsteroid>(out ExplodeAsteroid asteroid))
            {
                asteroid.InitializeObject();
                //������ ������Ʈ�� ȭ�� ����(�߾� ����)�� �����̰� �����. 
                asteroid.MoveToTargetPoint(GetRandomPointNearPlayer());
            }
        }
        
        //��踦 ����� �������. 
    }


    Vector2 GetRandomPointNearPlayer()
    {
        float min = 2;
        float max = 5;
        //�÷��̾� ��ġ���� 2~10 ������ ���� ����Ʈ�� �����´�. 
        float radius = UnityEngine.Random.Range(min, max);
        Vector2 dir = UnityEngine.Random.insideUnitCircle;
        Vector2 pos = (Vector2)transform.position + (dir * radius);
        return pos;
    }




}

[System.Serializable]
public class StageObject
{
    public GameObject spawnPrefab;    //���� ������Ʈ
    public int spawnCountAtOnce = 1;  //�ѹ��� ���ܳ��� ��
    public float spawnInterval = 10.0f;   //������Ʈ ����
    [HideInInspector] public float timer = 0f;
}