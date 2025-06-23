using System;
namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph 输出事件预制体生成器，用于在 VFX 事件触发时生成预制体实例
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VisualEffect))]
    class VFXOutputEventPrefabSpawn : VFXOutputEventAbstractHandler
    {
        // 允许在编辑器模式下执行
        public override bool canExecuteInEditor => true;
        // 可同时激活的预制体实例最大数量
        public uint instanceCount => m_InstanceCount;
        // 要生成的预制体
        public GameObject prefabToSpawn => m_PrefabToSpawn;
        // 生成的实例是否作为当前对象的子对象
        public bool parentInstances => m_ParentInstances;

#pragma warning disable 414, 649
        [SerializeField, Tooltip("同一时间可激活的预制体最大数量")]
        uint m_InstanceCount = 5;
        [SerializeField, Tooltip("接收到事件时激活的预制体。预制体在启用此行为时创建为隐藏状态并存储在池中。接收到事件时，从池中启用一个预制体，并在达到其生命周期时禁用")]
        GameObject m_PrefabToSpawn;
        [SerializeField, Tooltip("是否将预制体实例附加到当前游戏对象。使用此设置可将位置和角度属性视为局部空间")]
        bool m_ParentInstances;
#pragma warning restore 414, 649

#if UNITY_EDITOR
        // 标记数据是否需要重建
        bool m_Dirty = true;
#endif

        [Tooltip("是否使用 position 属性设置生成预制体的位置")]
        public bool usePosition = true;
        [Tooltip("是否使用 angle 属性设置生成预制体的旋转")]
        public bool useAngle = true;
        [Tooltip("是否使用 scale 属性设置生成预制体的本地缩放")]
        public bool useScale = true;
        [Tooltip("是否使用 lifetime 属性确定预制体将被启用的时间")]
        public bool useLifetime = true;

        // 空游戏对象数组（用于初始化）
        static readonly GameObject[] k_EmptyGameObjects = new GameObject[0];
        // 空生存时间数组（用于初始化）
        static readonly float[] k_EmptyTimeToLive = new float[0];
        // 预制体实例数组（对象池）
        GameObject[] m_Instances = k_EmptyGameObjects;
        // 实例生存时间数组
        float[] m_TimesToLive = k_EmptyTimeToLive;

        /// <summary>
        /// 组件禁用时调用，禁用所有实例
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            foreach (var instance in m_Instances)
                instance.SetActive(false);
        }

        /// <summary>
        /// 对象销毁时调用，释放所有实例
        /// </summary>
        void OnDestroy()
        {
            DisposeInstances();
        }

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器中属性值验证时调用，标记数据为需要重建
        /// </summary>
        void OnValidate()
        {
            m_Dirty = true;
        }

