using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// һ���Զ���UIԪ�أ�����һ���������ָʾ��/Բ�ν�������ʹ��ʸ��API�������ڲ��ı�ǩ�԰ٷֱ���ʽ��ʾ��ǰ���ȡ�
    /// 
    /// �ı��ԣ�
    /// https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-radial-Progress-use-vector-api.html
    /// </summary>
    [UxmlElement]
    public partial class RadialProgress : VisualElement
    {
        /// <summary>
        /// ������USS��Ϊ�����������ʽ��������
        /// </summary>
        static class ClassNames
        {
            public static readonly string RadialProgress = "radial-progress";
            public static readonly string Label = "radial-progress__label";
        }

        // ��Щ��������C#��������Զ���USS���ԡ�
        static CustomStyleProperty<Color> s_TrackColor = new CustomStyleProperty<Color>("--track-color");
        static CustomStyleProperty<Color> s_ProgressColor = new CustomStyleProperty<Color>("--progress-color");

        // ����Բ����ɫ
        Color m_TrackColor = Color.black;

        // ����������ɫ
        Color m_ProgressColor = Color.red;

        // ��ʾ�ٷֱȵı�ǩ
        Label m_Label;

        // ��ǩ��ʾΪ�ٷֱȵ�����
        float m_Progress;

        [UxmlAttribute]
        public Color TrackColor
        {
            get => m_TrackColor;
            set
            {
                m_TrackColor = value;
                MarkDirtyRepaint();
            }
        }

        [UxmlAttribute]
        public Color ProgressColor
        {
            get => m_ProgressColor;
            set
            {
                m_ProgressColor = value;
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// �����Դ洢һ������0��100֮���ֵ��
        ///
        /// ע����Щ���ԣ�
        /// [UxmlAttribute] ���������ֱ����UXML���������á�
        /// [CreateProperty] �������ݰ󶨺͸���֪ͨ����������
        /// 
        /// </summary>
        [UxmlAttribute]
        [CreateProperty]
        public float Progress
        {
            // �����������ԡ�
            get => m_Progress;
            set
            {
                // ���ý���ֵ�����±�ǩ�ı�
                m_Progress = value;
                m_Label.text = Mathf.Clamp(Mathf.Round(value), 0, 100) + "%";

                // ����generateVisualContent�ص����ػ�Ԫ�ء�
                // �������ڽ��ȸ���ʱˢ��UI��
                // �����Զ���ؼ������ã�����������Ҫ�����Ӿ�����ʱ��
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// ����RadialProgress��Ψһ���캯����
        /// </summary>
        public RadialProgress()
        {
            // ����һ����ǩ�����һ��USS��������������ӵ�����Ӿ����С�
            m_Label = new Label();
            m_Label.AddToClassList(ClassNames.Label);
            Add(m_Label);

            // �ڲ�νṹ�з���һ�����ص�����
            m_Label.name = ClassNames.Label;

            // ��������ؼ���USS������
            AddToClassList(ClassNames.RadialProgress);

            // ���Զ�����ʽ������ע��һ���ص���
            RegisterCallback<CustomStyleResolvedEvent>(evt => CustomStylesResolved(evt));

            // ע��һ���ص������ɿؼ����Ӿ����ݡ�
            generateVisualContent += GenerateVisualContent;

            Progress = 0.0f;
        }

        /// <summary>
        /// �Զ�����ʽ������ɫ������ʽ�������Ӧ�á�
        /// �˷���ȷ���Զ�����ʽ�õ���Ӧ���¡�
        /// </summary>
        /// <param name="evt">�Զ�����ʽ�����󴥷����¼���</param>
        static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            RadialProgress element = (RadialProgress)evt.currentTarget;
            element.UpdateCustomStyles();
        }

        /// <summary>
        /// �Զ�����ɫ�����󣬴˷���Ϊ������ɫ���ػ�ؼ���
        /// </summary>
        void UpdateCustomStyles()
        {
            bool repaint = customStyle.TryGetValue(s_ProgressColor, out m_ProgressColor);

            if (customStyle.TryGetValue(s_TrackColor, out m_TrackColor))
                repaint = true;

            if (repaint)
                MarkDirtyRepaint();
        }

        /// <summary>
        /// ΪRadialProgress�ؼ������Ӿ����ݣ���������ͽ��Ȼ���
        /// </summary>
        /// <param name="context">���������������ݵ������ġ�</param>
        void GenerateVisualContent(MeshGenerationContext context)
        {
            float width = contentRect.width;
            float height = contentRect.height;

            // ��ȡ2D��ͼ�����Ի���ʸ��ͼ�Ρ�
            var painter = context.painter2D;

            // ���þ��������Բ���Ŀ��
            painter.lineWidth = 10.0f;

            // �ʻ���ĩ�˽���ƽ�ģ����ᳬ���˵㡣
            painter.lineCap = LineCap.Butt;

            // ���ƹ��/����Բ
            painter.strokeColor = m_TrackColor;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, 0.0f, 360.0f);
            painter.Stroke();

            // ���ƽ���
            painter.strokeColor = m_ProgressColor;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, -90.0f,
                360.0f * (Progress / 100.0f) - 90.0f);
            painter.Stroke();
        }
    }
}