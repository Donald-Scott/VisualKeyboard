using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using WindowsInput;

namespace VisualKeyboard.Control
{
    public class Keyboard : Grid
    {
        private Dictionary<UIElement, LogicalKey> keys;
        private List<ModifierKeyBase> modifierKeys;
        private IInputSimulator inputSimulator;

        public static VirtualKeyCode GetKeyCode(DependencyObject obj)
        {
            return (VirtualKeyCode)obj.GetValue(KeyCodeProperty);
        }

        public static void SetKeyCode(DependencyObject obj, VirtualKeyCode value)
        {
            obj.SetValue(KeyCodeProperty, value);
        }


        public static readonly DependencyProperty KeyCodeProperty =
            DependencyProperty.RegisterAttached("KeyCode", typeof(VirtualKeyCode), typeof(Keyboard), new PropertyMetadata(VirtualKeyCode.None, KeyCodeChanged));

        public static KeyBehaviour GetKeyBehaviour(DependencyObject obj)
        {
            return (KeyBehaviour)obj.GetValue(KeyBehaviourProperty);
        }

        public static void SetKeyBehaviour(DependencyObject obj, KeyBehaviour value)
        {
            obj.SetValue(KeyBehaviourProperty, value);
        }

        public static readonly DependencyProperty KeyBehaviourProperty =
            DependencyProperty.RegisterAttached("KeyBehaviour", typeof(KeyBehaviour), typeof(Keyboard), new PropertyMetadata(KeyBehaviour.None, KeyBehaviourChanged));

        public static VirtualKeyCollection GetChordKeys(DependencyObject obj)
        {
            var collection = (VirtualKeyCollection)obj.GetValue(ChordKeysProperty);
            if (collection == null)
            {
                collection = new VirtualKeyCollection();
                obj.SetValue(ChordKeysProperty, collection);
            }
            return collection;
        }

        public static void SetChordKeys(DependencyObject obj, VirtualKeyCollection value)
        {
            obj.SetValue(ChordKeysProperty, value);
        }

        public static readonly DependencyProperty ChordKeysProperty =
            DependencyProperty.RegisterAttached("ChordKeysInternal", typeof(VirtualKeyCollection), typeof(Keyboard), new PropertyMetadata(null, ChordKeysChanged));

        public static string GetOutputText(DependencyObject obj)
        {
            return (string)obj.GetValue(OutputTextProperty);
        }

        public static void SetOutputText(DependencyObject obj, string value)
        {
            obj.SetValue(OutputTextProperty, value);
        }

        public static readonly DependencyProperty OutputTextProperty =
            DependencyProperty.RegisterAttached("OutputText", typeof(string), typeof(Keyboard), new PropertyMetadata(string.Empty, OutputTextChanged));

        public static readonly RoutedEvent ModifierChangedEvent = EventManager.RegisterRoutedEvent("ModifierChanged", RoutingStrategy.Bubble, 
            typeof(ModifierChangedEventHandler), typeof(Keyboard));

        // Provide CLR accessors for the event
        public event ModifierChangedEventHandler ModifierChanged
        {
            add { AddHandler(ModifierChangedEvent, value); }
            remove { RemoveHandler(ModifierChangedEvent, value); }
        }

        public override void EndInit()
        {
            inputSimulator = CreateInputSimulator();
            keys = new Dictionary<UIElement, LogicalKey>();
            modifierKeys = new List<ModifierKeyBase>();

            foreach (UIElement child in Children)
            {
                VirtualKeyCode keyCode = (VirtualKeyCode)child.GetValue(KeyCodeProperty);
                KeyBehaviour keyBehaviour = (KeyBehaviour)child.GetValue(KeyBehaviourProperty);
                VirtualKeyCollection chordKeys = (VirtualKeyCollection)child.GetValue(ChordKeysProperty);
                string outputText = (string)child.GetValue(OutputTextProperty);

                if (keyBehaviour == KeyBehaviour.None)
                {
                    // skip to the next child element
                    continue;
                }

                LogicalKey key = CreateKey(keyCode, inputSimulator, keyBehaviour, chordKeys, outputText);
                if (key == null)
                {
                    // skip to the next child element
                    continue;
                }

                ConnectToChildControl(child);
                keys.Add(child, key);
                key.KeyPressed += LogicalKeyPressed;

                if (keyBehaviour == KeyBehaviour.InstantaneousModifier || keyBehaviour == KeyBehaviour.TogglingModifier)
                {
                    modifierKeys.Add(key as ModifierKeyBase);
                }
            }

            SynchroniseModifierKeyState();
            base.EndInit();
        }

        /// <summary>
        /// Instruct all modifier keys to synchronise their state with the operating system\hardware.
        /// </summary>
        public void SynchroniseModifierKeyState()
        {
            modifierKeys.ToList().ForEach(x => x.SynchroniseKeyState());
        }

        protected virtual IInputSimulator CreateInputSimulator()
        {
            return new InputSimulator();
        }

        /// <summary>
        /// Recieves notification of the left mouse button down event from a child control of the keyboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UIElement keyboardElement = sender as UIElement;
            if (keyboardElement == null)
            {
                return;
            }
            ProcessGenericKeyPress(keyboardElement);
        }

        /// <summary>
        /// Recieves notification of a button click event from a child control of the keyboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonKeyClicked(object sender, RoutedEventArgs e)
        {
            UIElement keyboardElement = sender as UIElement;
            if (keyboardElement == null)
            {
                return;
            }
            ProcessGenericKeyPress(keyboardElement);
        }

