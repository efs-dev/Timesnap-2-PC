// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("Logic")]
    [HutongGames.PlayMaker.Tooltip("Test multiple booleans.")]
    public class ToggleBoxCollider : FsmStateAction
    {
        public FsmGameObject Target;
        public FsmBool IsOn;

        public override void Reset()
        {
            Target = null;
            IsOn = false;
        }
        
        public override void OnEnter()
        {
            Target.Value.GetComponent<BoxCollider>().enabled = IsOn.Value;
            Finish();
        }
    }
}