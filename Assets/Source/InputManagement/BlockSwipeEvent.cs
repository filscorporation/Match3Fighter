using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.InputManagement
{
    public enum BlockSwipeDirection
    {
        Top = 0,
        Right = 1,
        Bottom = 2,
        Left = 3,
    }

    /// <summary>
    /// Event when player swipes block
    /// </summary>
    public class BlockSwipeEvent : InputEvent
    {
        /// <summary>
        /// Direction where block was swiped
        /// </summary>
        public BlockSwipeDirection Direction;
    }
}