#endif

        /// <summary>
        /// 释放所有实例资源
        /// </summary>
        void DisposeInstances()
        {
            foreach (var instance in m_Instances)
            {
                if (instance)
                {
                    if (Application.isPlaying)
                        Destroy(instance);
                    else
                        DestroyImmediate(instance);
                }
            }
            m_Instances = k_EmptyGameObjects;
            m_TimesToLive = k_EmptyTimeToLive;
        }

        // VFX 属性 ID 定义
        static readonly int k_PositionID = Shader.PropertyToID("position");
        static readonly int k_AngleID = Shader.PropertyToID("angle");
        static readonly int k_ScaleID = Shader.PropertyToID("scale");
        static readonly int k_LifetimeID = Shader.PropertyToID("lifetime");

        /// <summary>
        /// 更新实例的隐藏标志
        /// </summary>
        void UpdateHideFlag(GameObject instance)
        {
            instance.hideFlags = HideFlags.HideAndDontSave;
        }

        /// <summary>
        /// 检查并重建实例池
        /// </summary>
        void CheckAndRebuildInstances()
        {
            // 当实例数量变化时需要重建
            bool rebuild = m_Instances.Length != m_InstanceCount;
#if UNITY_EDITOR
            if (m_Dirty)
            {
                rebuild = true;
                m_Dirty = false;
            }
#endif
            if (rebuild)
            {
                DisposeInstances();
                if (m_PrefabToSpawn != null && m_InstanceCount != 0)
                {
                    m_Instances = new GameObject[m_InstanceCount];
                    m_TimesToLive = new float[m_InstanceCount];
#if UNITY_EDITOR
                    var prefabAssetType = UnityEditor.PrefabUtility.GetPrefabAssetType(m_PrefabToSpawn);
#endif
                    for (int i = 0; i < m_Instances.Length; i++)
                    {
                        GameObject newInstance = null;
#if UNITY_EDITOR
                        if (prefabAssetType != UnityEditor.PrefabAssetType.NotAPrefab)
                            newInstance = UnityEditor.PrefabUtility.InstantiatePrefab(m_PrefabToSpawn) as GameObject;

                        if (newInstance == null)
                            newInstance = Instantiate(m_PrefabToSpawn);
#else
newInstance = Instantiate(m_PrefabToSpawn);
#endif
                        newInstance.name = $"{name} - #{i} - {m_PrefabToSpawn.name}";
                        newInstance.SetActive(false);
                        newInstance.transform.parent = m_ParentInstances ? transform : null;
                        UpdateHideFlag(newInstance);

                        m_Instances[i] = newInstance;
                        m_TimesToLive[i] = float.NegativeInfinity;
                    }
                }
            }
        }

        /// <summary>
        /// 当 Visual Effect Graph 触发输出事件时调用，生成预制体实例
        /// </summary>
        public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
        {
            CheckAndRebuildInstances();

            // 查找可用的实例索引
            int freeIdx = -1;
            for (int i = 0; i < m_Instances.Length; i++)
            {
                if (!m_Instances[i].activeSelf)
                {
                    freeIdx = i;
                    break;
                }
            }

            if (freeIdx != -1)
            {
                var availableInstance = m_Instances[freeIdx];
                availableInstance.SetActive(true);

                // 根据 VFX 属性设置实例位置
                if (usePosition && eventAttribute.HasVector3(k_PositionID))
                {
                    if (m_ParentInstances)
                        availableInstance.transform.localPosition = eventAttribute.GetVector3(k_PositionID);
                    else
                        availableInstance.transform.position = eventAttribute.GetVector3(k_PositionID);
                }

                // 根据 VFX 属性设置实例旋转
                if (useAngle && eventAttribute.HasVector3(k_AngleID))
                {
                    if (m_ParentInstances)
                        availableInstance.transform.localEulerAngles = eventAttribute.GetVector3(k_AngleID);
                    else
                        availableInstance.transform.eulerAngles = eventAttribute.GetVector3(k_AngleID);
                }

                // 根据 VFX 属性设置实例缩放
                if (useScale && eventAttribute.HasVector3(k_ScaleID))
                    availableInstance.transform.localScale = eventAttribute.GetVector3(k_ScaleID);

                // 设置实例生存时间
                if (useLifetime && eventAttribute.HasFloat(k_LifetimeID))
                    m_TimesToLive[freeIdx] = eventAttribute.GetFloat(k_LifetimeID);
                else
                    m_TimesToLive[freeIdx] = float.NegativeInfinity;

                // 触发实例上的 VFX 事件处理器
                var handlers = availableInstance.GetComponentsInChildren<VFXOutputEventPrefabAttributeAbstractHandler>();
                foreach (var handler in handlers)
                    handler.OnVFXEventAttribute(eventAttribute, m_VisualEffect);
            }
        }

        /// <summary>
        /// 每帧更新，管理实例的生命周期
        /// </summary>
        void Update()
        {
            if (Application.isPlaying || (executeInEditor && canExecuteInEditor))
            {
                CheckAndRebuildInstances();

                var dt = Time.deltaTime;
                for (int i = 0; i < m_Instances.Length; i++)
                {
#if UNITY_EDITOR
                    // 重新分配隐藏标志，预制体打开可能会重置此标志
                    UpdateHideFlag(m_Instances[i]);
#endif
                    // 负无穷表示不管理时间
                    if (m_TimesToLive[i] == float.NegativeInfinity)
                        continue;

                    // 管理实例生存时间
                    if (m_TimesToLive[i] <= 0.0f && m_Instances[i].activeSelf)
                        m_Instances[i].SetActive(false);
                    else
                        m_TimesToLive[i] -= dt;
                }
            }
            else
            {
                DisposeInstances();
            }
        }
    }
}