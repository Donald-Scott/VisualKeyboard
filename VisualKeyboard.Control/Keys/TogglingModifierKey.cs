using WindowsInput;

namespace VisualKeyboard.Control
{
    internal class TogglingModifierKey : ModifierKeyBase
    {
        public TogglingModifierKey(IInputSimulator inputSimulator, VirtualKeyCode key) : base(inputSimulator, key)
        {
        }

        internal override void ScreenKeyPress()
        {
            IsInEffect = !IsTogglingKeyInEffect();
            base.ScreenKeyPress();
        }

        internal override void SynchroniseKeyState()
        {
            IsInEffect = IsTogglingKeyInEffect();
        }
    }
}
