using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : EnemyAction
{
    public SkeletonAnimation skeletonAnimation;
    [SpineEvent(dataField: "skeletonAnimation", fallbackToTextField: true)]
    public string eventName;

    [Space]
    public bool logDebugMessage = false;

    Spine.EventData eventData;


    private void Start()
    {
        if (skeletonAnimation == null) return;
        skeletonAnimation.Initialize(false);
        if (!skeletonAnimation.valid) return;

        eventData = skeletonAnimation.Skeleton.Data.FindEvent(eventName);
        skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
    }

    private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (logDebugMessage) Debug.Log("Event fired! " + e.Data.Name);
        //bool eventMatch = string.Equals(e.Data.Name, eventName, System.StringComparison.Ordinal); // Testing recommendation: String compare.
        bool eventMatch = (eventData == e.Data); // Performance recommendation: Match cached reference instead of string.
        if (eventMatch)
        {
            //스파인 이벤트 실행 시 총알 발사
            //Shoot();
        }
    }

    public override void BrainStateChange()
    {
        throw new System.NotImplementedException();
    }

    public override void EnemyKnockBack(Vector2 hitPos, float forceAmount)
    {
        throw new System.NotImplementedException();
    }
}
