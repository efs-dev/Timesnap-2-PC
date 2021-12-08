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
    [HutongGames.PlayMaker.Tooltip("Resume a timeline.")]
    public class TimelineResume : FsmStateAction
    {
        public PlayableDirector Target;


        public override void Reset()
        {
            Target = null;
        }

        public override void OnExit()
        {
        }

        public override void OnEnter()
        {
            Target.Resume();
            Finish();
        }
    }
}