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
            //stageUI�� ������ ���¿��� �����Ѵ�.
            OpenPanel(0);
        }
    }


    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="panel"></param>
    public void OpenPanel(int index)
    {
        if(index > uiPanels.Length)
        {
            Debug.Log("�������� �ʴ� �г��Դϴ�");
            return;
        }

        if(!uiPanels[index].opened)
        {
            ////�ٸ� �г��� ��� �ݴ´�
            //CloseAllTab();
            //���ϴ� �г��� ����
            uiPanels[index].opened = true;
            uiPanels[index].panel.SetActive(true);
            //���� �޴� ��Ȱ��ȭ
            mainCanvasGroup.interactable = false;
        }
    }

    /// <summary>
    /// �����ִ� ���� �ݴ´�. 
    /// </summary>
    public void CloseAllTab()
    {
        for (int i = 0; i < uiPanels.Length; i++)
        {
            //���� ������ �ݴ´�.
            if (uiPanels[i].opened)
            {
                uiPanels[i].opened = false;
                uiPanels[i].panel.SetActive(false);
            }
        }

        //���� �޴� Ȱ��ȭ
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
