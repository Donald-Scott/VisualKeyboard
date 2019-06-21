WPF on screen keyboard
Inspired by [WPF Touch Screen Keyboard](https://github.com/imasm/wpfkb).

The visual keyboard control is a WPF control derived from Grid.  Attached properties allow child elements of the keyboard to behave as keys of the keyboard.

The WPF Touch Screen Keyboard was the inspiration for this control.  I felt the Touch Screen Keyboard's behavior was too tightly coupled to it's display logic.  This prompted me to create the Visual Keyboard control.

There are five different defined key behaviors:

- Virtual Keys - This key behavior generates input based on a specified virtual key code.
- Instantaneous Modifier Keys – This key behavior modifies the input for the next key code that is entered.  Examples of Instantaneous Modifier Keys are the Shift, Ctrl and Alt.
- Toggling Modifier Keys – This key behavior is similar to the Instantaneous Modifier Keys but the modifier remains in effect until it is turned off by pressing the key a second time.
- Chord Keys – This key behavior combines one or more modifier keys with a virtual key. A “Copy” command can be executed via a single key press by defining a chord key with a modifier of Ctrl plus a virtual key of “C”.
- Text Keys – This key behavior allows multiple text characters to be input via a single key.