// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine.Video;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("VideoPlayer")]
    [HutongGames.PlayMaker.Tooltip("Pause a video player.")]
    public class VPPause : FsmStateAction
    {
        public VideoPlayer VideoPlayer;

        public override void Reset()
        {
            VideoPlayer = null;
        }
        
        public override void OnEnter()
        {
            VideoPlayer.Pause();
            Finish();
        }
    }
}