        /// <summary>
        /// Processes the event from a child control when is has been clicked.  The logical
        /// key that is associated with the child control is obtained and its "ScreenKeyPress"
        /// method is called.
        /// </summary>
        /// <param name="keyboardElement"></param>
        private void ProcessGenericKeyPress(UIElement keyboardElement)
        {
            if (!keys.ContainsKey(keyboardElement))
            {
                return;
            }

            LogicalKey key = keys[keyboardElement];
            key.ScreenKeyPress();
        }

        /// <summary>
        /// Based on the type of UIElement, start listinging for event notifications that indicate
        /// the key has been pressed.
        /// </summary>
        /// <param name="child"></param>
        private void ConnectToChildControl(UIElement child)
        {
            //Controls derived from ButtonBase have specific code to determine when the control is clicked
            //use the Click event if it is available.  If the child element is not a button then use it's
            //left mouse button down event to trigger the key press logic
            var button = child as ButtonBase;
            if (button != null)
            {
                button.Click += ButtonKeyClicked;
            }
            else
            {
                child.MouseLeftButtonDown += KeyMouseLeftButtonDown;
            }
        }

        /// <summary>
        /// Receives notificaton from a logical key that it has been pressed.  The keyboard control
        /// uses this notificatoin to monitor and reset modifier keys if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogicalKeyPressed(object sender, LogicalKeyEventArgs e)
        {
            ModifierKeyBase modifierKey = sender as ModifierKeyBase;
            if (modifierKey != null)
            {
                RaiseModifierChangedEvent((VirtualKeyCode)modifierKey.KeyCode, modifierKey.IsInEffect);
            }
            else // not a modifier key
            {
                ResetInstantaneousModifierKeys();
            }
            modifierKeys.OfType<InstantaneousModifierKey>().ToList().ForEach(x => x.SynchroniseKeyState());
        }

        /// <summary>
        /// If an Instantaneous Modifier Key is in effect then press it to turn it off
        /// </summary>
        private void ResetInstantaneousModifierKeys()
        {
            modifierKeys.OfType<InstantaneousModifierKey>().ToList().ForEach(key =>
            {
                if (key.IsInEffect)
                {
                    key.ScreenKeyPress();
                }
            });
        }

        // This method raises the RaiseModifierChange event
        private void RaiseModifierChangedEvent(VirtualKeyCode keyCode, bool isInEffect)
        {
            ModifierChangedEventArgs newEventArgs = new ModifierChangedEventArgs(ModifierChangedEvent, keyCode, isInEffect);
            RaiseEvent(newEventArgs);
        }

        private static void RecreateKey(UIElement child)
        {
            if (child == null)
            {
                return;
            }

            Keyboard parentKeyboard = VisualTreeHelper.GetParent(child) as Keyboard;
            if (parentKeyboard == null || parentKeyboard.keys == null)
            {
                return;
            }

            // make sure key exist. If it's behavour is none it will not exist
            if (parentKeyboard.keys.ContainsKey(child))
            {
                LogicalKey oldKey = parentKeyboard.keys[child];

                parentKeyboard.keys.Remove(child);
                parentKeyboard.modifierKeys.Remove(oldKey as ModifierKeyBase);
            }

            VirtualKeyCode keyCode = (VirtualKeyCode)child.GetValue(KeyCodeProperty);
            KeyBehaviour keyBehaviour = (KeyBehaviour)child.GetValue(KeyBehaviourProperty);
            VirtualKeyCollection chordKeys = (VirtualKeyCollection)child.GetValue(ChordKeysProperty);
            string outputText = (string)child.GetValue(OutputTextProperty);

            if (keyBehaviour == KeyBehaviour.None)
            {
                // do not create a new key
                return;
            }

            LogicalKey key = CreateKey(keyCode, parentKeyboard.inputSimulator, keyBehaviour, chordKeys, outputText);

            if (key == null)
            {
                return;
            }

            //parentKeyboard.ConnectToChildControl(child);
            parentKeyboard.keys.Add(child, key);
            key.KeyPressed += parentKeyboard.LogicalKeyPressed;

            if (keyBehaviour == KeyBehaviour.InstantaneousModifier || keyBehaviour == KeyBehaviour.TogglingModifier)
            {
                parentKeyboard.modifierKeys.Add(key as ModifierKeyBase);
            }
        }

        private static void KeyCodeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RecreateKey(obj as UIElement);
        }

        private static void KeyBehaviourChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RecreateKey(obj as UIElement);
        }

        private static void ChordKeysChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RecreateKey(obj as UIElement);
        }

        private static void OutputTextChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RecreateKey(obj as UIElement);
        }

        private static LogicalKey CreateKey(VirtualKeyCode keyCode, IInputSimulator inputSimulator, KeyBehaviour keyType, VirtualKeyCollection chordKeys, string outputText)
        {
            LogicalKey result = null;

            switch (keyType)
            {
                case KeyBehaviour.VirtualKey:
                    if (keyCode != VirtualKeyCode.None)
                    {
                        result = new VirtualKey(inputSimulator, keyCode);
                    }
                    break;
                case KeyBehaviour.Chord:
                    if (chordKeys != null && chordKeys.Any())
                    {
                        result = new ChordKey(inputSimulator, keyCode, chordKeys);
                    }
                    break;
                case KeyBehaviour.Text:
                    if (!string.IsNullOrEmpty(outputText))
                    {
                        result = new TextKey(inputSimulator, keyCode, outputText);
                    }
                    break;
                case KeyBehaviour.InstantaneousModifier:
                    if (keyCode != VirtualKeyCode.None)
                    {
                        result = new InstantaneousModifierKey(inputSimulator, keyCode);
                    }
                    break;
                case KeyBehaviour.TogglingModifier:
                    if (keyCode != VirtualKeyCode.None)
                    {
                        result = new TogglingModifierKey(inputSimulator, keyCode);
                    }
                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
