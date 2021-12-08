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
        public enum Types { Workshop, Tavern, Jail };
        public Types Type;                

        public override void OnEnter()
        {
            switch (Type)
            {
                case Types.Workshop:
                    EFS.Timesnap.VR.TimesnapVRPlayer.Instance.StartCoroutine(AssetBundleLoader.LoadWorkshop(() => Finish()));
                    break;
                case Types.Tavern:
                    EFS.Timesnap.VR.TimesnapVRPlayer.Instance.StartCoroutine(AssetBundleLoader.LoadTavern(() => Finish()));
                    break;
                case Types.Jail:
                    EFS.Timesnap.VR.TimesnapVRPlayer.Instance.StartCoroutine(AssetBundleLoader.LoadJail(() => Finish()));
                    break;
            }
        }
    }
}