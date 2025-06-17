using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// �����������Բ����Ļ�豸�İ�ȫ����߿�
    /// �������������Ҫ��ѭ��ȫ�����UI�Ϸ�ʹ��һ������Ԫ�أ�
    /// Ȼ�����borderWidth������ƥ��Screen.safeArea����ֵ��
    /// </summary>
    [ExecuteInEditMode]
    public class SafeAreaBorder : MonoBehaviour
    {
        [Tooltip("����UXML�㼶�ṹ��UI�ĵ�")]
        [SerializeField] UIDocument m_Document;

        [Tooltip("�߿��������ɫ��ʹ��͸��ɫ����ʾ����")]
        [SerializeField] Color m_BorderColor = Color.black;

        [Tooltip("����Ԫ�����������ơ�������ʹ��rootVisualElement")]
        [SerializeField] string m_Element;

        [Tooltip("��ȫ�������İٷֱȳ���")]
        [Range(0, 1f)]
        [SerializeField] float m_Multiplier = 1f;

        [Tooltip("�ڿ���̨��ʾ������Ϣ")]
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
                Debug.LogWarning("UIDocument��rootVisualElementΪ�ա��ӳٳ�ʼ����");
                return;
            }

            // ���δָ������ѡ���VisualElement
            if (string.IsNullOrEmpty(m_Element))
            {
                m_Root = m_Document.rootVisualElement;
            }
            // �����԰����Ʋ�������
            else
            {
                m_Root = m_Document.rootVisualElement.Q<VisualElement>(m_Element);
            }

            if (m_Root == null)
            {
                if (m_Debug)
                {
                    Debug.LogWarning("[SafeAreaBorder]: m_RootΪ�ա�δ�ҵ�Ԫ�ػ�UIDocumentδ��ʼ����");
                }
                return;
            }

            // ע��UI���α仯ʱ�Ļص�
            m_Root.RegisterCallback<GeometryChangedEvent>(evt => OnGeometryChangedEvent());

            ApplySafeArea();
        }

        void OnGeometryChangedEvent()
        {
            ApplySafeArea();
        }

        void OnValidate()
        {
            // ��m_Multiplier����ʱ����ApplySafeArea
            ApplySafeArea();
        }

        // ����ȫ����Ӧ�õ��߿�
        void ApplySafeArea()
        {
            if (m_Root == null)
                return;

            Rect safeArea = Screen.safeArea;

            // ���ݰ�ȫ������μ���߿�
            m_LeftBorder = safeArea.x;
            m_RightBorder = Screen.width - safeArea.xMax;
            m_TopBorder = Screen.height - safeArea.yMax;
            m_BottomBorder = safeArea.y;

            // ���۷�����ζ����ñ߿���
            m_Root.style.borderTopWidth = m_TopBorder * m_Multiplier;
            m_Root.style.borderBottomWidth = m_BottomBorder * m_Multiplier;
            m_Root.style.borderLeftWidth = m_LeftBorder * m_Multiplier;
            m_Root.style.borderRightWidth = m_RightBorder * m_Multiplier;

            // Ӧ�ñ߿���ɫ
            m_Root.style.borderBottomColor = m_BorderColor;
            m_Root.style.borderTopColor = m_BorderColor;
            m_Root.style.borderLeftColor = m_BorderColor;
            m_Root.style.borderRightColor = m_BorderColor;

            if (m_Debug)
            {
                Debug.Log($"[SafeAreaBorder] ��Ӧ�ð�ȫ���� | ��Ļ����: {Screen.orientation} | ��: {m_LeftBorder}, ��: {m_RightBorder}, ��: {m_TopBorder}, ��: {m_BottomBorder}");
            }
        }
    }
}