using WindowsInput.Native;

namespace VisualKeyboard.Tests
{
    internal partial class MockInputSimulator
    {
        internal class KeyActionChange
        {
            public KeyActionChange(VirtualKeyCode keyCode, KeyAction action)
            {
                KeyCode = keyCode;
                Action = action;
            }

            public VirtualKeyCode KeyCode { get; }
            public KeyAction Action { get; }
        }
    }
}
