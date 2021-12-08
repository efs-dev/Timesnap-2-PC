// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("Audio")]
    public class SetAudioManagerVolume : FsmStateAction
    {


        public override void Reset()
        {
        }
        
        public override void OnEnter()
        {
            AudioManager.Volume = PlayerPrefs.GetFloat("timesnap_volume");
            Finish();
        }


        public override void OnUpdate()
        {
        }
    }
}