using System;
using Assets.Source.GameManagement;
using Assets.Source.InputManagement;
using NetworkShared.Data.Field;
using UnityEngine;

namespace Assets.Source.FieldManagement
{
    /// <summary>
    /// Controls field generation and updating
    /// </summary>
    public class FieldManager : MonoBehaviour, IInputSubscriber
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

        public void Start()
        {
            AutoInputInitializer.InputManager.Subscribe(this);
        }

        #region Field Management

        /// <summary>
        /// Deletes both fields
        /// </summary>
        public void DeleteFields()
        {
            DeleteField(mainField);
            DeleteField(enemyField);
        }

        private void DeleteField(Field field)
        {
            int w = field.Blocks.GetLength(0);
            int h = field.Blocks.GetLength(1);
            
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Destroy(field.Blocks[i, j]);
                }
            }
        }

        /// <summary>
        /// Generates players field from data
        /// </summary>
        /// <param name="data"></param>
        public void GenerateMainField(FieldData data)
        {
            mainField = GenerateField(data, FieldType.Player, new Vector2(0, -data.Blocks.GetLength(1) / 2F));
        }

        /// <summary>
        /// Generates enemy field from data
        /// </summary>
        /// <param name="data"></param>
        public void GenerateEnemyField(FieldData data)
        {
            enemyField = GenerateField(data, FieldType.Enemy, new Vector2(0, data.Blocks.GetLength(1) / 2F + 1));
        }

        private Field GenerateField(FieldData data, FieldType type, Vector2 center)
        {
            Field field = new Field(type);

            int w = data.Blocks.GetLength(0);
            int h = data.Blocks.GetLength(1);

            field.Blocks = new Block[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float x = i + center.x - w / 2F + 0.5F;
                    float y = j + center.y - h / 2F - 0.5F;
                    field.Blocks[i, j] = InstantiateBlock(x, y, i, j, (BlockTypes)data.Blocks[i, j].ID);
                    field.Blocks[i, j].Field = field;
                }
            }

            return field;
        }

        private Block InstantiateBlock(float x, float y, int i, int j, BlockTypes type)
        {
            GameObject go = Instantiate(BlockPrefab, new Vector2(x, y), Quaternion.identity, transform);
            Block block = go.GetComponent<Block>();

            block.Type = type;
            block.X = i;
            block.Y = j;

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

        #endregion

        #region Field Input

        public void Handle(InputEvent input)
        {
            if (input is BlockSwipeEvent swipe)
            {
                Block block = input.InputObject.GetComponent<Block>();
                if (block.Field.Type == FieldType.Enemy)
                {
                    return;
                }

                GameManager.Instance.OnPlayerBlockSwap(block.X, block.Y, swipe.Direction);
            }
        }

        #endregion
    }
}
