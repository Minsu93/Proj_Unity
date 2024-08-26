using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_UIOpener : InteractableOBJ
{
    [SerializeField] LobbyUIManager targetUIManager;
    [SerializeField] private int openIndex;
    public override void InteractAction()
    {
        if(targetUIManager != null)
        {
            targetUIManager.OpenPanel(openIndex);
        }
    }
}
