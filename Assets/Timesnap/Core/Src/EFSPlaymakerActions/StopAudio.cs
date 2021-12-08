// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("Audio")]
    [HutongGames.PlayMaker.Tooltip("Plays the audio by ID, using resources for desktop and streamingassets for webgl.")]
    public class StopAudio : FsmStateAction
    {

        public string Id;

        public override void Reset()
        {
            Id = null;
        }
        
        public override void OnEnter()
        {
            AudioManager.Stop(Id);
            Finish();
        }
    }
}