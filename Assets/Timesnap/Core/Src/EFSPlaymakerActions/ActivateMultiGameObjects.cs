// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

using System.Collections.Generic;
using HutongGames.PlayMaker;

namespace Efs.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameObject)]
    [HutongGames.PlayMaker.Tooltip("Activates/deactivates a Game Object. Use this to hide/show areas, or enable/disable many Behaviours at once.")]
    public class ActivateMultiGameObjects : FsmStateAction
    {
        [HutongGames.PlayMaker.ActionSection("Activate")]
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObjects to activate.")]
        public FsmGameObject[] activateGameObjects;

        [HutongGames.PlayMaker.Tooltip("Recursively activate/deactivate all children.")]
        public FsmBool activateRecursive;

        [HutongGames.PlayMaker.Tooltip("Is activateIf inverted?.")]
        public FsmBool invertActivateIf;

        [HutongGames.PlayMaker.Tooltip("Only activate if this is true.")]
        public FsmBool activateIf;

        [HutongGames.PlayMaker.ActionSection("Deactivate")]
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObjects to deactivate.")]
        public FsmGameObject[] deactivateGameObjects;

        [HutongGames.PlayMaker.Tooltip("Recursively activate/deactivate all children.")]
        public FsmBool deactivateRecursive;

        [HutongGames.PlayMaker.Tooltip("Is deactivateIf inverted?.")]
        public FsmBool invertDeactivateIf;

        [HutongGames.PlayMaker.Tooltip("Only deactivate if this is true.")]
        public FsmBool deactivateIf;

        [HutongGames.PlayMaker.Tooltip("Repeat this action every frame. Useful if Activate changes over time.")]
        public bool everyFrame;
        

        public override void Reset()
        {
            activateGameObjects = new FsmGameObject[1];
            deactivateGameObjects = new FsmGameObject[1];
            activateRecursive = false;
            deactivateRecursive = false;
            activateIf = true;
            deactivateIf = true;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            DoActivateGameObjects();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoActivateGameObjects();
        }

        public override void OnExit()
        {
        }

        void DoActivateGameObjects()
        {
            if (invertActivateIf.Value ? !activateIf.Value : activateIf.Value)
            {
                for (var i = 0; i < activateGameObjects.Length; i++)
                {
                    var gameObject = activateGameObjects[i].Value;

                    if (gameObject == null)
                    {
                        return;
                    }

                    if (activateRecursive.Value)
                    {
#if UNITY_3_5 || UNITY_3_4
                go.SetActiveRecursively(activate.Value);
#else
                        SetActiveRecursively(gameObject, true);
#endif
                    }
                    else
                    {
#if UNITY_3_5 || UNITY_3_4
                go.active = activate.Value;
#else
                        gameObject.SetActive(true);
#endif
                    }
                }
            }
            
            if (invertDeactivateIf.Value ? !deactivateIf.Value : deactivateIf.Value)
            {
                for (var i = 0; i < deactivateGameObjects.Length; i++)
                {
                    var gameObject = deactivateGameObjects[i].Value;

                    if (gameObject == null)
                    {
                        return;
                    }

                    if (deactivateRecursive.Value)
                    {
#if UNITY_3_5 || UNITY_3_4
                go.SetActiveRecursively(activate.Value);
#else
                        SetActiveRecursively(gameObject, false);
#endif
                    }
                    else
                    {
#if UNITY_3_5 || UNITY_3_4
                go.active = activate.Value;
#else
                        gameObject.SetActive(false);
#endif
                    }
                }
            }
           
        }

#if !(UNITY_3_5 || UNITY_3_4)
        public void SetActiveRecursively(GameObject go, bool state)
        {
            go.SetActive(state);
            foreach (Transform child in go.transform)
            {
                SetActiveRecursively(child.gameObject, state);
            }
        }
#endif

        
    }
}