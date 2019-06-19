using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Controls;
using VisualKeyboard.Control;

namespace VisualKeyboard.Tests
{
    [TestClass]
    public class KeyboardTest
    {
        [TestMethod]
        public void InputVirtualKeyA()
        {
            MockInputSimulator inputSimulator = new MockInputSimulator();
            TestabelKeyboard keyboard = CreateKeyboard(inputSimulator);

            MockButton btn = keyboard.Children[3] as MockButton;
            btn.ClickButton();

            Assert.AreEqual(1, inputSimulator.KeyActions.Count);
            Assert.AreEqual(WindowsInput.Native.VirtualKeyCode.VK_A, inputSimulator.KeyActions[0].KeyCode);
        }

        [TestMethod]
        public void InputVirtualKeyShiftA()
        {
            MockInputSimulator inputSimulator = new MockInputSimulator();
            TestabelKeyboard keyboard = CreateKeyboard(inputSimulator);

            MockButton shift = keyboard.Children[0] as MockButton;
            MockButton btn = keyboard.Children[3] as MockButton;

            shift.ClickButton();
            btn.ClickButton();

            Assert.AreEqual(3, inputSimulator.KeyActions.Count);
            Assert.AreEqual(WindowsInput.Native.VirtualKeyCode.SHIFT, inputSimulator.KeyActions[0].KeyCode);
            Assert.AreEqual(WindowsInput.Native.VirtualKeyCode.VK_A, inputSimulator.KeyActions[1].KeyCode);
            Assert.AreEqual(WindowsInput.Native.VirtualKeyCode.SHIFT, inputSimulator.KeyActions[2].KeyCode);
        }

        [TestMethod]
        public void InputTextKeyHello()
        {
            MockInputSimulator inputSimulator = new MockInputSimulator();
            TestabelKeyboard keyboard = CreateKeyboard(inputSimulator);

            MockButton btn = keyboard.Children[4] as MockButton;
            btn.ClickButton();

            Assert.AreEqual("Hello World!", inputSimulator.GetTextInput());
        }

        [TestMethod]
        public void InputCopyCommand()
        {
            MockInputSimulator inputSimulator = new MockInputSimulator();
            TestabelKeyboard keyboard = CreateKeyboard(inputSimulator);

            MockButton btn = keyboard.Children[5] as MockButton;
            btn.ClickButton();

            Assert.AreEqual(3, inputSimulator.KeyActions.Count);
            Assert.AreEqual(WindowsInput.Native.VirtualKeyCode.CONTROL, inputSimulator.KeyActions[0].KeyCode);
            Assert.AreEqual(WindowsInput.Native.VirtualKeyCode.VK_C, inputSimulator.KeyActions[1].KeyCode);
            Assert.AreEqual(WindowsInput.Native.VirtualKeyCode.CONTROL, inputSimulator.KeyActions[2].KeyCode);
        }

        [TestMethod]
        public void ChangeKeyAtoZ()
        {
            MockInputSimulator inputSimulator = new MockInputSimulator();
            TestabelKeyboard keyboard = CreateKeyboard(inputSimulator);

            MockButton btn = keyboard.Children[3] as MockButton;
            btn.ClickButton();

            Assert.AreEqual(1, inputSimulator.KeyActions.Count);
            Assert.AreEqual(WindowsInput.Native.VirtualKeyCode.VK_A, inputSimulator.KeyActions[0].KeyCode);

            inputSimulator.ClearState();

            Keyboard.SetKeyCode(btn, VirtualKeyCode.VkZ);
            btn.ClickButton();

            Assert.AreEqual(1, inputSimulator.KeyActions.Count);
            Assert.AreEqual(WindowsInput.Native.VirtualKeyCode.VK_Z, inputSimulator.KeyActions[0].KeyCode);
        }

        [TestMethod]
        public void ChangeTextKey()
        {
            MockInputSimulator inputSimulator = new MockInputSimulator();
            TestabelKeyboard keyboard = CreateKeyboard(inputSimulator);

            MockButton btn = keyboard.Children[4] as MockButton;
            btn.ClickButton();

            Assert.AreEqual("Hello World!", inputSimulator.GetTextInput());

            inputSimulator.ClearState();
            Keyboard.SetOutputText(btn, "Goodbye");

            btn.ClickButton();
            Assert.AreEqual("Goodbye", inputSimulator.GetTextInput());
        }

        private TestabelKeyboard CreateKeyboard(MockInputSimulator inputSimulator)
        {
            TestabelKeyboard keyboard = new TestabelKeyboard();
            keyboard.InputSimulator = inputSimulator;

            keyboard.BeginInit();

            keyboard.Width = 550;
            keyboard.Height = 350;

            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            ColumnDefinition col3 = new ColumnDefinition();
            ColumnDefinition col4 = new ColumnDefinition();
            ColumnDefinition col5 = new ColumnDefinition();
            ColumnDefinition col6 = new ColumnDefinition();

            keyboard.ColumnDefinitions.Add(col1);
            keyboard.ColumnDefinitions.Add(col2);
            keyboard.ColumnDefinitions.Add(col3);
            keyboard.ColumnDefinitions.Add(col4);
            keyboard.ColumnDefinitions.Add(col5);
            keyboard.ColumnDefinitions.Add(col6);

            MockButton btnShift = new MockButton();
            Keyboard.SetColumn(btnShift, 0);
            Keyboard.SetKeyBehaviour(btnShift, KeyBehaviour.InstantaneousModifier);
            Keyboard.SetKeyCode(btnShift, VirtualKeyCode.Shift);

            MockButton btnCaps = new MockButton();
            Keyboard.SetColumn(btnCaps, 1);
            Keyboard.SetKeyBehaviour(btnCaps, KeyBehaviour.TogglingModifier);
            Keyboard.SetKeyCode(btnCaps, VirtualKeyCode.Capital);

            MockButton btnCtrl = new MockButton();
            Keyboard.SetColumn(btnCtrl, 2);
            Keyboard.SetKeyBehaviour(btnCtrl, KeyBehaviour.InstantaneousModifier);
            Keyboard.SetKeyCode(btnCtrl, VirtualKeyCode.Control);

            MockButton btnA = new MockButton();
            Keyboard.SetColumn(btnA, 3);
            Keyboard.SetKeyBehaviour(btnA, KeyBehaviour.VirtualKey);
            Keyboard.SetKeyCode(btnA, VirtualKeyCode.VkA);

            MockButton btnHello = new MockButton();
            Keyboard.SetColumn(btnHello, 4);
            Keyboard.SetKeyBehaviour(btnHello, KeyBehaviour.Text);
            Keyboard.SetKeyCode(btnHello, VirtualKeyCode.None);
            Keyboard.SetOutputText(btnHello, "Hello World!");

            MockButton btnCopy = new MockButton();
            VirtualKeyCollection chordKeys = new VirtualKeyCollection();
            chordKeys.Add(VirtualKeyCode.Control);
            Keyboard.SetColumn(btnCopy, 5);
            Keyboard.SetKeyBehaviour(btnCopy, KeyBehaviour.Chord);
            Keyboard.SetKeyCode(btnCopy, VirtualKeyCode.VkC);
            Keyboard.SetChordKeys(btnCopy, chordKeys);

            keyboard.Children.Add(btnShift);
            keyboard.Children.Add(btnCaps);
            keyboard.Children.Add(btnCtrl);
            keyboard.Children.Add(btnA);
            keyboard.Children.Add(btnHello);
            keyboard.Children.Add(btnCopy);

            keyboard.EndInit();

            return keyboard;
        }
    }
}
