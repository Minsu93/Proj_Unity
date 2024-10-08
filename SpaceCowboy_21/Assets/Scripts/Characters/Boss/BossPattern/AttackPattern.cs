using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AttackPattern : MonoBehaviour
{
    ///보스 패턴은 
    ///쿨타임 체크(이전 사용 시간 - 현재시간)
    ///행동 코루틴(시작 시간, 완료 시간) 
    ///캡슐 형태로 만들어도 괜찮을듯? -> 재사용 가능성.
    ///
    bool isPlaying = false;
    public float coolTime { get; set; }
    float patternCloseTime;

    [SerializeField] protected float startDelay = 0.2f;
    [SerializeField] protected float delay = 0.2f;
    [SerializeField] protected float endDelay = 0.2f;

    public bool IsCoolTimeOver()
    {
        if (Time.time - patternCloseTime > coolTime && !isPlaying)
        {
            return true;
        }
        else return false;
    }

    public bool CheckCondition()
    {
        if (Condition()) return true;
        else return false;
    }

    public IEnumerator StartPattern()
    {
        isPlaying = true;

        yield return StartCoroutine(PatternFunction());

        isPlaying = false;
        patternCloseTime = Time.time;
    }

    protected abstract bool Condition();
    protected abstract IEnumerator PatternFunction();
}
