using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Int_TaxiCall : InteractableOBJ
{
    [Header("WorldMap")]
    public TaxiWorldMap worldMap;

    public override void InteractAction()
    {
        worldMap.gameObject.SetActive(true);
        worldMap.OpenMap(1);
    }

    public override void turnOnAction()
    {
        return;
    }



}
