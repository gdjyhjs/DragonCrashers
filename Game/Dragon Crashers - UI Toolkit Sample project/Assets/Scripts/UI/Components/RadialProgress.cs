using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 一个自定义UI元素，代表一个径向进度指示器/圆形进度条，使用矢量API创建。内部的标签以百分比形式显示当前进度。
    /// 
    /// 改编自：
    /// https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-radial-Progress-use-vector-api.html
    /// </summary>
    [UxmlElement]
    public partial class RadialProgress : VisualElement
    {
        /// <summary>
        /// 用于在USS中为该组件设置样式的类名。
        /// </summary>
        static class ClassNames
        {
            public static readonly string RadialProgress = "radial-progress";
            public static readonly string Label = "radial-progress__label";
        }

        // 这些对象允许C#代码访问自定义USS属性。
        static CustomStyleProperty<Color> s_TrackColor = new CustomStyleProperty<Color>("--track-color");
        static CustomStyleProperty<Color> s_ProgressColor = new CustomStyleProperty<Color>("--progress-color");

        // 背景圆的颜色
        Color m_TrackColor = Color.black;

        // 进度条的颜色
        Color m_ProgressColor = Color.red;

        // 显示百分比的标签
        Label m_Label;

        // 标签显示为百分比的数字
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
        /// 此属性存储一个介于0和100之间的值。
        ///
        /// 注意这些属性：
        /// [UxmlAttribute] 允许此属性直接在UXML定义中设置。
        /// [CreateProperty] 允许数据绑定和更改通知正常工作。
        /// 
        /// </summary>
        [UxmlAttribute]
        [CreateProperty]
        public float Progress
        {
            // 公开进度属性。
            get => m_Progress;
            set
            {
                // 设置进度值并更新标签文本
                m_Progress = value;
                m_Label.text = Mathf.Clamp(Mathf.Round(value), 0, 100) + "%";

                // 触发generateVisualContent回调并重绘元素。
                // 这用于在进度更改时刷新UI。
                // 对于自定义控件很有用，尤其是在需要更新视觉内容时。
                MarkDirtyRepaint();
            }
        }

        /// <summary>
        /// 这是RadialProgress的唯一构造函数。
        /// </summary>
        public RadialProgress()
        {
            // 创建一个标签，添加一个USS类名，并将其添加到这个视觉树中。
            m_Label = new Label();
            m_Label.AddToClassList(ClassNames.Label);
            Add(m_Label);

            // 在层次结构中分配一个独特的名称
            m_Label.name = ClassNames.Label;

            // 添加整个控件的USS类名。
            AddToClassList(ClassNames.RadialProgress);

            // 在自定义样式解析后注册一个回调。
            RegisterCallback<CustomStyleResolvedEvent>(evt => CustomStylesResolved(evt));

            // 注册一个回调以生成控件的视觉内容。
            generateVisualContent += GenerateVisualContent;

            Progress = 0.0f;
        }

        /// <summary>
        /// 自定义样式（如颜色）在样式表解析后应用。
        /// 此方法确保自定义样式得到相应更新。
        /// </summary>
        /// <param name="evt">自定义样式解析后触发的事件。</param>
        static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            RadialProgress element = (RadialProgress)evt.currentTarget;
            element.UpdateCustomStyles();
        }

        /// <summary>
        /// 自定义颜色解析后，此方法为网格着色并重绘控件。
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
        /// 为RadialProgress控件生成视觉内容，包括轨道和进度弧。
        /// </summary>
        /// <param name="context">用于生成网格内容的上下文。</param>
        void GenerateVisualContent(MeshGenerationContext context)
        {
            float width = contentRect.width;
            float height = contentRect.height;

            // 获取2D绘图对象以绘制矢量图形。
            var painter = context.painter2D;

            // 设置径向进度条圆弧的宽度
            painter.lineWidth = 10.0f;

            // 笔划的末端将是平的，不会超出端点。
            painter.lineCap = LineCap.Butt;

            // 绘制轨道/背景圆
            painter.strokeColor = m_TrackColor;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, 0.0f, 360.0f);
            painter.Stroke();

            // 绘制进度
            painter.strokeColor = m_ProgressColor;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, -90.0f,
                360.0f * (Progress / 100.0f) - 90.0f);
            painter.Stroke();
        }
    }
}