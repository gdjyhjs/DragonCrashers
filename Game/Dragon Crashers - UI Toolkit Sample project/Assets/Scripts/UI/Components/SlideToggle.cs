// 这是一个自定义控件的示例，展示了一个类似开关的切换控件。
// 手动文档：https://docs.unity3d.com/Manual/UIE-slide-toggle.html

using UnityEngine;
using UnityEngine.UIElements;

namespace MyUILibrary
{
    /// <summary>
    /// 一个从BaseField<bool>派生的自定义类似开关的切换控件。
    /// </summary>
    [UxmlElement]
    public partial class SlideToggle : BaseField<bool>
    {
        // 切换控件的块、元素和状态的类名。名称使用BEM标准。

        public static readonly new string ussClassName = "slide-toggle";
        public static readonly new string inputUssClassName = "slide-toggle__input";
        public static readonly string inputKnobUssClassName = "slide-toggle__input-knob";
        public static readonly string inputCheckedUssClassName = "slide-toggle__input--checked";
        public static readonly string stateLabelUssClassName = "slide-toggle__state-label";

        bool m_IsOn;

        /// <summary>
        /// 切换控件处于“开”状态时显示的标签。
        /// </summary>
        public string OnLabel { get; set; } = "开"; // 默认值

        /// <summary>
        /// 切换控件处于“关”状态时显示的标签。
        /// </summary>
        public string OffLabel { get; set; } = "关"; // 默认值

        /// <summary>
        /// 切换状态的属性。
        /// </summary>
        [UxmlAttribute("IsOn")]
        public bool IsOn
        {
            get => m_IsOn;
            set
            {
                if (m_IsOn == value)
                    return;

                m_IsOn = value;
                UpdateStateLabel();
                SetValueWithoutNotify(m_IsOn);
            }
        }

        VisualElement m_Input; // 切换控件的输入部分。
        VisualElement m_Knob; // 旋钮元素。
        Label m_StateLabel; // 开/关标签。

        /// <summary>
        /// 默认构造函数（调用此类中的另一个构造函数）。
        /// </summary>
        public SlideToggle() : this(null)
        {
        }

        /// <summary>
        /// 带标签的构造函数。
        /// </summary>
        /// <param name="label">切换控件标签的文本。</param>
        public SlideToggle(string label) : base(label, null)
        {
            // 整体样式设置
            AddToClassList(ussClassName);

            // 获取BaseField的视觉输入元素，并将其用作滑块的背景。
            m_Input = this.Q(className: BaseField<bool>.inputUssClassName);
            m_Input.AddToClassList(inputUssClassName);
            Add(m_Input);

            // 创建一个“旋钮”子元素作为背景，代表切换控件的实际滑块。
            m_Knob = new();
            m_Knob.AddToClassList(inputKnobUssClassName);
            m_Input.Add(m_Knob);

            m_StateLabel = new Label();
            m_StateLabel.AddToClassList(stateLabelUssClassName);
            m_Input.Add(m_StateLabel);

            // 有三种主要方式来激活或停用SlideToggle。所有三个事件处理程序都使用
            // 自定义控件最佳实践中描述的静态函数模式。

            // 当指针按下和抬起动作序列发生时，ClickEvent触发。
            RegisterCallback<ClickEvent>(evt => OnClick(evt));

            // 当字段获得焦点且用户按下键时，KeydownEvent触发。
            RegisterCallback<KeyDownEvent>(evt => OnKeydownEvent(evt));

            // NavigationSubmitEvent在运行时检测来自键盘、游戏手柄或其他设备的输入。
            RegisterCallback<NavigationSubmitEvent>(evt => OnSubmit(evt));

            // 
            UpdateStateLabel();
        }

        /// <summary>
        /// 处理指针点击事件以切换开关。
        /// </summary>
        /// <param name="evt"></param>
        static void OnClick(ClickEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;
            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        /// <summary>
        /// 处理提交事件（键盘/游戏手柄）以切换开关。
        /// </summary>
        /// <param name="evt"></param>
        static void OnSubmit(NavigationSubmitEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;

            if (slideToggle == null)
                return;

            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        /// <summary>
        /// 处理按键事件以切换开关。
        /// </summary>
        /// <param name="evt"></param>
        static void OnKeydownEvent(KeyDownEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;

            if (slideToggle == null)
                return;

            // NavigationSubmitEvent事件已经涵盖了运行时的按键事件，因此此方法不应处理
            // 它们。
            if (slideToggle.panel.contextType == ContextType.Player)
                return;

            // 仅当用户按下Enter、Return或Space时切换值。
            if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
            {
                slideToggle.ToggleValue();
                evt.StopPropagation();
            }
        }

        /// <summary>
        /// 所有三个回调都调用此方法。
        /// </summary>
        void ToggleValue()
        {
            value = !value;
            UpdateStateLabel();
        }

        /// <summary>
        /// 设置开关的值而不发送更改通知，并更新切换控件的视觉状态。
        /// 
        /// 由在ToggleValue中修改值时调用的ChangeEvent触发。
        /// </summary>
        /// <param name="newValue">开关的新状态。</param>
        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);

            // 这会设置输入元素的样式，使其看起来启用或禁用。
            m_Input.EnableInClassList(inputCheckedUssClassName, newValue);

            // 更新开/关文本
            UpdateStateLabel();
        }

        /// <summary>
        /// 更新开/关标签。
        /// </summary>
        void UpdateStateLabel()
        {
            if (value) // 如果切换控件处于开状态
            {
                m_StateLabel.text = OnLabel;
            }
            else
            {
                m_StateLabel.text = OffLabel;
            }
        }
    }
}