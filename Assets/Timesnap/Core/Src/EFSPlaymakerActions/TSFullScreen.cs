// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine.Video;
using UnityEngine;
using System.Collections;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("VR")]
    [HutongGames.PlayMaker.Tooltip("change to fullscreen")]
    public class TSFullScreen : FsmStateAction
    {
       

        public bool EnableFullScreen;


        public override void Reset()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override void OnEnter()
        {
            Debug.Log("I'm calling fullscreen "+ EnableFullScreen);
            Screen.fullScreen = EnableFullScreen;
            Finish();
        }

       
    }
}