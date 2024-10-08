using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class AttackPattern : MonoBehaviour
{
    ///���� ������ 
    ///��Ÿ�� üũ(���� ��� �ð� - ����ð�)
    ///�ൿ �ڷ�ƾ(���� �ð�, �Ϸ� �ð�) 
    ///ĸ�� ���·� ���� ��������? -> ���� ���ɼ�.
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
