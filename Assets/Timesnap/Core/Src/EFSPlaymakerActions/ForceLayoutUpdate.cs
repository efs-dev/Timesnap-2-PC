// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace Efs.PlayMaker.Actions
{
    [ActionCategory("UI")]
    [HutongGames.PlayMaker.Tooltip("Hack to disable and renable the layout group.")]
    public class ForceLayoutUpdate : FsmStateAction
    {
        public GameObject Target;

        public override void Reset()
        {
        }

        public override void OnEnter()
        {
            StartCoroutine(Run());
        }

        IEnumerator Run()
        {
            Target.GetComponent<LayoutGroup>().enabled = false;
            Target.SetActive(false);
            yield return new WaitForSeconds(1);
            Target.SetActive(true);
            Target.GetComponent<LayoutGroup>().enabled = true;
            Canvas.ForceUpdateCanvases();
            Finish();
        }
    }
}