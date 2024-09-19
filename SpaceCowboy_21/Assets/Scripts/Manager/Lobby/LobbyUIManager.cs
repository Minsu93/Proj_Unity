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
            //stageUI�� ������ ���¿��� �����Ѵ�.
            SwitchLobbyAndStage(true);
        }
    }

    #endregion

    public void SwitchLobbyAndStage(bool stageOn)
    {
        this.stageOn = stageOn;
        stageUIPanel.opened = stageOn;
        stageUIPanel.panel.SetActive(stageOn);

        //�� ���¿� ���� �޴� ��ư Ȱ��ȭ.
        lobbyCanvasGroup.interactable = !stageOn;
        stageCanvasGroup.interactable = stageOn;
    }

    /// <summary>
    /// ���� ����
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
    /// �����ִ� ���� �ݴ´�. 
    /// </summary>
    public void CloseAllTab()
    {
        if (stageOn)
        {
            for (int i = 0; i < stagePanels.Length; i++)
            {
                //���� ������ �ݴ´�.
                if (stagePanels[i].opened)
                {
                    stagePanels[i].opened = false;
                    stagePanels[i].panel.SetActive(false);
                }
            }

            //���� �޴� Ȱ��ȭ
            stageCanvasGroup.interactable = true;
        }
        else
        {
            for (int i = 0; i < lobbyPanels.Length; i++)
            {
                //���� ������ �ݴ´�.
                if (lobbyPanels[i].opened)
                {
                    lobbyPanels[i].opened = false;
                    lobbyPanels[i].panel.SetActive(false);
                }
            }

            //���� �޴� Ȱ��ȭ
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
