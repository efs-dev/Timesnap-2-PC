using TMPro;
using UnityEngine;

namespace Src.Scripts.OnScreenKeyboard
{
    public class CharKey : Key
    {
        [SerializeField]
        private char _keyValue;
        public char KeyValue
        {
            set
            {
                _keyValue = value;
                Label.text = GetValue();
            }

            get { return _keyValue; }
        }
        
        public TMP_Text Label;
        private bool _upperCase;

        private void Start()
        {
            Label.text = GetValue();
        }

        public override void ToggleUppercase()
        {
            _upperCase = !_upperCase;
            Label.text = GetValue();
        }

        protected override string GetValue()
        {
            return _upperCase ? char.ToUpper(_keyValue).ToString() : char.ToLower(_keyValue).ToString();
        }

        private void OnValidate()
        {
            Label.text = GetValue();
        }
    }
}