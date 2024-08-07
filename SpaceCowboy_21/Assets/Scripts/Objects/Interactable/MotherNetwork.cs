using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로비에서 스테이지 선택 시 해당 포탈을 생성한다. 
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
