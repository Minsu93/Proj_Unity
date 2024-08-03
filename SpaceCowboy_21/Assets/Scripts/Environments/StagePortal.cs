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
        GameManager.Instance.Loadscene(sceneName);
    }
}
