using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] GameObject BossCanvas;
    GameObject bossUI;
    Image HpImage;
    TextMeshProUGUI text;

    public void SpawnBoss(string bossName)
    {
        SpawnBossUI(bossName);

        GameObject bossPrefab = GameManager.Instance.monsterDictonary.monsDictionary[bossName];
        Vector2 bossSpawnPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.7f, Camera.main.nearClipPlane));
        GameObject monster = GameManager.Instance.poolManager.GetPoolObj(bossPrefab, 3);
        monster.transform.position = bossSpawnPos;
        monster.transform.rotation = Quaternion.identity;

        BossAction bossAction = monster.GetComponent<BossAction>();
        bossAction.BossUIUpdateEvent += BossUiUpdate;
        bossAction.BossDieEvent += BossClear;
        bossAction.BossAwake();
    }



    #region Boss HP UI 

    void SpawnBossUI(string bossName)
    {
        //���� HP ui����
        bossUI = Instantiate(BossCanvas);
        HpImage = bossUI.transform.Find("Panel/HP_ImageBack/HP_ImageFill").GetComponent<Image>();
        text = bossUI.transform.Find("Panel/BossName").GetComponent<TextMeshProUGUI>();
        text.text = bossName;
    }

    void RemoveBossUI()
    {
        bossUI.SetActive(false);
    }

    void BossUiUpdate(float percent)
    {
        HpImage.fillAmount = percent;
    }

    #endregion


    #region Boss Clear(Chapter Clear)

    public void BossClear()
    {
        //ü���� �����Ѵ�.
        RemoveBossUI();

        //é�� Ŭ���� ȭ���� �����Ѵ�. 
        StartCoroutine(GameManager.Instance.ShowStageEndUI(2f, 3f));

        //é�� Ŭ����! 
        StartCoroutine(ClearRoutine());
    }

    IEnumerator ClearRoutine()
    {
        yield return new WaitForSeconds(5f);
        GameManager.Instance.TransitionFadeOut(true);
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.ChapterClear();
    }
    #endregion
}
