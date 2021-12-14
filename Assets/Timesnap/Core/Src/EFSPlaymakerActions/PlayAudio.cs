// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("Audio")]
    [HutongGames.PlayMaker.Tooltip("Plays the audio by ID, using resources for desktop and streamingassets for webgl.")]
    public class PlayAudio : FsmStateAction
    {

        public string Id;
        public float Volume = 1;
        public bool Loop;

        public bool FinishImmediately;
        public bool KillOnSceneChange;

        private Audio _audio;
        private bool _wasEnabled;

        public override void Reset()
        {
            _audio = null;
            _wasEnabled = false;
            Id = null;
        }
        
        public override void OnEnter()
        {
            _wasEnabled = VRInputHelper.IsPointerEnabled;
            VRInputHelper.IsPointerEnabled = false;
            EFS.Timesnap.VR.TimesnapVRPlayer.Instance.CCBox.SetActive(true);
            _audio = AudioManager.Play(Id, Loop ? -1 : 0, Volume);

            Debug.Log("Play Audio: " + Id);

            if (KillOnSceneChange)
            {
                AudioManager.AudioIdsToKill.Add(_audio.InstanceId);
            }

            if (FinishImmediately || Loop)
            {
                Complete();
            }
        }


        public override void OnUpdate()
        {
            if (Loop)
                return;

            if (_audio.IsComplete)
            {
                Complete();
            }
        }

        void Complete()
        {
            //if (KillOnSceneChange)
            //    SceneManager.sceneUnloaded -= SceneManager_sceneUnloaded;

            _audio = null;

            VRInputHelper.IsPointerEnabled = true;// _wasEnabled;
            EFS.Timesnap.VR.TimesnapVRPlayer.Instance.CCBox.SetActive(false);
            Finish();
        }
    }
}