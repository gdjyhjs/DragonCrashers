using System;
namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph ����¼�Ԥ������������������ VFX �¼�����ʱ����Ԥ����ʵ��
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VisualEffect))]
    class VFXOutputEventPrefabSpawn : VFXOutputEventAbstractHandler
    {
        // �����ڱ༭��ģʽ��ִ��
        public override bool canExecuteInEditor => true;
        // ��ͬʱ�����Ԥ����ʵ���������
        public uint instanceCount => m_InstanceCount;
        // Ҫ���ɵ�Ԥ����
        public GameObject prefabToSpawn => m_PrefabToSpawn;
        // ���ɵ�ʵ���Ƿ���Ϊ��ǰ������Ӷ���
        public bool parentInstances => m_ParentInstances;

#pragma warning disable 414, 649
        [SerializeField, Tooltip("ͬһʱ��ɼ����Ԥ�����������")]
        uint m_InstanceCount = 5;
        [SerializeField, Tooltip("���յ��¼�ʱ�����Ԥ���塣Ԥ���������ô���Ϊʱ����Ϊ����״̬���洢�ڳ��С����յ��¼�ʱ���ӳ�������һ��Ԥ���壬���ڴﵽ����������ʱ����")]
        GameObject m_PrefabToSpawn;
        [SerializeField, Tooltip("�Ƿ�Ԥ����ʵ�����ӵ���ǰ��Ϸ����ʹ�ô����ÿɽ�λ�úͽǶ�������Ϊ�ֲ��ռ�")]
        bool m_ParentInstances;
#pragma warning restore 414, 649

#if UNITY_EDITOR
        // ��������Ƿ���Ҫ�ؽ�
        bool m_Dirty = true;
#endif

        [Tooltip("�Ƿ�ʹ�� position ������������Ԥ�����λ��")]
        public bool usePosition = true;
        [Tooltip("�Ƿ�ʹ�� angle ������������Ԥ�������ת")]
        public bool useAngle = true;
        [Tooltip("�Ƿ�ʹ�� scale ������������Ԥ����ı�������")]
        public bool useScale = true;
        [Tooltip("�Ƿ�ʹ�� lifetime ����ȷ��Ԥ���彫�����õ�ʱ��")]
        public bool useLifetime = true;

        // ����Ϸ�������飨���ڳ�ʼ����
        static readonly GameObject[] k_EmptyGameObjects = new GameObject[0];
        // ������ʱ�����飨���ڳ�ʼ����
        static readonly float[] k_EmptyTimeToLive = new float[0];
        // Ԥ����ʵ�����飨����أ�
        GameObject[] m_Instances = k_EmptyGameObjects;
        // ʵ������ʱ������
        float[] m_TimesToLive = k_EmptyTimeToLive;

        /// <summary>
        /// �������ʱ���ã���������ʵ��
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
            foreach (var instance in m_Instances)
                instance.SetActive(false);
        }

        /// <summary>
        /// ��������ʱ���ã��ͷ�����ʵ��
        /// </summary>
        void OnDestroy()
        {
            DisposeInstances();
        }

#if UNITY_EDITOR
        /// <summary>
        /// �༭��������ֵ��֤ʱ���ã��������Ϊ��Ҫ�ؽ�
        /// </summary>
        void OnValidate()
        {
            m_Dirty = true;
        }

#endif

        /// <summary>
        /// �ͷ�����ʵ����Դ
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

        // VFX ���� ID ����
        static readonly int k_PositionID = Shader.PropertyToID("position");
        static readonly int k_AngleID = Shader.PropertyToID("angle");
        static readonly int k_ScaleID = Shader.PropertyToID("scale");
        static readonly int k_LifetimeID = Shader.PropertyToID("lifetime");

        /// <summary>
        /// ����ʵ�������ر�־
        /// </summary>
        void UpdateHideFlag(GameObject instance)
        {
            instance.hideFlags = HideFlags.HideAndDontSave;
        }

        /// <summary>
        /// ��鲢�ؽ�ʵ����
        /// </summary>
        void CheckAndRebuildInstances()
        {
            // ��ʵ�������仯ʱ��Ҫ�ؽ�
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
        /// �� Visual Effect Graph ��������¼�ʱ���ã�����Ԥ����ʵ��
        /// </summary>
        public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
        {
            CheckAndRebuildInstances();

            // ���ҿ��õ�ʵ������
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

                // ���� VFX ��������ʵ��λ��
                if (usePosition && eventAttribute.HasVector3(k_PositionID))
                {
                    if (m_ParentInstances)
                        availableInstance.transform.localPosition = eventAttribute.GetVector3(k_PositionID);
                    else
                        availableInstance.transform.position = eventAttribute.GetVector3(k_PositionID);
                }

                // ���� VFX ��������ʵ����ת
                if (useAngle && eventAttribute.HasVector3(k_AngleID))
                {
                    if (m_ParentInstances)
                        availableInstance.transform.localEulerAngles = eventAttribute.GetVector3(k_AngleID);
                    else
                        availableInstance.transform.eulerAngles = eventAttribute.GetVector3(k_AngleID);
                }

                // ���� VFX ��������ʵ������
                if (useScale && eventAttribute.HasVector3(k_ScaleID))
                    availableInstance.transform.localScale = eventAttribute.GetVector3(k_ScaleID);

                // ����ʵ������ʱ��
                if (useLifetime && eventAttribute.HasFloat(k_LifetimeID))
                    m_TimesToLive[freeIdx] = eventAttribute.GetFloat(k_LifetimeID);
                else
                    m_TimesToLive[freeIdx] = float.NegativeInfinity;

                // ����ʵ���ϵ� VFX �¼�������
                var handlers = availableInstance.GetComponentsInChildren<VFXOutputEventPrefabAttributeAbstractHandler>();
                foreach (var handler in handlers)
                    handler.OnVFXEventAttribute(eventAttribute, m_VisualEffect);
            }
        }

        /// <summary>
        /// ÿ֡���£�����ʵ������������
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
                    // ���·������ر�־��Ԥ����򿪿��ܻ����ô˱�־
                    UpdateHideFlag(m_Instances[i]);
#endif
                    // �������ʾ������ʱ��
                    if (m_TimesToLive[i] == float.NegativeInfinity)
                        continue;

                    // ����ʵ������ʱ��
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