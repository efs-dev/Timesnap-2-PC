// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("VR")]
    public class VREffect : FsmStateAction
    {
        public FsmBool Enable;
        public FsmFloat Duration;

        public override void Reset()
        {
            Enable = null;
        }
        
        public override void OnEnter()
        {
            var vrPlayer = UnityEngine.GameObject.FindObjectOfType<EFS.Timesnap.VR.TimesnapVRPlayer>();
            if (Enable.Value)
                vrPlayer.EnableVR(Duration.Value);
            else
                vrPlayer.DisableVR(Duration.Value);
            Finish();
        }
    }

    [ActionCategory("VR")]
    public class AutoVREffect : FsmStateAction
    {
        public FsmBool Enable;
        public FsmFloat Duration;

        public override void Reset()
        {
            Enable = null;
        }

        public override void OnEnter()
        {
            var vrPlayer = UnityEngine.GameObject.FindObjectOfType<EFS.Timesnap.VR.TimesnapVRPlayer>();
            vrPlayer.AutoVREffect = Enable.Value;
            vrPlayer.AutoVREffectDuration = Duration.Value;
            Finish();
        }
    }

    [ActionCategory("VR")]
    public class DisablePointer : FsmStateAction
    {
        public FsmBool EnableOnExit;
        private bool _wasEnabled;


        public override void Reset()
        {
            EnableOnExit = null;
        }

        public override void OnEnter()
        {
            _wasEnabled = VRInputHelper.IsPointerEnabled;
            VRInputHelper.IsPointerEnabled = false;

            Finish();
        }

        public override void OnExit()
        {
            if (EnableOnExit.Value)
                VRInputHelper.IsPointerEnabled = _wasEnabled;
        }
    }

    [ActionCategory("VR")]
    public class EnablePointer : FsmStateAction
    {
        public FsmBool DisableOnExit;
        private bool _wasEnabled;


        public override void Reset()
        {
            DisableOnExit = null;
        }

        public override void OnEnter()
        {
            _wasEnabled = VRInputHelper.IsPointerEnabled;
            VRInputHelper.IsPointerEnabled = true;

            Finish();
        }

        public override void OnExit()
        {
            if (DisableOnExit.Value)
                VRInputHelper.IsPointerEnabled = _wasEnabled;
        }
    }
}