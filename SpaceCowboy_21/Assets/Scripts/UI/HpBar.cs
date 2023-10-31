using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    public TextMeshProUGUI currHealth;
    Health health;
    private void Awake()
    {
        health = GameManager.Instance.player.GetComponent<Health>();
    }

    private void LateUpdate()
    {
        currHealth.text = health.currHealth.ToString();
    }
}
