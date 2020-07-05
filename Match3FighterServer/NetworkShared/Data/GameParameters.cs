using System;

namespace NetworkShared.Data
{
    public enum PracticeMode
    {
        /// <summary>
        /// Bot will do basic moves
        /// </summary>
        Active,

        /// <summary>
        /// Bot doesn't do anything
        /// </summary>
        Passive,
    }

    [Serializable]
    public class GameParameters
    {
        public PracticeMode PracticeMode;
    }
}
