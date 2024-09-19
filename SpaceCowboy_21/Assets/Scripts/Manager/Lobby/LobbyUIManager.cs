using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LobbyUIManager : MonoBehaviour
{
    [Header("LobbyUI")]

    [SerializeField]
    UiPanel[] lobbyPanels;

    [Header("StageUI")]
    [SerializeField]
    UiPanel stageUIPanel;
    [SerializeField]
    UiPanel[] stagePanels;
    bool stageOn;


    [SerializeField]
    CanvasGroup lobbyCanvasGroup;
    [SerializeField]
    CanvasGroup stageCanvasGroup;

    #region Start from StageUI
    private void OnEnable()
    {
        SceneManager.sceneLoaded += StartFromStageUI;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= StartFromStageUI;
    }

    void StartFromStageUI(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.Instance.fromStageUI)
        {
            //stageUI를 오픈한 상태에서 시작한다.
            SwitchLobbyAndStage(true);
        }
    }

    #endregion

    public void SwitchLobbyAndStage(bool stageOn)
    {
        this.stageOn = stageOn;
        stageUIPanel.opened = stageOn;
        stageUIPanel.panel.SetActive(stageOn);

        //각 상태에 따른 메뉴 버튼 활성화.
        lobbyCanvasGroup.interactable = !stageOn;
        stageCanvasGroup.interactable = stageOn;
    }

    /// <summary>
    /// 탭을 연다
    /// </summary>
    /// <param name="panel"></param>
    public void OpenPanel(int index)
    {
        if(stageOn)
        {
            stagePanels[index].opened = true;
            stagePanels[index].panel.SetActive(true);
            stageCanvasGroup.interactable = false;
        }
        else
        {
            lobbyPanels[index].opened = true;
            lobbyPanels[index].panel.SetActive(true);
            lobbyCanvasGroup.interactable = false;
        }
    }

    /// <summary>
    /// 열려있는 탭을 닫는다. 
    /// </summary>
    public void CloseAllTab()
    {
        if (stageOn)
        {
            for (int i = 0; i < stagePanels.Length; i++)
            {
                //열려 있으면 닫는다.
                if (stagePanels[i].opened)
                {
                    stagePanels[i].opened = false;
                    stagePanels[i].panel.SetActive(false);
                }
            }

            //메인 메뉴 활성화
            stageCanvasGroup.interactable = true;
        }
        else
        {
            for (int i = 0; i < lobbyPanels.Length; i++)
            {
                //열려 있으면 닫는다.
                if (lobbyPanels[i].opened)
                {
                    lobbyPanels[i].opened = false;
                    lobbyPanels[i].panel.SetActive(false);
                }
            }

            //메인 메뉴 활성화
            lobbyCanvasGroup.interactable = true;
        }

        
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
