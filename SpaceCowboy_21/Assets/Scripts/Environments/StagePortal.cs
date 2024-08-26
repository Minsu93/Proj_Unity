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
            //임시로, 시작하자마자 포탈 활성화
            ActivatePortal();

        }
        else
            collObject.SetActive(false);
    }

    //포털의 UI를 리프레쉬한다
    public void RefreshPortalUi(int curStage, int maxStage)
    {
        curStageText.text = curStage.ToString();
        maxStageText.text = maxStage.ToString();
    }

    //포털을 활성화한다
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

