using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyObject : MonoBehaviour
{
    EnemyAction action;
    MapObjective mapObjective;

    private void Start()
    {
        mapObjective = FindObjectOfType<MapObjective>();
        action = GetComponent<EnemyAction>();

        action.EnemyDieEvent += KeyDieEvent;
        
    }

    void KeyDieEvent()
    {
        mapObjective.GetStar(this.gameObject);
    }
}
