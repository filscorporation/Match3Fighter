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

        public Transform PlayerFieldCenter;
        public float PlayerFieldScale = 1F;
        public Transform EnemyFieldCenter;
        public float EnemyFieldScale = 1F;

        private Field mainField;
        private Field enemyField;

        public List<Sprite> BlockSprites;
        public GameObject LockedFramePrefab;
        private GameObject lockedFrame;
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
            if (lockedFrame != null)
                Destroy(lockedFrame);
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
        /// Starts swap animation before getting server response
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="direction"></param>
        private void PreAnimateSwap(int x, int y, BlockSwipeDirection direction)
        {
            Block blockFrom = mainField.Blocks[x, y];
            Vector2Int newXY = InputManagerBase.GetPosition(x, y, direction);
            Block blockTo = mainField.Blocks[newXY.x, newXY.y];
            int w = mainField.Blocks.GetLength(0);
            int h = mainField.Blocks.GetLength(1);

            Vector2 oldPosition = blockFrom.transform.position;
            float newx = PlayerFieldCenter.position.x + (newXY.x - w / 2F + 0.5F) * PlayerFieldScale;
            float newy = PlayerFieldCenter.position.y + (newXY.y - h / 2F + 0.5F) * PlayerFieldScale;
            blockFrom.transform.position = new Vector3(newx, newy);
            blockFrom.AnimateSwap(oldPosition);

            oldPosition = blockTo.transform.position;
            newx = PlayerFieldCenter.position.x + (x - w / 2F + 0.5F) * PlayerFieldScale;
            newy = PlayerFieldCenter.position.y + (y - h / 2F + 0.5F) * PlayerFieldScale;
            blockFrom.transform.position = new Vector3(newx, newy);
            blockFrom.AnimateSwap(oldPosition);
        }

        /// <summary>
        /// Generates players field from data
        /// </summary>
        /// <param name="data"></param>
        public void GenerateMainField(FieldData data)
        {
            mainField = GenerateField(data, FieldType.Player, PlayerFieldCenter.position, PlayerFieldScale, PlayerFieldCenter);
            mainField.InGameID = data.InGameID;
        }

        /// <summary>
        /// Generates enemy field from data
        /// </summary>
        /// <param name="data"></param>
        public void GenerateEnemyField(FieldData data)
        {
            enemyField = GenerateField(data, FieldType.Enemy, EnemyFieldCenter.position, EnemyFieldScale, EnemyFieldCenter);
            enemyField.InGameID = data.InGameID;
        }

        private Vector2 GetPositionForBlock(int i, int j, Vector2 center, float scale)
        {
            float w = mainField.Blocks.GetLength(0);
            float h = mainField.Blocks.GetLength(1);
            float x = center.x + (i - w / 2F + 0.5F) * scale;
            float y = center.y + (j - h / 2F + 0.5F) * scale;
            return new Vector2(x, y);
        }

        private Field GenerateField(FieldData data, FieldType type, Vector2 center, float scale, Transform parent)
        {
            Field field = new Field(type);
            
            int w = data.Blocks.GetLength(0);
            int h = data.Blocks.GetLength(1);

            float dropY = center.y + (h / 2F + 0.5F) * scale;
            int[] dropYoffset = new int[w];
            for (int i = 0; i < w; i++)
            {
                dropYoffset[i] = 0;
            }

            List<Block> lockedBlocks = new List<Block>();

            field.Blocks = new Block[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    float x = center.x + (i - w / 2F + 0.5F) * scale;
                    float y = center.y + (j - h / 2F + 0.5F) * scale;

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
                                float oldx = center.x + (prevState.X - w / 2F + 0.5F) * scale;
                                float oldy = center.y + (prevState.Y - h / 2F + 0.5F) * scale;
                                StartCoroutine(block.AnimateDropped(new Vector2(oldx, oldy)));
                            }

                            // Blocks that swapped
                            if (prevState.State == BlockState.Swapped)
                            {
                                float oldx = center.x + (prevState.X - w / 2F + 0.5F) * scale;
                                float oldy = center.y + (prevState.Y - h / 2F + 0.5F) * scale;
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
                                float oldx = center.x + (blockData.ReplacedBlock.X - w / 2F + 0.5F) * scale;
                                float oldy = center.y + (blockData.ReplacedBlock.Y - h / 2F + 0.5F) * scale;
                                Block destroyedBlock = InstantiateBlock(field, oldx, oldy, i, j, blockData.ReplacedBlock, scale, parent);
                                
                                AnimateAllBlockTransitions(destroyedBlock, blockData.ReplacedBlock);
                            }
                        }
                    }

                    Block newBlock = InstantiateBlock(field, x, y, i, j, data.Blocks[i, j], scale, parent);
                    field.Blocks[i, j] = newBlock;
                    if (newBlock.IsLocked)
                        lockedBlocks.Add(newBlock);

                    AnimateAllBlockTransitions(newBlock, data.Blocks[i, j]);
                }
            }

            if (lockedBlocks.Any())
            {
                DrawLockedFrame(lockedBlocks, parent);
            }

            AutoInputInitializer.InputManager.FreezeFor(0.5F);

            return field;
        }

        private Block InstantiateBlock(Field field, float x, float y, int i, int j, BlockData data, float scale, Transform parent)
        {
            GameObject go = Instantiate(BlockPrefab, new Vector2(x, y), Quaternion.identity);
            go.transform.localScale = Vector3.one * scale;
            go.transform.SetParent(parent);
            Block block = go.GetComponent<Block>();
            go.name = $"Block {i} {j}";
            block.Field = field;
            if (field.Type == FieldType.Player)
                block.SetPlayerLayer();
            else
                block.SetEnemyLayer();

            block.Type = (BlockTypes)data.ID;
            block.IsLocked = data.IsLocked;
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

        private void DrawLockedFrame(List<Block> lockedBlocks, Transform parent)
        {
            lockedFrame = Instantiate(LockedFramePrefab);
            lockedFrame.transform.SetParent(parent);
            lockedFrame.transform.localScale = Vector3.one;
            lockedFrame.transform.SetAsFirstSibling();

            float leftMost = lockedBlocks.Min(b => b.transform.position.x);
            float rightMost = lockedBlocks.Max(b => b.transform.position.x);
            float downMost = lockedBlocks.Min(b => b.transform.position.y);
            float upMost = lockedBlocks.Max(b => b.transform.position.y);
            int verCount = (rightMost - leftMost) > (upMost - downMost) ? 1 : lockedBlocks.Count;
            int horCount = verCount == 1 ? lockedBlocks.Count : 1;
            lockedFrame.transform.position = new Vector3((rightMost + leftMost) / 2F, (upMost + downMost) / 2F);
            Vector2 baseSize = lockedFrame.GetComponent<SpriteRenderer>().size;
            lockedFrame.GetComponent<SpriteRenderer>().size =
                new Vector2(baseSize.x * horCount, baseSize.y * verCount);
        }

        /// <summary>
        /// Draws effect of block shooting another block
        /// </summary>
        /// <param name="data"></param>
        public void DrawShootEffect(EffectData data)
        {
            Vector2 from = (int)data.Data["InitField"] == mainField.InGameID ?
                GetPositionForBlock((int)data.Data["InitX"], (int)data.Data["InitY"], PlayerFieldCenter.position, PlayerFieldScale) :
                GetPositionForBlock((int)data.Data["TargetX"], (int)data.Data["TargetY"], EnemyFieldCenter.position, EnemyFieldScale);
            Vector2 to = (int)data.Data["TargetField"] == enemyField.InGameID ?
                GetPositionForBlock((int)data.Data["TargetX"], (int)data.Data["TargetY"], EnemyFieldCenter.position, EnemyFieldScale) :
                GetPositionForBlock((int)data.Data["InitX"], (int)data.Data["InitY"], PlayerFieldCenter.position, PlayerFieldScale);
            GameObject go = Instantiate(ShootEffectPrefab, Vector3.zero, Quaternion.identity, transform);
            StartCoroutine(go.GetComponent<IPointToPointEffect>().Initialize(from, to));
        }

        #endregion

        #region Field Input

        public void Handle(InputEvent input)
        {
            if (!CanControl)
                return;

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

            switch (input)
            {
                case BlockSwipeEvent swipe:
                    PreAnimateSwap(block.X, block.Y, swipe.Direction);
                    GameManager.Instance.OnPlayerBlockSwap(block.X, block.Y, swipe.Direction);
                    break;
                case BlockTapEvent tap:
                    GameManager.Instance.OnPlayerBlockTap(block.X, block.Y);
                    break;
            }
        }

        #endregion
    }
}
