using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 管理带刘海或圆角屏幕设备的安全区域边框。
    /// 该组件在所有需要遵循安全区域的UI上方使用一个容器元素，
    /// 然后调整borderWidth属性以匹配Screen.safeArea属性值。
    /// </summary>
    [ExecuteInEditMode]
    public class SafeAreaBorder : MonoBehaviour
    {
        [Tooltip("包含UXML层级结构的UI文档")]
        [SerializeField] UIDocument m_Document;

        [Tooltip("边框区域的颜色。使用透明色可显示背景")]
        [SerializeField] Color m_BorderColor = Color.black;

        [Tooltip("顶级元素容器的名称。留空则使用rootVisualElement")]
        [SerializeField] string m_Element;

        [Tooltip("安全区域距离的百分比乘数")]
        [Range(0, 1f)]
        [SerializeField] float m_Multiplier = 1f;

        [Tooltip("在控制台显示调试信息")]
        [SerializeField] bool m_Debug;

        VisualElement m_Root;
        float m_LeftBorder;
        float m_RightBorder;
        float m_TopBorder;
        float m_BottomBorder;

        public VisualElement RootElement => m_Root;
        public float LeftBorder => m_LeftBorder;
        public float RightBorder => m_RightBorder;
        public float TopBorder => m_TopBorder;
        public float BottomBorder => m_BottomBorder;

        public float Multiplier { get => m_Multiplier; set => m_Multiplier = value; }

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (m_Document == null || m_Document.rootVisualElement == null)
            {
                Debug.LogWarning("UIDocument或rootVisualElement为空。延迟初始化。");
                return;
            }

            // 如果未指定，则选择根VisualElement
            if (string.IsNullOrEmpty(m_Element))
            {
                m_Root = m_Document.rootVisualElement;
            }
            // 否则尝试按名称查找容器
            else
            {
                m_Root = m_Document.rootVisualElement.Q<VisualElement>(m_Element);
            }

            if (m_Root == null)
            {
                if (m_Debug)
                {
                    Debug.LogWarning("[SafeAreaBorder]: m_Root为空。未找到元素或UIDocument未初始化。");
                }
                return;
            }

            // 注册UI几何变化时的回调
            m_Root.RegisterCallback<GeometryChangedEvent>(evt => OnGeometryChangedEvent());

            ApplySafeArea();
        }

        void OnGeometryChangedEvent()
        {
            ApplySafeArea();
        }

        void OnValidate()
        {
            // 当m_Multiplier更改时调用ApplySafeArea
            ApplySafeArea();
        }

        // 将安全区域应用到边框
        void ApplySafeArea()
        {
            if (m_Root == null)
                return;

            Rect safeArea = Screen.safeArea;

            // 根据安全区域矩形计算边框
            m_LeftBorder = safeArea.x;
            m_RightBorder = Screen.width - safeArea.xMax;
            m_TopBorder = Screen.height - safeArea.yMax;
            m_BottomBorder = safeArea.y;

            // 无论方向如何都设置边框宽度
            m_Root.style.borderTopWidth = m_TopBorder * m_Multiplier;
            m_Root.style.borderBottomWidth = m_BottomBorder * m_Multiplier;
            m_Root.style.borderLeftWidth = m_LeftBorder * m_Multiplier;
            m_Root.style.borderRightWidth = m_RightBorder * m_Multiplier;

            // 应用边框颜色
            m_Root.style.borderBottomColor = m_BorderColor;
            m_Root.style.borderTopColor = m_BorderColor;
            m_Root.style.borderLeftColor = m_BorderColor;
            m_Root.style.borderRightColor = m_BorderColor;

            if (m_Debug)
            {
                Debug.Log($"[SafeAreaBorder] 已应用安全区域 | 屏幕方向: {Screen.orientation} | 左: {m_LeftBorder}, 右: {m_RightBorder}, 上: {m_TopBorder}, 下: {m_BottomBorder}");
            }
        }
    }
}