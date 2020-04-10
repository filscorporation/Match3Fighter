using UnityEngine;

namespace Assets.Source.InputManagement
{
    /// <summary>
    /// PC mouse input
    /// </summary>
    public class MouseInputManager : InputManagerBase
    {
        protected override void CheckForInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProcessInputBegin(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                ProcessInputEnd(Input.mousePosition);
            }
        }
    }
}
