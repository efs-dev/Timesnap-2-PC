﻿// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;

using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("Timeline")]
    [HutongGames.PlayMaker.Tooltip("Stop a timeline.")]
    public class TimelineStop : FsmStateAction
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
            Target.Stop();
            Finish();
        }
    }
}