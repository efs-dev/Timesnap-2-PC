// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine.UI;

namespace Efs.PlayMaker
{
	public class PlaymakerUGUIButton : MonoBehaviour
    { 
        void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => GetComponent<PlayMakerFSM>().SendEvent("MOUSE UP AS BUTTON"));
        }
	}
}