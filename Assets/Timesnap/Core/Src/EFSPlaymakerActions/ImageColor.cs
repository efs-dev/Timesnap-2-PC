// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("UGUI")]
    [HutongGames.PlayMaker.Tooltip("Set the color of an Image.")]
    public class ImageColor : FsmStateAction
    {
        public Image Target;
        public FsmColor Color;

        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful if the value is animated.")]
        public bool everyFrame;

        public override void Reset()
        {
            Target = null;
            everyFrame = false;
        }
        
        public override void OnEnter()
        {
            Target.color = Color.Value;
            if (!everyFrame)
                Finish();
        }

        public override void OnUpdate()
        {
            if (everyFrame)
                Target.color = Color.Value;
        }
    }
}