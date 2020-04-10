using UnityEngine;

namespace Assets.Source.InputManagement
{
    /// <summary>
    /// Event with information about input, sent to InputManager subscribers
    /// </summary>
    public class InputEvent
    {
        /// <summary>
        /// Object that was clicked
        /// </summary>
        public GameObject InputObject;
    }
}
