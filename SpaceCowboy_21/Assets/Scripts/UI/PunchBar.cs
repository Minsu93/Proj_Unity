using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PunchBar : MonoBehaviour
{
    public Image fillImage;

    SlingPunch slingPunch;



    private void Awake()
    {
        Transform playerTr = GameManager.Instance.player;
        slingPunch = playerTr.GetComponent<SlingPunch>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        fillImage.fillAmount = slingPunch.coolTime / slingPunch.punchCooltime;
    }
}
