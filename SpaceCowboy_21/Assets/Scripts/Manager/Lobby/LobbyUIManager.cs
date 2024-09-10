using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField]
    UiPanel[] uiPanels;
    [SerializeField]
    CanvasGroup mainCanvasGroup;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OpenStageUI;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OpenStageUI;
    }

    void OpenStageUI(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance.fromStageUI)
        {
            //stageUI를 오픈한 상태에서 시작한다.
            OpenPanel(0);
        }
    }


    /// <summary>
    /// 탭을 연다
    /// </summary>
    /// <param name="panel"></param>
    public void OpenPanel(int index)
    {
        if(index > uiPanels.Length)
        {
            Debug.Log("존재하지 않는 패널입니다");
            return;
        }

        if(!uiPanels[index].opened)
        {
            ////다른 패널을 모두 닫는다
            //CloseAllTab();
            //원하는 패널을 연다
            uiPanels[index].opened = true;
            uiPanels[index].panel.SetActive(true);
            //메인 메뉴 비활성화
            mainCanvasGroup.interactable = false;
        }
    }

    /// <summary>
    /// 열려있는 탭을 닫는다. 
    /// </summary>
    public void CloseAllTab()
    {
        for (int i = 0; i < uiPanels.Length; i++)
        {
            //열려 있으면 닫는다.
            if (uiPanels[i].opened)
            {
                uiPanels[i].opened = false;
                uiPanels[i].panel.SetActive(false);
            }
        }

        //메인 메뉴 활성화
        mainCanvasGroup.interactable = true;
    }

    public void DisableAllButtons()
    {
        mainCanvasGroup.interactable = false;
        mainCanvasGroup.alpha = 0.0f;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CloseAllTab();
        }
    }

}

[Serializable]
public struct UiPanel
{
    [HideInInspector] public bool opened;
    public GameObject panel;
}
