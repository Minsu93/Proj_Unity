using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gauge_ChargeJump : MonoBehaviour
{
    public Image chargeJumpGauge;

    public Color chargeColor;
    public Color maxColor;

    float currPower;
    float maxPower;
    bool activate;
    PlayerBehavior playerBehavior;


    private void Awake()
    {
        playerBehavior = GameManager.Instance.player.GetComponent<PlayerBehavior>();
    }

    private void LateUpdate()
    {
        //currPower = playerBehavior.curChargedTime;
        //maxPower = playerBehavior.maxChargeTime;
        //currPower = playerBehavior.chargedPower;
        //maxPower = playerBehavior.maxChargePower;
        DecideShowing();

        if (!activate) return;

        if (currPower / maxPower < 0.9f)
        {
            chargeJumpGauge.color = chargeColor;
        }
        else
        {
            chargeJumpGauge.color = maxColor;
        }

        chargeJumpGauge.fillAmount = currPower / maxPower;
    }

    //������ �� ���� ���.
    void DecideShowing()
    {
        if (currPower < 0.01f)
        {
            if (activate)
            {
                HideGauge();
            }
        }
        else
        {
            if (!activate)
            {
                ShowGauge();
            }
        }
    }

    void ShowGauge()
    {
        activate = true;
        chargeJumpGauge.gameObject.SetActive(true);
    }

    void HideGauge()
    {
        activate = false;
        chargeJumpGauge.gameObject.SetActive(false);
    }
}
