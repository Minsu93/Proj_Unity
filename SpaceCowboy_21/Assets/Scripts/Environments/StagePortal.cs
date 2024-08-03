using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StagePortal : MonoBehaviour
{
    [SerializeField] string sceneName;
    [SerializeField] private GameObject collObject;
    [SerializeField] private GameObject stageUI;
    [SerializeField] private TextMeshProUGUI curStageText;
    [SerializeField] private TextMeshProUGUI maxStageText;

    private void OnEnable()
    {
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
        GameManager.Instance.Loadscene(sceneName);
    }
}
