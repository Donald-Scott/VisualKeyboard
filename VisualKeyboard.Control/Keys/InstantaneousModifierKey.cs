using WindowsInput;

namespace VisualKeyboard.Control
{
    internal class InstantaneousModifierKey : ModifierKeyBase
    {
        public InstantaneousModifierKey(IInputSimulator inputSimulator, VirtualKeyCode key) : base(inputSimulator, key)
        {
        }

        internal override void ScreenKeyPress()
        {
            if (IsInEffect)
            {
                KeyUp();
            }
            else
            {
                KeyDown();
            }

            IsInEffect = IsHardwareKeyDown();
            OnKeyPress(new LogicalKeyEventArgs(KeyCode));
        }

        internal override void SynchroniseKeyState()
        {
            IsInEffect = IsHardwareKeyDown();
        }
    }
}
