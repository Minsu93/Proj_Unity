using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class UISkillPanel : MonoBehaviour
{
    [SerializeField] GameObject aSlot;
    [SerializeField] GameObject bSlot;
    [SerializeField] GameObject cSlot;
    [SerializeField] Image skill_A_Image_Background;
    [SerializeField] Image skill_B_Image_Background;
    [SerializeField] Image skill_C_Image_Background;
    [SerializeField] Image skill_A_Image;
    [SerializeField] Image skill_B_Image;
    [SerializeField] Image skill_C_Image;

    //UI인스턴스를 생성하면 슬롯을 리셋한다
    public void Awake()
    {
        aSlot.SetActive(false);
        bSlot.SetActive(false);
        cSlot.SetActive(false);
    }

    public void SetSkillImages(int index, Sprite backSprite, Sprite sprite)
    {
        switch (index)
        {
            case 0:
                aSlot.SetActive(true); 
                skill_A_Image_Background.sprite = backSprite;
                skill_A_Image.sprite = sprite;
                break;
            case 1:
                bSlot.SetActive(true);
                skill_B_Image_Background.sprite = backSprite;
                skill_B_Image.sprite = sprite;
                break;
            case 2:
                cSlot.SetActive(true);
                skill_C_Image_Background.sprite = backSprite;
                skill_C_Image.sprite = sprite;
                break;
        }
    }

    public void UpdateSkillFillamount(int index, float amount)
    {
        switch(index)
        {
            case 0:
                skill_A_Image.fillAmount = amount;
                break;
            case 1:
                skill_B_Image.fillAmount = amount;
                break;
            case 2:
                skill_C_Image.fillAmount = amount;
                break;
        }
    }
}
