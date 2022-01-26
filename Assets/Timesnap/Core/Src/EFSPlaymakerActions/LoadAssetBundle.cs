// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using System.Collections;

using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("AssetBundles")]
    public class LoadAssetBundle : FsmStateAction
    {
        public enum Types { Ohio, Boston, Christiana };
        public Types Type;                

        public override void OnEnter()
        {
            switch (Type)
            {
                case Types.Ohio:
                    EFS.Timesnap.VR.TimesnapVRPlayer.Instance.StartCoroutine(AssetBundleLoader.LoadOhio(() => Finish()));
                    break;
                case Types.Boston:
                    EFS.Timesnap.VR.TimesnapVRPlayer.Instance.StartCoroutine(AssetBundleLoader.LoadBoston(() => Finish()));
                    break;
                case Types.Christiana:
                    EFS.Timesnap.VR.TimesnapVRPlayer.Instance.StartCoroutine(AssetBundleLoader.LoadChristiana(() => Finish()));
                    break;
            }
        }
    }
}