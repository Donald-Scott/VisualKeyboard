using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindowsInput;
using WindowsInput.Native;

namespace VisualKeyboard.Tests
{
    internal partial class MockInputSimulator : IInputSimulator, IKeyboardSimulator, IInputDeviceStateAdaptor
    {
        private readonly Dictionary<VirtualKeyCode, KeyState> keyStates;
        private bool isCapsLockOn;
        private readonly StringBuilder stringBuilder;

        private enum KeyState
        {
            Unknown = 0,
            Up,
            Down
        }

        internal enum KeyAction
        {
            Unknow = 0,
            Press,
            Down,
            Up
        }

        internal MockInputSimulator()
        {
            KeyActions = new List<KeyActionChange>();
            keyStates = new Dictionary<VirtualKeyCode, KeyState>();

            Keyboard = this;
            InputDeviceState = this;

            stringBuilder = new StringBuilder();
        }

        internal List<KeyActionChange> KeyActions { get; set; }

        internal string GetTextInput()
        {
            return stringBuilder.ToString();
        }

        internal void ClearState()
        {
            keyStates.Clear();
            KeyActions.Clear();
            stringBuilder.Clear();
            isCapsLockOn = false;
        }

        #region IInputSimulator Implementation
        public IKeyboardSimulator Keyboard { get; set; }

        public IMouseSimulator Mouse => throw new NotImplementedException();

        public IInputDeviceStateAdaptor InputDeviceState { get; set; }
        #endregion

        #region IKeyboardSimulator Implementation
        public IKeyboardSimulator KeyDown(VirtualKeyCode keyCode)
        {
            KeyActions.Add(new KeyActionChange(keyCode, KeyAction.Down));
            TrackKeyDown(keyCode);
            return this;
        }

        public IKeyboardSimulator KeyPress(VirtualKeyCode keyCode)
        {
            KeyActions.Add(new KeyActionChange(keyCode, KeyAction.Press));
            if (keyCode == VirtualKeyCode.CAPITAL)
            {
                isCapsLockOn = !isCapsLockOn;
            }

            return this;
        }

        public IKeyboardSimulator KeyPress(params VirtualKeyCode[] keyCodes)
        {
            foreach (var keyCode in keyCodes)
            {
                KeyActions.Add(new KeyActionChange(keyCode, KeyAction.Press));
                if (keyCode == VirtualKeyCode.CAPITAL)
                {
                    isCapsLockOn = !isCapsLockOn;
                }
            }

            return this;
        }

        public IKeyboardSimulator KeyUp(VirtualKeyCode keyCode)
        {
            KeyActions.Add(new KeyActionChange(keyCode, KeyAction.Up));
            TrackKeyUp(keyCode);
            return this;
        }

        public IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, IEnumerable<VirtualKeyCode> keyCodes)
        {
            foreach (var key in modifierKeyCodes)
            {
                KeyDown(key);
            }

            foreach(var key in keyCodes)
            {
                KeyPress(key);
            }

            foreach (var key in modifierKeyCodes.Reverse())
            {
                KeyUp(key);
            }
            return this;
        }

        public IKeyboardSimulator ModifiedKeyStroke(IEnumerable<VirtualKeyCode> modifierKeyCodes, VirtualKeyCode keyCode)
        {
            foreach (var key in modifierKeyCodes)
            {
                KeyDown(key);
            }

            KeyPress(keyCode);

            foreach(var key in modifierKeyCodes.Reverse())
            {
                KeyUp(key);
            }
            return this;
        }

        public IKeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKey, IEnumerable<VirtualKeyCode> keyCodes)
        {
            KeyDown(modifierKey);
            foreach (var key in keyCodes)
            {
                KeyPress(key);
            }
            KeyUp(modifierKey);

            return this;
        }

        public IKeyboardSimulator ModifiedKeyStroke(VirtualKeyCode modifierKeyCode, VirtualKeyCode keyCode)
        {
            KeyDown(modifierKeyCode);
            KeyPress(keyCode);
            KeyUp(modifierKeyCode);

            return this;
        }

        public IKeyboardSimulator Sleep(int millsecondsTimeout)
        {
            throw new NotImplementedException();
        }

        public IKeyboardSimulator Sleep(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public IKeyboardSimulator TextEntry(string text)
        {
            stringBuilder.Append(text);
            return this;
        }

        public IKeyboardSimulator TextEntry(char character)
        {
            stringBuilder.Append(character);
            return this;
        }
        #endregion

        #region IInputDeviceStateAdaptor Implementation
        public bool IsHardwareKeyDown(VirtualKeyCode keyCode)
        {
            return IsKeyDown(keyCode);
        }

        public bool IsHardwareKeyUp(VirtualKeyCode keyCode)
        {
            return IsKeyUp(keyCode);
        }

        public bool IsKeyDown(VirtualKeyCode keyCode)
        {
            if (keyStates.ContainsKey(keyCode))
            {
                return keyStates[keyCode] == KeyState.Down;
            }
            return false;
        }

        public bool IsKeyUp(VirtualKeyCode keyCode)
        {
            if (keyStates.ContainsKey(keyCode))
            {
                return keyStates[keyCode] == KeyState.Up;
            }
            return true;
        }

        public bool IsTogglingKeyInEffect(VirtualKeyCode keyCode)
        {
            return isCapsLockOn;
        }
        #endregion

        private void TrackKeyDown(VirtualKeyCode keyCode)
        {
            if (!keyStates.ContainsKey(keyCode))
            {
                keyStates.Add(keyCode, KeyState.Down);
            }
            else
            {
                keyStates[keyCode] = KeyState.Down;
            }
        }

        private void TrackKeyUp(VirtualKeyCode keyCode)
        {
            if (!keyStates.ContainsKey(keyCode))
            {
                keyStates.Add(keyCode, KeyState.Up);
            }
            else
            {
                keyStates[keyCode] = KeyState.Up;
            }
        }
    }
}
