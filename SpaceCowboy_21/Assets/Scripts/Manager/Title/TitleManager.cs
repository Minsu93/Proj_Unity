using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject MainMenu;
    public void OpenMenu(bool open)
    {
        StartButton.SetActive(!open);
        MainMenu.SetActive(open);
    }
}
