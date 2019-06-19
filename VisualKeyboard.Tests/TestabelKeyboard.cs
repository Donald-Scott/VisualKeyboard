using VisualKeyboard.Control;
using WindowsInput;

namespace VisualKeyboard.Tests
{
    public class TestabelKeyboard : Keyboard
    {
        public IInputSimulator InputSimulator { get; set; }

        protected override IInputSimulator CreateInputSimulator()
        {
            return InputSimulator;
        }
    }
}
