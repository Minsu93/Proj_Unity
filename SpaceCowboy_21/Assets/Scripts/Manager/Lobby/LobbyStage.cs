using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyStage : MonoBehaviour
{
    ///맵 버튼을 누르면 해당 맵에 대한 설명 그림이 나온다. 
    ///선택된 맵이 있는 경우, 게임 시작 버튼을 누르면 그 맵으로 이동된다. 
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
        //현재 스테이지 수보다 더 크면 작동x
        if (index > stageDatas.Length)
        {
            Debug.Log("존재하지 않는 스테이지입니다");
            return;
        }

        //stageData를 가져온다
        StageData curData = stageDatas[index];
        //스테이지 정보를 변경한다. (이미지, 설명...)
        mainStageImage.sprite = curData.StageImage;
        mainStageNameText.text = curData.StageName;
        stageDescriptionText.text = curData.StageDescription;
        //선택한 스테이지를 변경한다. (index)
        curStageIndex = index;
    }



    //해당하는 씬을 불러온다. 
    public void StartStage()
    {
        //눈앞에 포탈이 나타난다. 
        //주인공은 포탈로 빠져들어가며 화이트아웃?

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
