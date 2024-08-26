using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StagePortal : MonoBehaviour
{
    [SerializeField] StageState stageState = StageState.Lobby;
    //[SerializeField] bool autoActivate = false;
    [SerializeField] string sceneName;
    public string SceneName { get { return sceneName; } set { sceneName = value; } }
    [SerializeField] private GameObject collObject;
    [SerializeField] private GameObject stageUI;
    [SerializeField] private TextMeshProUGUI curStageText;
    [SerializeField] private TextMeshProUGUI maxStageText;

    private void OnEnable()
    {
        if (stageState == StageState.Lobby)
        {   
            //�ӽ÷�, �������ڸ��� ��Ż Ȱ��ȭ
            ActivatePortal();

        }
        else
            collObject.SetActive(false);
    }

    //������ UI�� ���������Ѵ�
    public void RefreshPortalUi(int curStage, int maxStage)
    {
        curStageText.text = curStage.ToString();
        maxStageText.text = maxStage.ToString();
    }

    //������ Ȱ��ȭ�Ѵ�
    public void ActivatePortal()
    {
        stageUI.SetActive(false);
        collObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.SetActive(false);
            GameManager.Instance.LoadsceneByName(sceneName);
        }
    }
}

