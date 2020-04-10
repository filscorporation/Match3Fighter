using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkShared.Data.Field;
using UnityEngine;

namespace Assets.Source.FieldManagement
{
    /// <summary>
    /// Field element
    /// </summary>
    public class Block : MonoBehaviour
    {
        public int X;

        public int Y;

        public BlockTypes Type;

        public Field Field;
    }
}
