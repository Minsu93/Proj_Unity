using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDrop : SelfCollectable
{
    [SerializeField] float healAmount = 5f;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float torqueSpeed = 60f;
    [SerializeField] AnimationCurve moveCurve;
    protected override bool ConsumeEvent()
    {
        StopAllCoroutines();
        return GameManager.Instance.playerManager.HealthUp(healAmount);
    }

    public void LaunchPotion(Vector2 direction)
    {
        Vector2 startPos = (Vector2)transform.position;
        Vector2 targetPos = startPos + direction;

        //StartCoroutine(LaunchRoutine(startPos, targetPos));
        rb.AddForce(direction, ForceMode2D.Impulse);
        float ranTorque = UnityEngine.Random.Range(-torqueSpeed, torqueSpeed);
        rb.AddTorque(ranTorque);
    }

    IEnumerator LaunchRoutine(Vector2 startPos, Vector2 targetPos)
    {
        float distance = Vector2.Distance(startPos, targetPos);
        float timer = 0; 
        float maxTime = distance/ moveSpeed;
        while (timer <= maxTime)
        {
            timer += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(startPos, targetPos, moveCurve.Evaluate(timer/maxTime));
            rb.MovePosition(pos);
            yield return null;
        }

    }

}
