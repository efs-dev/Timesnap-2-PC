// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.


using System.Collections.Generic;
using HutongGames.PlayMaker;
using TMPro;

namespace Efs.PlayMaker.Actions
{

    [ActionCategory("TextMeshPro")]
    [HutongGames.PlayMaker.Tooltip("Activates/deactivates a Game Object. Use this to hide/show areas, or enable/disable many Behaviours at once.")]
    public class SetTMPText : FsmStateAction
    {
        public string Note;
        public bool UseOwner;
        public FsmGameObject GameObject;
        public TMP_Text TextField;
        [UIHint(UIHint.TextArea)]
        public FsmString Text;
        public bool UseInt;
        public FsmInt IntVariable;
        public bool UseFloat;
        public FsmFloat FloatVariable;

        public FsmBool UseColor;
        public FsmColor Color;

        public override void Reset()
        {
            TextField = null;
            Text = null;
            Note = "";
            Color = null;
        }

        public override void OnEnter()
        {
            var textField = UseOwner ? this.Fsm.GameObject.GetComponent<TMP_Text>() : (GameObject.Value != null ? ((UnityEngine.GameObject)GameObject.Value).GetComponent<TMP_Text>() : TextField);

            if (UseInt)
            {
                textField.text = IntVariable.Value.ToString();
            }
            else if (UseFloat)
            {
                textField.text = FloatVariable.Value.ToString();
            }
            else
            {
                textField.text = Text.Value;
            }

            if (UseColor.Value)
                textField.color = Color.Value;
            
            Finish();
        }
    }

    [ActionCategory("TextMeshPro")]
    [HutongGames.PlayMaker.Tooltip("Set the text color.")]
    public class SetTMPTextColor : FsmStateAction
    {
        public bool UseOwner;
        public FsmGameObject GameObject;
        public TMP_Text TextField;
        public FsmColor Color;

        public override void Reset()
        {
            Color = null;
        }

        public override void OnEnter()
        {
            var textField = UseOwner ? this.Fsm.GameObject.GetComponent<TMP_Text>() : (GameObject.Value != null ? ((UnityEngine.GameObject)GameObject.Value).GetComponent<TMP_Text>() : TextField);

            textField.color = Color.Value;

            Finish();
        }
    }
}