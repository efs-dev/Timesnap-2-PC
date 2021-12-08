using TMPro;
using UnityEngine;

namespace Src.Scripts.OnScreenKeyboard
{
    public class SymbolKey : Key
    {
        public char LowercaseSymbolValue;
        public char UppercaseSymbolValue;
        public TMP_Text MainLabel;
        public TMP_Text UpperLabel;
        private bool _uppercase;

        private void Start()
        {
            SetLabels();
        }

        public override void ToggleUppercase()
        {
            _uppercase = !_uppercase;
            SetLabels();
        }

        private void SetLabels()
        {
            if (_uppercase)
            {
                MainLabel.text = UppercaseSymbolValue.ToString();
                UpperLabel.text = LowercaseSymbolValue.ToString();
            }
            else
            {
                MainLabel.text = LowercaseSymbolValue.ToString();
                UpperLabel.text = UppercaseSymbolValue.ToString();
            }
        }

        protected override string GetValue()
        {
            // # is how the unity event system represents an uppercased value for some reason
            return _uppercase ? UppercaseSymbolValue.ToString() : LowercaseSymbolValue.ToString();
        }

        private void Update()
        {
        }
    }
}