using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    private void LateUpdate()
    {
        text.text = ResourceManager.Instance.alienResource.ToString();
    }
}
