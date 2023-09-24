using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Consumable_GoalShip : Consumable
{
    public string nextLevelName;
    protected override void ConsumeFuction()
    {
        //다음 레벨로 이동
        SceneManager.LoadScene(nextLevelName);
    }
}
