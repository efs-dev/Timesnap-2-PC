// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine.Video;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("VideoPlayer")]
    [HutongGames.PlayMaker.Tooltip("Resume a video player.")]
    public class VPResume : FsmStateAction
    {
        public VideoPlayer VideoPlayer;
        
        public FsmEvent OnLoopOrComplete;
        public bool FinishOnComplete;


        public override void Reset()
        {
            if (VideoPlayer != null)
                VideoPlayer.loopPointReached -= OnLoop;
            VideoPlayer = null;
        }

        public override void OnExit()
        {
            VideoPlayer.loopPointReached -= OnLoop;
        }

        public override void OnEnter()
        {
            VideoPlayer.loopPointReached -= OnLoop;
            VideoPlayer.loopPointReached += OnLoop;

            VideoPlayer.Play();

            if (!FinishOnComplete)
                Finish();
        }

        void OnLoop(VideoPlayer video)
        {
            Event(OnLoopOrComplete);

            if (FinishOnComplete)
                Finish();
        }
    }
}