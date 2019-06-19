using WindowsInput;

namespace VisualKeyboard.Control
{
    internal class TextKey : LogicalKey
    {
        public TextKey(IInputSimulator inputSimulator, VirtualKeyCode key, string text) : base(inputSimulator, key)
        {
            Text = text;
        }

        public string Text { get; set; }

        internal override void ScreenKeyPress()
        {
            OutputText(Text);
            base.ScreenKeyPress();
        }
    }
}
