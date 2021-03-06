// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine.Video;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("Logic")]
    [HutongGames.PlayMaker.Tooltip("Test multiple booleans.")]
    public class MutiBoolTest : FsmStateAction
    {
        public FsmBool[] Values;
        public int numTrue;

        public FsmBool Result;

        public FsmEvent GreaterOrEqual;
        public FsmEvent LessThan;

        public override void Reset()
        {
            Values = null;
            numTrue = 0;
        }
        
        public override void OnEnter()
        {
            var count = 0;
            for (var i = 0; i < Values.Length; i++)
            {
                if (Values[i].Value)
                    count++;
            }

            if (Result != null)
                Result.Value = count >= numTrue;

            //UnityEngine.Debug.Log(count + ", " + numTrue);

            if (count >= numTrue)
                Fsm.Event(GreaterOrEqual);
            else
                Fsm.Event(LessThan);

            Finish();
        }
    }
}