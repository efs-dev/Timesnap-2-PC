// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using HutongGames.PlayMaker;

namespace Efs.Playmaker.Actions
{
    [ActionCategory(ActionCategory.Logic)]
    [Tooltip("Tests if a GameObject is active.")]
    public class GameObjectIsActive : FsmStateAction
    {
        [Tooltip("The GameObject variable to test.")]
        public FsmGameObject gameObject;

        [Tooltip("Event to send if the GamObject is active.")]
        public FsmEvent isActive;

        [Tooltip("Event to send if the GamObject is NOT active.")]
        public FsmEvent isNotActive;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a bool variable.")]
        public FsmBool storeResult;

        [Tooltip("Repeat every frame.")]
        public bool everyFrame;

        public override void Reset()
        {
            gameObject = null;
            isActive = null;
            isNotActive = null;
            storeResult = null;
            everyFrame = false;
        }

        public override void OnEnter()
        {
            DoIsGameObjectActive();

            if (!everyFrame)
            {
                Finish();
            }
        }

        public override void OnUpdate()
        {
            DoIsGameObjectActive();
        }

        void DoIsGameObjectActive()
        {
            var goIsActive = gameObject.Value.activeSelf;

            if (storeResult != null)
            {
                storeResult.Value = goIsActive;
            }

            Fsm.Event(goIsActive ? isActive : isNotActive);
        }
    }
}