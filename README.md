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

### Usage
#### Include the Visual Keyboard namespace
```XML
xmlns:kbd="clr-namespace:VisualKeyboard.Control;assembly=VisualKeyboard.Control"
```

```XML
<kbd:Keyboard>
  <Grid.ColumnDefinitions>
      <ColumnDefinition Width="1.5*"/>
      <ColumnDefinition Width="1.5*"/>
      <ColumnDefinition/>
      <ColumnDefinition/>
      <ColumnDefinition/>
  </Grid.ColumnDefinitions>
  <Button Grid.Row="0" Grid.Column="0" Content="CapsLock"  kbd:Keyboard.KeyBehaviour="TogglingModifier" kbd:Keyboard.KeyCode="Capital"/>
  <Button Grid.Row="0" Grid.Column="1" Content="Shift" kbd:Keyboard.KeyBehaviour="InstantaneousModifier" kbd:Keyboard.KeyCode="Shift"/>
  <Button Grid.Row="0" Grid.Column="2" Content="q" kbd:Keyboard.KeyBehaviour="VirtualKey" kbd:Keyboard.KeyCode="VkQ"/>
  <Button Grid.Row="0" Grid.Column="3" Content="w" kbd:Keyboard.KeyBehaviour="VirtualKey" kbd:Keyboard.KeyCode="VkW"/>
  <Button Grid.Row="0" Grid.Column="4" Content="e" kbd:Keyboard.KeyBehaviour="VirtualKey" kbd:Keyboard.KeyCode="VkE"/>
</kbd:Keyboard>
```
![alt text](https://github.com/Donald-Scott/VisualKeyboard/blob/master/VisualKeyboard.Examples/Images/qwe_kbd.png "Sample keyboard")
