using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Sprite> BlockSprites;

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
                    Destroy(field.Blocks[i, j].gameObject);
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

            float dropY = h + center.y - h / 2F - 0.5F;
            int[] dropYoffset = new int[w];
            for (int i = 0; i < w; i++)
            {
                dropYoffset[i] = 0;
            }

            field.Blocks = new Block[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float x = i + center.x - w / 2F + 0.5F;
                    float y = j + center.y - h / 2F - 0.5F;

                    void AnimateAllBlockTransitions(Block block, BlockData blockData)
                    {
                        if (!(blockData.PreviousStates?.Any() ?? false))
                            return;

                        foreach (BlockStateData prevState in blockData.PreviousStates)
                        {
                            // Block dropped from above as new
                            if (prevState.State == BlockState.DroppedAsNew)
                            {
                                block.AnimateDropped(new Vector2(x, dropY + dropYoffset[i] + 1), true);
                                dropYoffset[i]++;
                            }

                            // Block was moved
                            if (prevState.State == BlockState.Moved)
                            {
                                float oldx = prevState.X + center.x - w / 2F + 0.5F;
                                float oldy = prevState.Y + center.y - h / 2F - 0.5F;
                                block.AnimateDropped(new Vector2(oldx, oldy));
                            }

                            // Blocks that swapped
                            if (prevState.State == BlockState.Swapped)
                            {
                                float oldx = prevState.X + center.x - w / 2F + 0.5F;
                                float oldy = prevState.Y + center.y - h / 2F - 0.5F;
                                block.AnimateSwap(new Vector2(oldx, oldy));
                            }

                            // Adding block that was generated from combo
                            if (prevState.State == BlockState.CreatedAsComboResult)
                            {
                                block.AnimateAppeared();
                            }

                            // Show if previous was destroyed by damage
                            if (prevState.State == BlockState.DestroyedByDamage)
                            {
                                block.AnimateDestroyed();
                            }

                            // Show if previous was destroyed as combo part
                            if (prevState.State == BlockState.DestroyedAsCombo)
                            {
                                block.AnimateDestroyed();
                            }

                            if (blockData.ReplacedBlock != null)
                            {
                                float oldx = blockData.ReplacedBlock.X + center.x - w / 2F + 0.5F;
                                float oldy = blockData.ReplacedBlock.Y + center.y - h / 2F - 0.5F;
                                Block destroyedBlock = InstantiateBlock(field, oldx, oldy, i, j, (BlockTypes)blockData.ReplacedBlock.ID);
                                
                                AnimateAllBlockTransitions(destroyedBlock, blockData.ReplacedBlock);
                            }
                        }
                    }

                    Block newBlock = InstantiateBlock(field, x, y, i, j, (BlockTypes)data.Blocks[i, j].ID);
                    field.Blocks[i, j] = newBlock;

                    AnimateAllBlockTransitions(newBlock, data.Blocks[i, j]);
                }
            }

            AutoInputInitializer.InputManager.FreezeFor(0.5F);

            return field;
        }

        private Block InstantiateBlock(Field field, float x, float y, int i, int j, BlockTypes type)
        {
            GameObject go = Instantiate(BlockPrefab, new Vector2(x, y), Quaternion.identity, transform);
            Block block = go.GetComponent<Block>();
            go.name = $"Block {i} {j}";
            block.Field = field;
            if (field.Type == FieldType.Player)
                block.SetPlayerLayer();
            else
                block.SetEnemyLayer();

            block.Type = type;
            block.X = i;
            block.Y = j;
            
            SpriteRenderer sprite = block.GetComponent<SpriteRenderer>();
            sprite.sprite = BlockSprites[(int) type];

            return block;
        }

        #endregion

        #region Field Input

        public void Handle(InputEvent input)
        {
            if (input is BlockSwipeEvent swipe)
            {
                Block block = input.InputObject.GetComponent<Block>();
                if (block.Field == null)
                {
                    // Destroyed block
                    return;
                }
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
