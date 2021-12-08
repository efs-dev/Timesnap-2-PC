// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine.Video;
using UnityEngine;
using System.Collections;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("VideoPlayer")]
    [HutongGames.PlayMaker.Tooltip("Play a video player.")]
    public class VPPlay : FsmStateAction
    {
        public VideoPlayer VideoPlayer;

        public VideoClip OptionalClip;
        public FsmString OptionalUrl;
        //public AudioSource[] OptionalAudioSources;
        //public bool IsDirectAudio;

        public FsmFloat Time;

        public bool IsLooping;
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
            if (OptionalClip != null)
            {
                VideoPlayer.source = VideoSource.VideoClip;
                VideoPlayer.clip = OptionalClip;
            }
            if (!string.IsNullOrEmpty(OptionalUrl.Value))
            {
                VideoPlayer.source = VideoSource.Url;
                VideoPlayer.url = OptionalUrl.Value;
            }
            /*if (OptionalAudioSources.Length > 0)
            {
                VideoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
                for (var i = 0; i < OptionalAudioSources.Length; i++)
                {
                    VideoPlayer.SetTargetAudioSource((ushort)i, OptionalAudioSources[i]);
                }
            }
            if (IsDirectAudio)
                VideoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;*/

            VideoPlayer.time = Time.Value;
            VideoPlayer.isLooping = IsLooping;

            VideoPlayer.loopPointReached -= OnLoop;
            VideoPlayer.loopPointReached += OnLoop;

            //VideoPlayer.skipOnDrop = false;

            VideoPlayer.prepareCompleted -= OnPrepare;
            VideoPlayer.prepareCompleted += OnPrepare;
            VideoPlayer.Prepare();
            
            
            if (!FinishOnComplete)
                Finish();
        }

        void OnPrepare(VideoPlayer videoPlayer)
        {
            VideoPlayer.prepareCompleted -= OnPrepare;
            StartCoroutine(OnReadyToPlay());
        }

        IEnumerator OnReadyToPlay()
        {
            yield return new WaitForSeconds(0.5f);
            VideoPlayer.Play();
        }

        void OnLoop(VideoPlayer video)
        {
            Event(OnLoopOrComplete);

            if (FinishOnComplete)
                Finish();
        }
    }
}