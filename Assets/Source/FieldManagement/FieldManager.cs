using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Source.GameManagement;
using Assets.Source.InputManagement;
using NetworkShared.Data.Effects;
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

        public bool CanControl = true;
        
        public GameObject BlockPrefab;
        public GameObject ShootEffectPrefab;

        private Field mainField;
        private Field enemyField;

        public List<Sprite> BlockSprites;
        private const string uniqueBlockSpritesPath = "UniqueBlockSprites";
        private const string onBlockEffectsSpritesPath = "Prefabs";
        private const string onBlockEffectsSpritesPostfix = "Effect";

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
            mainField.InGameID = data.InGameID;
        }

        /// <summary>
        /// Generates enemy field from data
        /// </summary>
        /// <param name="data"></param>
        public void GenerateEnemyField(FieldData data)
        {
            enemyField = GenerateField(data, FieldType.Enemy, new Vector2(0, data.Blocks.GetLength(1) / 2F + 1));
            enemyField.InGameID = data.InGameID;
        }

        private Vector2 GetPositionForPlayerBlock(int i, int j)
        {
            float w = mainField.Blocks.GetLength(0);
            float h = mainField.Blocks.GetLength(1);
            Vector2 center = new Vector2(0, -h / 2F);
            float x = i + center.x - w / 2F + 0.5F;
            float y = j + center.y - h / 2F - 0.5F;
            return new Vector2(x, y);
        }

        private Vector2 GetPositionForEnemyBlock(int i, int j)
        {
            float w = enemyField.Blocks.GetLength(0);
            float h = enemyField.Blocks.GetLength(1);
            Vector2 center = new Vector2(0, h / 2F + 1);
            float x = i + center.x - w / 2F + 0.5F;
            float y = j + center.y - h / 2F - 0.5F;
            return new Vector2(x, y);
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
                                StartCoroutine(block.AnimateDropped(new Vector2(x, dropY + dropYoffset[i] + 1), true));
                                dropYoffset[i]++;
                            }

                            // Block was moved
                            if (prevState.State == BlockState.Moved)
                            {
                                float oldx = prevState.X + center.x - w / 2F + 0.5F;
                                float oldy = prevState.Y + center.y - h / 2F - 0.5F;
                                StartCoroutine(block.AnimateDropped(new Vector2(oldx, oldy)));
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
                                StartCoroutine(block.AnimateAppeared());
                            }

                            // Show if previous was destroyed by damage
                            if (prevState.State == BlockState.DestroyedByDamage)
                            {
                                StartCoroutine(block.AnimateDestroyed());
                            }

                            // Show if previous was destroyed as combo part
                            if (prevState.State == BlockState.DestroyedAsCombo)
                            {
                                StartCoroutine(block.AnimateDestroyed());
                            }

                            // Show if previous was flipped over
                            if (prevState.State == BlockState.FlippedOver)
                            {
                                StartCoroutine(block.AnimateFlipped());
                            }

                            // Show if previous was created on flipped one place
                            if (prevState.State == BlockState.CreatedFromFlip)
                            {
                                StartCoroutine(block.AnimateAppeared());
                            }

                            if (blockData.ReplacedBlock != null)
                            {
                                float oldx = blockData.ReplacedBlock.X + center.x - w / 2F + 0.5F;
                                float oldy = blockData.ReplacedBlock.Y + center.y - h / 2F - 0.5F;
                                Block destroyedBlock = InstantiateBlock(field, oldx, oldy, i, j, blockData.ReplacedBlock);
                                
                                AnimateAllBlockTransitions(destroyedBlock, blockData.ReplacedBlock);
                            }
                        }
                    }

                    Block newBlock = InstantiateBlock(field, x, y, i, j, data.Blocks[i, j]);
                    field.Blocks[i, j] = newBlock;

                    AnimateAllBlockTransitions(newBlock, data.Blocks[i, j]);
                }
            }

            AutoInputInitializer.InputManager.FreezeFor(0.5F);

            return field;
        }

        private Block InstantiateBlock(Field field, float x, float y, int i, int j, BlockData data)
        {
            GameObject go = Instantiate(BlockPrefab, new Vector2(x, y), Quaternion.identity, transform);
            Block block = go.GetComponent<Block>();
            go.name = $"Block {i} {j}";
            block.Field = field;
            if (field.Type == FieldType.Player)
                block.SetPlayerLayer();
            else
                block.SetEnemyLayer();

            block.Type = (BlockTypes)data.ID;
            block.X = i;
            block.Y = j;
            
            SpriteRenderer sprite = block.GetComponent<SpriteRenderer>();
            if (!string.IsNullOrWhiteSpace(data.UniqueBlock))
            {
                sprite.sprite = GetUniqueBlockSprite(data.UniqueBlock);
            }
            else
            {
                sprite.sprite = BlockSprites[data.ID];
            }

            foreach (OnBlockEffectData onBlockEffectData in data.OnBlockEffects)
            {
                OnBlockEffect effect = new OnBlockEffect(onBlockEffectData.Type, onBlockEffectData.Duration);
                effect.Prefab = Instantiate(
                    GetEffectPrefab(onBlockEffectData.Type),
                    block.transform.position,
                    Quaternion.identity,
                    block.transform);
                block.OnBlockEffects.Add(effect);
            }

            return block;
        }

        private GameObject GetEffectPrefab(OnBlockEffectType type)
        {
            return Resources.Load<GameObject>(
                Path.Combine(onBlockEffectsSpritesPath, type + onBlockEffectsSpritesPostfix));
        }

        private Sprite GetUniqueBlockSprite(string blockName)
        {
            return Resources.Load<Sprite>(Path.Combine(uniqueBlockSpritesPath, blockName));
        }

        /// <summary>
        /// Draws effect of block shooting another block
        /// </summary>
        /// <param name="data"></param>
        public void DrawShootEffect(EffectData data)
        {
            Vector2 from = (int)data.Data["InitField"] == mainField.InGameID ?
                GetPositionForPlayerBlock((int)data.Data["InitX"], (int)data.Data["InitY"]) :
                GetPositionForEnemyBlock((int)data.Data["TargetX"], (int)data.Data["TargetY"]);
            Vector2 to = (int)data.Data["TargetField"] == enemyField.InGameID ?
                GetPositionForEnemyBlock((int)data.Data["TargetX"], (int)data.Data["TargetY"]) :
                GetPositionForPlayerBlock((int)data.Data["InitX"], (int)data.Data["InitY"]);
            GameObject go = Instantiate(ShootEffectPrefab, Vector3.zero, Quaternion.identity, transform);
            StartCoroutine(go.GetComponent<IPointToPointEffect>().Initialize(from, to));
        }

        #endregion

        #region Field Input

        public void Handle(InputEvent input)
        {
            if (!CanControl)
                return;

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
