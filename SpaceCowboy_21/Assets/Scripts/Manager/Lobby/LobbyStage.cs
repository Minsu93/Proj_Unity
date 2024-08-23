using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyStage : MonoBehaviour
{
    ///�� ��ư�� ������ �ش� �ʿ� ���� ���� �׸��� ���´�. 
    ///���õ� ���� �ִ� ���, ���� ���� ��ư�� ������ �� ������ �̵��ȴ�. 
    ///

    [SerializeField] private StageData[] stageDatas;
    [SerializeField] private Image mainStageImage;
    [SerializeField] private TextMeshProUGUI mainStageNameText;
    [SerializeField] private TextMeshProUGUI stageDescriptionText;

    [Header("Portal")]
    [SerializeField] private GameObject portalObj;
    Animator portalAnim;
    private int curStageIndex = 0;

    private void Awake()
    {
        portalAnim = portalObj.GetComponent<Animator>();
        portalObj.SetActive(false);
    }

    public void SelectStage(int index)
    {
        //���� �������� ������ �� ũ�� �۵�x
        if (index > stageDatas.Length)
        {
            Debug.Log("�������� �ʴ� ���������Դϴ�");
            return;
        }

        //stageData�� �����´�
        StageData curData = stageDatas[index];
        //�������� ������ �����Ѵ�. (�̹���, ����...)
        mainStageImage.sprite = curData.StageImage;
        mainStageNameText.text = curData.StageName;
        stageDescriptionText.text = curData.StageDescription;
        //������ ���������� �����Ѵ�. (index)
        curStageIndex = index;
    }



    //�ش��ϴ� ���� �ҷ��´�. 
    public void StartStage()
    {
        //���տ� ��Ż�� ��Ÿ����. 
        //���ΰ��� ��Ż�� �������� ȭ��Ʈ�ƿ�?

        StartCoroutine(StageStartRoutine());
    }


    IEnumerator StageStartRoutine()
    {
        portalObj.SetActive(true);
        portalAnim.SetTrigger("PortalOpen");
        yield return new WaitForSeconds(2.0f);
        GameManager.Instance.TransitionFadeOut(true);
        yield return new WaitForSeconds(1.0f);
        GameManager.Instance.LoadsceneByName(stageDatas[curStageIndex].SceneAddress);

    }
}
