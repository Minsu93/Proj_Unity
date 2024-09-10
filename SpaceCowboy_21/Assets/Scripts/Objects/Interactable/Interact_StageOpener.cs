using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact_StageOpener : InteractableOBJ
{
    public override void InteractAction()
    {
        GameManager.Instance.LoadsceneByName("StageUI");
    }
}
