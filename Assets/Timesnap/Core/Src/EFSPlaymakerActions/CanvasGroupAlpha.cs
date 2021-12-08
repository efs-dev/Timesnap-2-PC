// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("UGUI")]
    [HutongGames.PlayMaker.Tooltip("Set the alpha of a CanvasGroup.")]
    public class CanvasGroupAlpha : FsmStateAction
    {
        public CanvasGroup Target;
        public FsmFloat Alpha;

        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is animated.")]
        public bool everyFrame;

        public override void Reset()
        {
            Target = null;
            everyFrame = false;
        }
        
        public override void OnEnter()
        {
            Target.alpha = Alpha.Value;
            if (!everyFrame)
                Finish();
        }

        public override void OnUpdate()
        {
            if (everyFrame)
                Target.alpha = Alpha.Value;
        }
    }
}