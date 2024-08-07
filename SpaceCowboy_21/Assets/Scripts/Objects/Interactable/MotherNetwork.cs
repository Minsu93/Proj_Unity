using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �κ񿡼� �������� ���� �� �ش� ��Ż�� �����Ѵ�. 
/// </summary>
public class MotherNetwork : UIController
{
    [SerializeField] StagePortal lobbyPortal;

    private void Awake()
    {
        lobbyPortal.gameObject.SetActive(false);
    }

    public void ActivatePortal(string sceneName)
    {
        lobbyPortal.gameObject.SetActive(true);
        lobbyPortal.SceneName = sceneName;
        StopInteract();
    }

}
