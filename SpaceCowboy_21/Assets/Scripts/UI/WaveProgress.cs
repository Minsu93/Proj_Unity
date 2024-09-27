using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveProgress : MonoBehaviour
{
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

    int stageCount;
    float[] waveTimes;

    public void InitializeWaveProgress(Stage stage)
    {
        iconSpawnTime = stage.startTime;
        stageCount = stage.waves.Count;
        
        waveTimes = new float[stage.waves.Count];
        for(int i = 0; i < stage.waves.Count; i++)
        {
            waveTimes[i] = stage.waves[i].totalTime;
        }

    }
    #region ���̺� ������ ���� 
    public void IconSpawner(float gameTime)
    {
        //icon����
        if (iconSpawnTime - gameTime < maxSpawnLimit)
        {
            if (spawnIndex < stageCount)
            {
                //���� �ð��� ������ ����
                GameObject obj = Instantiate(MonsterIcon, iconParent.transform);
                RectTransform rect = obj.GetComponent<RectTransform>();
                IconPairs.Add(rect, iconSpawnTime);
                //���� �ð� ����
                iconSpawnTime += waveTimes[spawnIndex];
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

        IconMover(gameTime);
    }
    void IconMover(float gameTime)
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
}
