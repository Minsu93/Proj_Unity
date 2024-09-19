using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField]
    UiPanel stageUIPanel;
    [SerializeField]
    UiPanel[] stagePanels;

    [SerializeField]
    CanvasGroup stageCanvasGroup;

    /// <summary>
    /// ���� ����
    /// </summary>
    /// <param name="panel"></param>
    public void OpenPanel(int index)
    {
        stagePanels[index].opened = true;
        stagePanels[index].panel.SetActive(true);
        stageCanvasGroup.interactable = false;
    }

    /// <summary>
    /// �����ִ� ���� �ݴ´�. 
    /// </summary>
    public void CloseAllTab()
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
