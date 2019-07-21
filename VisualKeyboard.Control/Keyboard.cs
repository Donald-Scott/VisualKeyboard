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

        public static VirtualKeyCode GetKeyCode(DependencyObject property)
        {
            if (property == null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            return (VirtualKeyCode)property.GetValue(KeyCodeProperty);
        }

        public static void SetKeyCode(DependencyObject property, VirtualKeyCode value)
        {
            if (property == null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            property.SetValue(KeyCodeProperty, value);
        }


        public static readonly DependencyProperty KeyCodeProperty =
            DependencyProperty.RegisterAttached("KeyCode", typeof(VirtualKeyCode), typeof(Keyboard), new PropertyMetadata(VirtualKeyCode.None, KeyCodeChanged));

        public static KeyBehaviour GetKeyBehaviour(DependencyObject property)
        {
            if (property == null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            return (KeyBehaviour)property.GetValue(KeyBehaviourProperty);
        }

        public static void SetKeyBehaviour(DependencyObject property, KeyBehaviour value)
        {
            if (property == null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            property.SetValue(KeyBehaviourProperty, value);
        }

        public static readonly DependencyProperty KeyBehaviourProperty =
            DependencyProperty.RegisterAttached("KeyBehaviour", typeof(KeyBehaviour), typeof(Keyboard), new PropertyMetadata(KeyBehaviour.None, KeyBehaviourChanged));

        public static VirtualKeyCollection GetChordKeys(DependencyObject property)
        {
            if (property == null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            var collection = (VirtualKeyCollection)property.GetValue(ChordKeysProperty);
            if (collection == null)
            {
                collection = new VirtualKeyCollection();
                property.SetValue(ChordKeysProperty, collection);
            }
            return collection;
        }

        public static void SetChordKeys(DependencyObject property, VirtualKeyCollection value)
        {
            if (property == null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            property.SetValue(ChordKeysProperty, value);
        }

        public static readonly DependencyProperty ChordKeysProperty =
            DependencyProperty.RegisterAttached("ChordKeysInternal", typeof(VirtualKeyCollection), typeof(Keyboard), new PropertyMetadata(null, ChordKeysChanged));

        public static string GetOutputText(DependencyObject property)
        {
            if (property == null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            return (string)property.GetValue(OutputTextProperty);
        }

        public static void SetOutputText(DependencyObject property, string value)
        {
            if (property == null)
            {
                throw new System.ArgumentNullException(nameof(property));
            }

            property.SetValue(OutputTextProperty, value);
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
            if (sender is UIElement keyboardElement)
            {
                ProcessGenericKeyPress(keyboardElement);
            };
        }

        /// <summary>
        /// Recieves notification of a button click event from a child control of the keyboard.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonKeyClicked(object sender, RoutedEventArgs e)
        {
            if (sender is UIElement keyboardElement)
            {
                ProcessGenericKeyPress(keyboardElement);
            }
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
            if (child is ButtonBase button)
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
            if (sender is ModifierKeyBase modifierKey)
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

            if (!(VisualTreeHelper.GetParent(child) is Keyboard parentKeyboard) || parentKeyboard.keys == null)
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
                    result = CreateVirtualKey(keyCode, inputSimulator);
                    break;
                case KeyBehaviour.Chord:
                    result = CreateChordKey(keyCode, inputSimulator, chordKeys);
                    break;
                case KeyBehaviour.Text:
                    result = CreateTextKey(keyCode, inputSimulator, outputText);
                    break;
                case KeyBehaviour.InstantaneousModifier:
                    result = CreateInstantaneousModifierKey(keyCode, inputSimulator);
                    break;
                case KeyBehaviour.TogglingModifier:
                    result = CreateTogglingModifierKey(keyCode, inputSimulator);
                    break;
                default:
                    break;
            }

            return result;
        }

        private static VirtualKey CreateVirtualKey(VirtualKeyCode keyCode, IInputSimulator inputSimulator)
        {
            if (keyCode == VirtualKeyCode.None)
            {
                return null;
            }
            return new VirtualKey(inputSimulator, keyCode);
        }

        private static ChordKey CreateChordKey(VirtualKeyCode keyCode, IInputSimulator inputSimulator, VirtualKeyCollection chordKeys)
        {
            if (chordKeys == null || !chordKeys.Any())
            {
                return null;
            }
            return new ChordKey(inputSimulator, keyCode, chordKeys);
        }

        private static TextKey CreateTextKey(VirtualKeyCode keyCode, IInputSimulator inputSimulator, string outputText)
        {
            if (string.IsNullOrEmpty(outputText))
            {
                return null;
            }
            return new TextKey(inputSimulator, keyCode, outputText);
        }

        private static InstantaneousModifierKey CreateInstantaneousModifierKey(VirtualKeyCode keyCode, IInputSimulator inputSimulator)
        {
            if (keyCode == VirtualKeyCode.None)
            {
                return null;
            }
            return new InstantaneousModifierKey(inputSimulator, keyCode);
        }

        private static TogglingModifierKey CreateTogglingModifierKey(VirtualKeyCode keyCode, IInputSimulator inputSimulator)
        {
            if (keyCode == VirtualKeyCode.None)
            {
                return null;
            }
            return new TogglingModifierKey(inputSimulator, keyCode);
        }
    }
}
