using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace HappyHarvest
{
    /// <summary>
    /// ��ҿ�������������������롢�ƶ�����������Ʒ����
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // ���붯���ʲ�
        public InputActionAsset InputAction;
        // ����ƶ��ٶ�
        public float Speed = 4.0f;

        // Ŀ��ָʾ��
        public SpriteRenderer Target;
        // ��Ʒ���ص�
        public Transform ItemAttachBone;

        // ��ҽ�����������ԣ��޸�ʱ�Զ�����UI��
        public int Coins
        {
            get => m_Coins;
            set
            {
                m_Coins = value;
                UIHandler.UpdateCoins(Coins);
            }
        }

        // ��ұ���ϵͳ
        public InventorySystem Inventory => m_Inventory;
        // ��Ҷ���������
        public Animator Animator => m_Animator;

        // ��ҽ��������˽���ֶΣ�ͨ�����Է��ʣ�
        [SerializeField]
        private int m_Coins = 10;

        // ��ұ���ϵͳ
        [SerializeField]
        private InventorySystem m_Inventory;

        // 2D�������
        private Rigidbody2D m_Rigidbody;

        // �������λ��
        private Vector3 m_SpawnPosition;

        // �ƶ����붯��
        private InputAction m_MoveAction;
        // �л�����һ����Ʒ�����붯��
        private InputAction m_NextItemAction;
        // �л�����һ����Ʒ�����붯��
        private InputAction m_PrevItemAction;
        // ʹ����Ʒ�����붯��
        private InputAction m_UseItemAction;

        // ��ǰ��������������е�λ��
        private Vector3 m_CurrentWorldMousePos;
        // ��ǰ��ҵĳ���
        private Vector2 m_CurrentLookDirection;
        // ��ǰѡ�е�Ŀ�����
        private Vector3Int m_CurrentTarget;

        // Ŀ�������
        private TargetMarker m_TargetMarker;

        // �Ƿ�����ЧĿ��
        private bool m_HasTarget = false;
        // �Ƿ������UI��
        private bool m_IsOverUI = false;

        // �Ƿ���Կ������
        private bool m_CanControl = true;

        // ����������
        private Animator m_Animator;

        // ��ǰ����Ŀ��
        private InteractiveObject m_CurrentInteractiveTarget = null;
        // ��ײ�建������
        private Collider2D[] m_CollidersCache = new Collider2D[8];

        // ��Ʒ�Ӿ�ʵ���ֵ䣨��Ʒ -> ��Ʒʵ����
        private Dictionary<Item, ItemInstance> m_ItemVisualInstance = new();

        // ����������ϣֵ�����������Ż���
        private int m_DirXHash = Animator.StringToHash("DirX");
        private int m_DirYHash = Animator.StringToHash("DirY");
        private int m_SpeedHash = Animator.StringToHash("Speed");

        /// <summary>
        /// ��ʼ����ҿ�����
        /// </summary>
        void Awake()
        {
            // ȷ��������ֻ��һ�����ʵ��
            if (GameManager.Instance.Player != null)
            {
                Destroy(gameObject);
                return;
            }

            m_Rigidbody = GetComponent<Rigidbody2D>();
            m_Animator = GetComponentInChildren<Animator>();
            m_TargetMarker = Target.GetComponent<TargetMarker>();
            m_TargetMarker.Hide();

            // ����Ҷ�����Ϊ�����󲢱��Ϊ���泡�����ض�����
            gameObject.transform.SetParent(null);

            GameManager.Instance.Player = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// ����ʱ��ʼ����������״̬
        /// </summary>
        void Start()
        {
            // ��ʼ�����붯������ӻص�
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

            // Ϊ�����е�ÿ����Ʒ�����Ӿ�ʵ��
            foreach (var entry in m_Inventory.Entries)
            {
                if (entry.Item != null)
                    CreateItemVisual(entry.Item);
            }
            ToggleToolVisual(true);

            // ����UI��ʾ
            UIHandler.UpdateInventory(m_Inventory);
            UIHandler.UpdateCoins(m_Coins);
        }

        /// <summary>
        /// ÿ֡�����߼�
        /// </summary>
        private void Update()
        {
            m_IsOverUI = EventSystem.current.IsPointerOverGameObject();
            m_CurrentInteractiveTarget = null;
            m_HasTarget = false;

            // �������Ƿ�����Ϸ������
            if (!IsMouseOverGameWindow())
            {
                UIHandler.ChangeCursor(UIHandler.CursorType.System);
                return;
            }

            // ������ܿ��ƻ������UI�ϣ������к�������
            if (!m_CanControl || m_IsOverUI)
            {
                if (m_IsOverUI) UIHandler.ChangeCursor(UIHandler.CursorType.Interact);
                return;
            }

            // ��ȡ�������������ϵ�е�λ��
            m_CurrentWorldMousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            // ���������Ƿ��н�������
            var overlapCol = Physics2D.OverlapPoint(m_CurrentWorldMousePos, 1 << 31);

            if (overlapCol != null)
            {
                m_CurrentInteractiveTarget = overlapCol.GetComponent<InteractiveObject>();
                m_HasTarget = false;
                UIHandler.ChangeCursor(UIHandler.CursorType.Interact);
                return;
            }

            // ��겻��UI�ͽ��������ϣ�������ͨ���
            UIHandler.ChangeCursor(UIHandler.CursorType.Normal);

            var grid = GameManager.Instance.Terrain?.Grid;

            // �����������Ƿ���ڣ�ĳЩ��������û�е��Σ�
            if (grid != null)
            {
                var currentCell = grid.WorldToCell(transform.position);
                var pointedCell = grid.WorldToCell(m_CurrentWorldMousePos);

                currentCell.z = 0;
                pointedCell.z = 0;

                var toTarget = pointedCell - currentCell;

                // ����Ŀ�귶ΧΪ�����Χ1��
                if (Mathf.Abs(toTarget.x) > 1)
                {
                    toTarget.x = (int)Mathf.Sign(toTarget.x);
                }

                if (Mathf.Abs(toTarget.y) > 1)
                {
                    toTarget.y = (int)Mathf.Sign(toTarget.y);
                }

                // ���õ�ǰĿ����Ӳ�����Ŀ��ָʾ��
                m_CurrentTarget = currentCell + toTarget;
                Target.transform.position = GameManager.Instance.Terrain.Grid.GetCellCenterWorld(m_CurrentTarget);

                // ��鵱ǰװ������Ʒ�Ƿ������Ŀ��λ��ʹ��
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

            // ���ٱ���/���ؿ�ݼ�
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
        /// ʹ����Ʒ���뽻�����󽻻�
        /// </summary>
        void UseObject()
        {
            if (m_IsOverUI)
                return;

            // ����н���Ŀ�꣬�����뽻�����󽻻�
            if (m_CurrentInteractiveTarget != null)
            {
                m_CurrentInteractiveTarget.InteractedWith();
                return;
            }

            // ʹ�õ�ǰװ������Ʒ
            if (m_Inventory.EquippedItem != null && m_Inventory.EquippedItem.NeedTarget() && !m_HasTarget) return;

            UseItem();
        }

        /// <summary>
        /// ������£���������ƶ�
        /// </summary>
        void FixedUpdate()
        {
            var move = m_MoveAction.ReadValue<Vector2>();

            // ��������������ҳ���
            if (move != Vector2.zero)
            {
                SetLookDirectionFrom(move);
            }
            else
            {
                // ���û���ƶ����룬�������λ�����ó���
                if (IsMouseOverGameWindow())
                {
                    Vector3 posToMouse = m_CurrentWorldMousePos - transform.position;
                    SetLookDirectionFrom(posToMouse);
                }
            }

            // �����ƶ�������Ӧ�õ�����
            var movement = move * Speed;
            var speed = movement.sqrMagnitude;

            // ���¶�������
            m_Animator.SetFloat(m_DirXHash, m_CurrentLookDirection.x);
            m_Animator.SetFloat(m_DirYHash, m_CurrentLookDirection.y);
            m_Animator.SetFloat(m_SpeedHash, speed);

            m_Rigidbody.MovePosition(m_Rigidbody.position + movement * Time.deltaTime);
        }

        /// <summary>
        /// �������Ƿ�����Ϸ������
        /// </summary>
        bool IsMouseOverGameWindow()
        {
            return !(0 > Input.mousePosition.x || 0 > Input.mousePosition.y || Screen.width < Input.mousePosition.x || Screen.height < Input.mousePosition.y);
        }

        /// <summary>
        /// ���ݷ�������������ҳ���
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
        /// ��鱳���Ƿ�������ָ����Ʒ
        /// </summary>
        public bool CanFitInInventory(Item item, int count)
        {
            return m_Inventory.CanFitItem(item, count);
        }

        /// <summary>
        /// �����Ʒ������
        /// </summary>
        public bool AddItem(Item newItem)
        {
            CreateItemVisual(newItem);
            return m_Inventory.AddItem(newItem);
        }

        /// <summary>
        /// ������Ʒ
        /// </summary>
        public void SellItem(int inventoryIndex, int count)
        {
            if (inventoryIndex < 0 || inventoryIndex > Inventory.Entries.Length)
                return;

            var item = Inventory.Entries[inventoryIndex].Item;

            if (item == null || !(item is Product product))
                return;

            // �ӱ����Ƴ���Ʒ����ý��
            int actualCount = m_Inventory.Remove(inventoryIndex, count);

            m_Coins += actualCount * product.SellPrice;
            UIHandler.UpdateCoins(m_Coins);
            UIHandler.PlayBuySellSound(transform.position);
        }

        /// <summary>
        /// ������Ʒ
        /// </summary>
        public bool BuyItem(Item item)
        {
            if (item.BuyPrice > m_Coins)
            {
                return false;
            }

            // �۳���Ҳ������Ʒ������
            m_Coins -= item.BuyPrice;
            UIHandler.UpdateCoins(m_Coins);
            UIHandler.PlayBuySellSound(transform.position);
            AddItem(item);
            return true;
        }

        /// <summary>
        /// �л�װ������Ʒ
        /// </summary>
        public void ChangeEquipItem(int index)
        {
            // ���õ�ǰ��Ʒ���Ӿ�Ч��
            ToggleToolVisual(false);

            m_Inventory.EquipItem(index);

            // ��������Ʒ���Ӿ�Ч��
            ToggleToolVisual(true);
        }

        /// <summary>
        /// ����/������ҿ���
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
        /// ʹ�õ�ǰװ������Ʒ
        /// </summary>
        public void UseItem()
        {
            if (m_Inventory.EquippedItem == null)
                return;

            var previousEquipped = m_Inventory.EquippedItem;

            // ʹ����Ʒ
            m_Inventory.UseEquippedObject(m_CurrentTarget);

            // ����ʹ�ö���
            if (m_ItemVisualInstance.ContainsKey(previousEquipped))
            {
                var visual = m_ItemVisualInstance[previousEquipped];

                m_Animator.SetTrigger(visual.AnimatorHash);

                if (visual.Animator != null)
                {
                    if (!visual.Instance.activeInHierarchy)
                    {
                        // �����Ʒ�Ӿ�Ч�������ã�������
                        var current = visual.Instance.transform;
                        while (current != null)
                        {
                            current.gameObject.SetActive(true);
                            current = current.parent;
                        }
                    }

                    // ���ö�������������ʹ�ö���
                    visual.Animator.SetFloat(m_DirXHash, m_CurrentLookDirection.x);
                    visual.Animator.SetFloat(m_DirYHash, m_CurrentLookDirection.y);
                    visual.Animator.SetTrigger("Use");
                }
            }

            // ���ʹ�ú���Ʒ�������꣬�ӳٽ����Ӿ�Ч��
            if (m_Inventory.EquippedItem == null)
            {
                if (previousEquipped != null)
                {
                    StartCoroutine(DelayedObjectDisable(previousEquipped));
                }
            }
        }

        /// <summary>
        /// �ӳٽ�����Ʒ�Ӿ�Ч��
        /// </summary>
        IEnumerator DelayedObjectDisable(Item item)
        {
            yield return new WaitForSeconds(1.0f);
            ToggleVisualExplicit(false, item);
        }

        /// <summary>
        /// �����������
        /// </summary>
        public void Save(ref PlayerSaveData data)
        {
            data.Position = m_Rigidbody.position;
            data.Coins = m_Coins;
            data.Inventory = new List<InventorySaveData>();
            m_Inventory.Save(ref data.Inventory);
        }

        /// <summary>
        /// �����������
        /// </summary>
        public void Load(PlayerSaveData data)
        {
            m_Coins = data.Coins;
            m_Inventory.Load(data.Inventory);

            m_Rigidbody.position = data.Position;
        }

        /// <summary>
        /// ����/���õ�ǰװ����Ʒ���Ӿ�Ч��
        /// </summary>
        void ToggleToolVisual(bool enable)
        {
            if (m_Inventory.EquippedItem != null && m_ItemVisualInstance.TryGetValue(m_Inventory.EquippedItem, out var itemVisual))
            {
                itemVisual.Instance.SetActive(enable);
            }
        }

        /// <summary>
        /// ��ȷ����/����ָ����Ʒ���Ӿ�Ч��
        /// </summary>
        void ToggleVisualExplicit(bool enable, Item item)
        {
            if (item != null && m_ItemVisualInstance.TryGetValue(item, out var itemVisual))
            {
                itemVisual.Instance.SetActive(enable);
            }
        }

        /// <summary>
        /// ������Ʒ���Ӿ�ʵ��
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
    /// ��Ʒ�Ӿ�ʵ����
    /// </summary>
    class ItemInstance
    {
        // ��Ʒ�Ӿ���Ϸ����ʵ��
        public GameObject Instance;
        // ��Ʒ����������
        public Animator Animator;
        // ����������ϣֵ
        public int AnimatorHash;
    }

    /// <summary>
    /// ��ұ������ݽṹ
    /// </summary>
    [System.Serializable]
    public struct PlayerSaveData
    {
        // ���λ��
        public Vector3 Position;

        // ��ҽ������
        public int Coins;
        // ��ұ�������
        public List<InventorySaveData> Inventory;
    }
}