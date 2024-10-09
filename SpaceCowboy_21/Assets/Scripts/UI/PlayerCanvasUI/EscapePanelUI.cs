using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapePanelUI : MonoBehaviour
{
    [SerializeField] Sprite fillBorder;
    [SerializeField] Sprite emptyBorder;
    [SerializeField] Image fill_Image;
    [SerializeField] Image border_Image;
    bool isFull;
    public void UpdateGauge(float amount)
    {
        fill_Image.fillAmount = amount;
        
        if(amount >= 1 && !isFull)
        {
            ChangeBorder(true);
        }
        else if(amount < 1 && isFull)
        {
            ChangeBorder(false);
        }
    }

    void ChangeBorder(bool isFull)
    {
        this.isFull = isFull;
        if (isFull)
        {
            border_Image.sprite = fillBorder;
        }
        else
        {
            border_Image.sprite = emptyBorder;
        }
    }
}
