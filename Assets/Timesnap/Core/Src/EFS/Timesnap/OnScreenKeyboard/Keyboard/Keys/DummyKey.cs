namespace Src.Scripts.OnScreenKeyboard
{
    public class DummyKey : Key
    {
        // used for keys like Shift and Enter
        public override void ToggleUppercase()
        {
        }

        protected override string GetValue()
        {
            return "";
        }
    }
}