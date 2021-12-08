using TMPro;

namespace Src.Scripts.OnScreenKeyboard
{
    public class StringKey : Key
    {
        public string Value;
        public TMP_Text Text;
        
        public override void ToggleUppercase()
        {
            // ignored
        }

        protected override string GetValue()
        {
            return Value;
        }
    }
}