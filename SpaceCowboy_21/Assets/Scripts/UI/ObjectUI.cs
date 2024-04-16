using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectUI : MonoBehaviour
{
    bool gaugeOn;

    InteractableOBJ int_Obj;
    public Image fillImage;
    public TextMeshProUGUI countText;

    private void Awake()
    {
        int_Obj = GetComponentInParent<InteractableOBJ>();
        countText.text = int_Obj.interactCost.ToString();
        
        countText.gameObject.SetActive(false);
        fillImage.transform.parent.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (!gaugeOn) return;

        fillImage.fillAmount = int_Obj.turnOnTimer / int_Obj.interactTime;

    }
    public void TextOn(bool on)
    {
        countText.gameObject.SetActive(on);
    }
    public void GaugeOn(bool on)
    {
        gaugeOn = on;
        fillImage.transform.parent.gameObject.SetActive(on);
    }

}
