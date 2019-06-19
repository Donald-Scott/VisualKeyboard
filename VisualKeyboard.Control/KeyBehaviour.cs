
namespace VisualKeyboard.Control
{
    public enum KeyBehaviour
    {
        /// <summary>
        /// No logical key behaviour
        /// </summary>
        None = 0,

        /// <summary>
        /// Virtual key.  This type of key provides a single virtual key code
        /// to simualate keyboard input.
        /// </summary>
        VirtualKey,

        /// <summary>
        /// This key type represents a modifier key that automaticly reverts to not-in-effect
        /// when another key is pressed or the same modfier key is pressed a second time.
        /// </summary>
        InstantaneousModifier,

        /// <summary>
        /// this key type represents a modifier key that remains in effect until the same 
        /// key is pressed a second time.
        /// </summary>
        TogglingModifier,

        /// <summary>
        /// This key represents one or more modifier keys and a non-modifer key, ex CTL+C or ALT+TAB
        /// </summary>
        Chord,

        /// <summary>
        /// This key represents a sequence of not modifier keys.
        /// </summary>
        Text
    }
}