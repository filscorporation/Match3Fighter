using UnityEngine;

namespace Assets.Source.FieldManagement
{
    public enum FieldType
    {
        Player,
        Enemy,
    }

    /// <summary>
    /// Stores one field info
    /// </summary>
    public class Field
    {
        public int InGameID;

        public Block[,] Blocks;

        public FieldType Type;

        public GameObject LockedFrame;

        public Field(FieldType type)
        {
            Type = type;
        }
    }
}
