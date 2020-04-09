using System;
using NetworkShared.Data.Field;
using UnityEngine;

namespace Assets.Source.FieldManagement
{
    /// <summary>
    /// Controls field generation and updating
    /// </summary>
    public class FieldManager : MonoBehaviour
    {
        private static FieldManager instance;

        public static FieldManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<FieldManager>();
                }
                return instance;
            }
        }

        public GameObject BlockPrefab;

        private Field mainField;
        private Field enemyField;

        /// <summary>
        /// Generates players field from data
        /// </summary>
        /// <param name="data"></param>
        public void GenerateMainField(FieldData data)
        {
            mainField = GenerateField(data, new Vector2(0, data.Blocks.GetLength(1) / 2F + 1));
        }

        /// <summary>
        /// Generates enemy field from data
        /// </summary>
        /// <param name="data"></param>
        public void GenerateEnemyField(FieldData data)
        {
            enemyField = GenerateField(data, new Vector2(0, -data.Blocks.GetLength(1) / 2F - 1));
        }

        private Field GenerateField(FieldData data, Vector2 center)
        {
            Field field = new Field();

            int w = data.Blocks.GetLength(0);
            int h = data.Blocks.GetLength(1);

            field.Blocks = new Block[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float x = i + center.x - w / 2F + 0.5F;
                    float y = j + center.y - h / 2F - 0.5F;
                    field.Blocks[i, j] = InstantiateBlock(x, y, (BlockTypes)data.Blocks[i, j].ID);
                }
            }

            return field;
        }

        private Block InstantiateBlock(float x, float y, BlockTypes type)
        {
            GameObject go = Instantiate(BlockPrefab, new Vector2(x, y), Quaternion.identity, transform);
            Block block = go.GetComponent<Block>();

            block.Type = type;

            // TODO: textures and dictionary
            SpriteRenderer sprite = block.GetComponent<SpriteRenderer>();
            switch (block.Type)
            {
                case BlockTypes.Attack:
                    sprite.color = Color.red;
                    break;
                case BlockTypes.Mana:
                    sprite.color = Color.blue;
                    break;
                case BlockTypes.Health:
                    sprite.color = Color.green;
                    break;
                case BlockTypes.Arcane:
                    sprite.color = Color.magenta;
                    break;
                case BlockTypes.Chameleon:
                    sprite.color = Color.white;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return block;
        }
    }
}
