// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;

using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("Timeline")]
    [HutongGames.PlayMaker.Tooltip("Play a timeline.")]
    public class TimelinePlay : FsmStateAction
    {
        public PlayableDirector Target;
        public TimelineAsset OptionalTimeline;

        public FsmFloat Time;
        
        public bool FinishOnComplete;


        public override void Reset()
        {
            Target = null;
            OptionalTimeline = null;
            Time = 0;
        }

        public override void OnExit()
        {
        }

        public override void OnEnter()
        {
            if (OptionalTimeline != null)
            {
                Target.playableAsset = OptionalTimeline;
            }

            Target.time = Time.Value;

            StartCoroutine(RunTimeline());
            
            if (!FinishOnComplete)
                Finish();
        }

        IEnumerator RunTimeline()
        {
            Target.Play();
            yield return new WaitUntil(() => Target.time >= Target.duration);
            if (FinishOnComplete)
                Finish();
        }


        
    }
}