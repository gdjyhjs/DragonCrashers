using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HappyHarvest
{
    /// <summary>
    /// 玩家控制器，负责处理玩家输入、移动、交互和物品管理
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // 输入动作资产
        public InputActionAsset InputAction;
        // 玩家移动速度
        public float Speed = 4.0f;

        // 目标指示器
        public SpriteRenderer Target;
        // 物品挂载点
        public Transform ItemAttachBone;

        // 玩家金币数量（属性，修改时自动更新UI）
        public int Coins
        {
            get => m_Coins;
            set
            {
                m_Coins = value;
                UIHandler.UpdateCoins(Coins);
            }
        }

        // 玩家背包系统
        public InventorySystem Inventory => m_Inventory;
        // 玩家动画控制器
        public Animator Animator => m_Animator;

        // 玩家金币数量（私有字段，通过属性访问）
        [SerializeField]
        private int m_Coins = 10;

        // 玩家背包系统
        [SerializeField]
        private InventorySystem m_Inventory;

        // 2D刚体组件
        private Rigidbody2D m_Rigidbody;

        // 玩家生成位置
        private Vector3 m_SpawnPosition;

        // 移动输入动作
        private InputAction m_MoveAction;
        // 切换到下一个物品的输入动作
        private InputAction m_NextItemAction;
        // 切换到上一个物品的输入动作
        private InputAction m_PrevItemAction;
        // 使用物品的输入动作
        private InputAction m_UseItemAction;

        // 当前鼠标在世界坐标中的位置
        private Vector3 m_CurrentWorldMousePos;
        // 当前玩家的朝向
        private Vector2 m_CurrentLookDirection;
        // 当前选中的目标格子
        private Vector3Int m_CurrentTarget;

        // 目标标记组件
        private TargetMarker m_TargetMarker;

        // 是否有有效目标
        private bool m_HasTarget = false;
        // 是否鼠标在UI上
        private bool m_IsOverUI = false;

        // 是否可以控制玩家
        private bool m_CanControl = true;

        // 动画控制器
        private Animator m_Animator;

        // 当前交互目标
        private InteractiveObject m_CurrentInteractiveTarget = null;
        // 碰撞体缓存数组
        private Collider2D[] m_CollidersCache = new Collider2D[8];

        // 物品视觉实例字典（物品 -> 物品实例）
        private Dictionary<Item, ItemInstance> m_ItemVisualInstance = new();

        // 动画参数哈希值（用于性能优化）
        private int m_DirXHash = Animator.StringToHash("DirX");
        private int m_DirYHash = Animator.StringToHash("DirY");
        private int m_SpeedHash = Animator.StringToHash("Speed");

        /// <summary>
        /// 初始化玩家控制器
        /// </summary>
        void Awake()
        {
            // 确保场景中只有一个玩家实例
            if (GameManager.Instance.Player != null)
            {
                Destroy(gameObject);
                return;
            }

            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_Animator = GetComponentInChildren<Animator>();
            m_TargetMarker = Target.GetComponent<TargetMarker>();
            m_TargetMarker.Hide();

            // 将玩家对象设为根对象并标记为不随场景加载而销毁
            gameObject.transform.SetParent(null);

            GameManager.Instance.Player = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 启动时初始化输入和玩家状态
        /// </summary>
        void Start()
        {
            // 初始化输入动作并添加回调
            m_MoveAction = InputAction.FindAction("Gameplay/Move");
            m_MoveAction.Enable();

            m_NextItemAction = InputAction.FindAction("Gameplay/EquipNext");
            m_PrevItemAction = InputAction.FindAction("Gameplay/EquipPrev");

            m_NextItemAction.Enable();
            m_NextItemAction.performed += context =>
            {
                ToggleToolVisual(false);
                m_Inventory.EquipNext();
                ToggleToolVisual(true);
            };

            m_PrevItemAction.Enable();
            m_PrevItemAction.performed += context =>
            {
                ToggleToolVisual(false);
                m_Inventory.EquipPrev();
                ToggleToolVisual(true);
            };

            m_UseItemAction = InputAction.FindAction("Gameplay/Use");
            m_UseItemAction.Enable();

            m_UseItemAction.performed += context => UseObject();

            m_CurrentLookDirection = Vector2.right;

            m_Inventory.Init();

            // 为背包中的每个物品创建视觉实例
            foreach (var entry in m_Inventory.Entries)
            {
                if (entry.Item != null)
                    CreateItemVisual(entry.Item);
            }
            ToggleToolVisual(true);

            // 更新UI显示
            UIHandler.UpdateInventory(m_Inventory);
            UIHandler.UpdateCoins(m_Coins);
        }

        /// <summary>
        /// 每帧更新逻辑
        /// </summary>
        private void Update()
        {
            m_IsOverUI = EventSystem.current.IsPointerOverGameObject();
            m_CurrentInteractiveTarget = null;
            m_HasTarget = false;

            // 检查鼠标是否在游戏窗口内
            if (!IsMouseOverGameWindow())
            {
                UIHandler.ChangeCursor(UIHandler.CursorType.System);
                return;
            }

            // 如果不能控制或鼠标在UI上，不进行后续操作
            if (!m_CanControl || m_IsOverUI)
            {
                if (m_IsOverUI) UIHandler.ChangeCursor(UIHandler.CursorType.Interact);
                return;
            }

            // 获取鼠标在世界坐标系中的位置
            m_CurrentWorldMousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            // 检查鼠标下是否有交互对象
            var overlapCol = Physics2D.OverlapPoint(m_CurrentWorldMousePos, 1 << 31);

            if (overlapCol != null)
            {
                m_CurrentInteractiveTarget = overlapCol.GetComponent<InteractiveObject>();
                m_HasTarget = false;
                UIHandler.ChangeCursor(UIHandler.CursorType.Interact);
                return;
            }

            // 鼠标不在UI和交互对象上，设置普通光标
            UIHandler.ChangeCursor(UIHandler.CursorType.Normal);

            var grid = GameManager.Instance.Terrain?.Grid;

            // 检查地形网格是否存在（某些场景可能没有地形）
            if (grid != null)
            {
                var currentCell = grid.WorldToCell(transform.position);
                var pointedCell = grid.WorldToCell(m_CurrentWorldMousePos);

                currentCell.z = 0;
                pointedCell.z = 0;

                var toTarget = pointedCell - currentCell;

                // 限制目标范围为玩家周围1格
                if (Mathf.Abs(toTarget.x) > 1)
                {
                    toTarget.x = (int)Mathf.Sign(toTarget.x);
                }

                if (Mathf.Abs(toTarget.y) > 1)
                {
                    toTarget.y = (int)Mathf.Sign(toTarget.y);
                }

                // 设置当前目标格子并更新目标指示器
                m_CurrentTarget = currentCell + toTarget;
                Target.transform.position = GameManager.Instance.Terrain.Grid.GetCellCenterWorld(m_CurrentTarget);

                // 检查当前装备的物品是否可以在目标位置使用
                if (m_Inventory.EquippedItem != null
                    && m_Inventory.EquippedItem.CanUse(m_CurrentTarget))
                {
                    m_HasTarget = true;
                    m_TargetMarker.Activate();
                }
                else
                {
                    m_TargetMarker.Hide();
                }
            }

            // 快速保存/加载快捷键
            if (Keyboard.current.f5Key.wasPressedThisFrame)
            {
                SaveSystem.Save();
            }
            else if (Keyboard.current.f9Key.wasPressedThisFrame)
            {
                SaveSystem.Load();
            }
        }

        /// <summary>
        /// 使用物品或与交互对象交互
        /// </summary>
        void UseObject()
        {
            if (m_IsOverUI)
                return;

            // 如果有交互目标，优先与交互对象交互
            if (m_CurrentInteractiveTarget != null)
            {
                m_CurrentInteractiveTarget.InteractedWith();
                return;
            }

            // 使用当前装备的物品
            if (m_Inventory.EquippedItem != null && m_Inventory.EquippedItem.NeedTarget() && !m_HasTarget) return;

            UseItem();
        }

        /// <summary>
        /// 物理更新，处理玩家移动
        /// </summary>
        void FixedUpdate()
        {
            var move = m_MoveAction.ReadValue<Vector2>();

            // 根据输入设置玩家朝向
            if (move != Vector2.zero)
            {
                SetLookDirectionFrom(move);
            }
            else
            {
                // 如果没有移动输入，根据鼠标位置设置朝向
                if (IsMouseOverGameWindow())
                {
                    Vector3 posToMouse = m_CurrentWorldMousePos - transform.position;
                    SetLookDirectionFrom(posToMouse);
                }
            }

            // 计算移动向量并应用到刚体
            var movement = move * Speed;
            var speed = movement.sqrMagnitude;

            // 更新动画参数
            m_Animator.SetFloat(m_DirXHash, m_CurrentLookDirection.x);
            m_Animator.SetFloat(m_DirYHash, m_CurrentLookDirection.y);
            m_Animator.SetFloat(m_SpeedHash, speed);

            m_Rigidbody.MovePosition(m_Rigidbody.position + movement * Time.deltaTime);
        }

        /// <summary>
        /// 检查鼠标是否在游戏窗口内
        /// </summary>
        bool IsMouseOverGameWindow()
        {
            return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);
        }

        /// <summary>
        /// 根据方向向量设置玩家朝向
        /// </summary>
        void SetLookDirectionFrom(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                m_CurrentLookDirection = direction.x > 0 ? Vector2.right : Vector2.left;
            }
            else
            {
                m_CurrentLookDirection = direction.y > 0 ? Vector2.up : Vector2.down;
            }
        }

        /// <summary>
        /// 检查背包是否能容纳指定物品
        /// </summary>
        public bool CanFitInInventory(Item item, int count)
        {
            return m_Inventory.CanFitItem(item, count);
        }

        /// <summary>
        /// 添加物品到背包
        /// </summary>
        public bool AddItem(Item newItem)
        {
            CreateItemVisual(newItem);
            return m_Inventory.AddItem(newItem);
        }

        /// <summary>
        /// 出售物品
        /// </summary>
        public void SellItem(int inventoryIndex, int count)
        {
            if (inventoryIndex < 0 || inventoryIndex > Inventory.Entries.Length)
                return;

            var item = Inventory.Entries[inventoryIndex].Item;

            if (item == null || !(item is Product product))
                return;

            // 从背包移除物品并获得金币
            int actualCount = m_Inventory.Remove(inventoryIndex, count);

            m_Coins += actualCount * product.SellPrice;
            UIHandler.UpdateCoins(m_Coins);
            UIHandler.PlayBuySellSound(transform.position);
        }

        /// <summary>
        /// 购买物品
        /// </summary>
        public bool BuyItem(Item item)
        {
            if (item.BuyPrice > m_Coins)
            {
                return false;
            }

            // 扣除金币并添加物品到背包
            m_Coins -= item.BuyPrice;
            UIHandler.UpdateCoins(m_Coins);
            UIHandler.PlayBuySellSound(transform.position);
            AddItem(item);
            return true;
        }

        /// <summary>
        /// 切换装备的物品
        /// </summary>
        public void ChangeEquipItem(int index)
        {
            // 禁用当前物品的视觉效果
            ToggleToolVisual(false);

            m_Inventory.EquipItem(index);

            // 启用新物品的视觉效果
            ToggleToolVisual(true);
        }

        /// <summary>
        /// 启用/禁用玩家控制
        /// </summary>
        public void ToggleControl(bool canControl)
        {
            m_CanControl = canControl;
            if (canControl)
            {
                m_MoveAction.Enable();
                m_NextItemAction.Enable();
                m_PrevItemAction.Enable();
                m_UseItemAction.Enable();
            }
            else
            {
                m_MoveAction.Disable();
                m_NextItemAction.Disable();
                m_PrevItemAction.Disable();
                m_UseItemAction.Disable();
            }
        }

        /// <summary>
        /// 使用当前装备的物品
        /// </summary>
        public void UseItem()
        {
            if (m_Inventory.EquippedItem == null)
                return;

            var previousEquipped = m_Inventory.EquippedItem;

            // 使用物品
            m_Inventory.UseEquippedObject(m_CurrentTarget);

            // 播放使用动画
            if (m_ItemVisualInstance.ContainsKey(previousEquipped))
            {
                var visual = m_ItemVisualInstance[previousEquipped];

                m_Animator.SetTrigger(visual.AnimatorHash);

                if (visual.Animator != null)
                {
                    if (!visual.Instance.activeInHierarchy)
                    {
                        // 如果物品视觉效果被禁用，启用它
                        var current = visual.Instance.transform;
                        while (current != null)
                        {
                            current.gameObject.SetActive(true);
                            current = current.parent;
                        }
                    }

                    // 设置动画参数并触发使用动画
                    visual.Animator.SetFloat(m_DirXHash, m_CurrentLookDirection.x);
                    visual.Animator.SetFloat(m_DirYHash, m_CurrentLookDirection.y);
                    visual.Animator.SetTrigger("Use");
                }
            }

            // 如果使用后物品被消耗完，延迟禁用视觉效果
            if (m_Inventory.EquippedItem == null)
            {
                if (previousEquipped != null)
                {
                    StartCoroutine(DelayedObjectDisable(previousEquipped));
                }
            }
        }

        /// <summary>
        /// 延迟禁用物品视觉效果
        /// </summary>
        IEnumerator DelayedObjectDisable(Item item)
        {
            yield return new WaitForSeconds(1.0f);
            ToggleVisualExplicit(false, item);
        }

        /// <summary>
        /// 保存玩家数据
        /// </summary>
        public void Save(ref PlayerSaveData data)
        {
            data.Position = m_Rigidbody.position;
            data.Coins = m_Coins;
            data.Inventory = new List<InventorySaveData>();
            m_Inventory.Save(ref data.Inventory);
        }

        /// <summary>
        /// 加载玩家数据
        /// </summary>
        public void Load(PlayerSaveData data)
        {
            m_Coins = data.Coins;
            m_Inventory.Load(data.Inventory);

            m_Rigidbody.position = data.Position;
        }

        /// <summary>
        /// 启用/禁用当前装备物品的视觉效果
        /// </summary>
        void ToggleToolVisual(bool enable)
        {
            if (m_Inventory.EquippedItem != null && m_ItemVisualInstance.TryGetValue(m_Inventory.EquippedItem, out var itemVisual))
            {
                itemVisual.Instance.SetActive(enable);
            }
        }

        /// <summary>
        /// 明确启用/禁用指定物品的视觉效果
        /// </summary>
        void ToggleVisualExplicit(bool enable, Item item)
        {
            if (item != null && m_ItemVisualInstance.TryGetValue(item, out var itemVisual))
            {
                itemVisual.Instance.SetActive(enable);
            }
        }

        /// <summary>
        /// 创建物品的视觉实例
        /// </summary>
        void CreateItemVisual(Item item)
        {
            if (item.VisualPrefab != null && !m_ItemVisualInstance.ContainsKey(item))
            {
                var newVisual = Instantiate(item.VisualPrefab, ItemAttachBone, false);
                newVisual.SetActive(false);

                m_ItemVisualInstance[item] = new ItemInstance()
                {
                    Instance = newVisual,
                    Animator = newVisual.GetComponentInChildren<Animator>(),
                    AnimatorHash = Animator.StringToHash(item.PlayerAnimatorTriggerUse)
                };
            }
        }
    }

    /// <summary>
    /// 物品视觉实例类
    /// </summary>
    class ItemInstance
    {
        // 物品视觉游戏对象实例
        public GameObject Instance;
        // 物品动画控制器
        public Animator Animator;
        // 动画触发哈希值
        public int AnimatorHash;
    }

    /// <summary>
    /// 玩家保存数据结构
    /// </summary>
    [System.Serializable]
    public struct PlayerSaveData
    {
        // 玩家位置
        public Vector3 Position;

        // 玩家金币数量
        public int Coins;
        // 玩家背包数据
        public List<InventorySaveData> Inventory;
    }
}