using SpaceCowboy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenHealth : Health
{
    public PlayerBehavior playerBehavior;
    public CharacterGravity playerGravity;
    public float consumePerSeconds = 0.01f;
    public float consumeMultiplier = 1.0f;  
    protected override void Update()
    {
        base.Update();

        switch (playerGravity.oxygenInt)
        {
            case 0:
                //Green
                consumeMultiplier = -1f;
                break;

            case 1:
                //Red
                consumeMultiplier = 2f;
                break;

            case 2:
                //Blue
                consumeMultiplier = 1f;
                break;

            default:
                consumeMultiplier = 1f;
                break;

        }

        if(currHealth > 0)
        {
            currHealth -= consumePerSeconds * consumeMultiplier * Time.deltaTime;
            currHealth = Mathf.Clamp(currHealth, 0, maxHealth);

        }

        if (currHealth <=0)
        {
            playerBehavior.SendMessage("DeadEvent", SendMessageOptions.DontRequireReceiver);
            this.enabled = false;
        }
        
    }

    public void GetOxygen(float amount)
    {
        // 산소 구슬을 먹었을 때
        currHealth += amount;
        currHealth = Mathf.Clamp(currHealth, 0, maxHealth);

    }

}
