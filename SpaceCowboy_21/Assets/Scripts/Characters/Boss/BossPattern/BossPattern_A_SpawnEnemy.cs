using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern_A_SpawnEnemy : BossPattern_A
{
    [SerializeField] string enemyName;
    [SerializeField] int spawnCount;
    protected override bool Condition()
    {
        return true;
    }

    protected override IEnumerator PatternFunction()
    {
        yield return new WaitForSeconds(startDelay); 
        SpawnEnemy(enemyName,spawnCount);
        yield return new WaitForSeconds(endDelay);
    }
}
