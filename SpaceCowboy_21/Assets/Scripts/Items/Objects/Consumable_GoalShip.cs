using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Consumable_GoalShip : Consumable
{
    public string nextLevelName;
    protected override void ConsumeFuction()
    {
        //���� ������ �̵�
        SceneManager.LoadScene(nextLevelName);
    }
}
