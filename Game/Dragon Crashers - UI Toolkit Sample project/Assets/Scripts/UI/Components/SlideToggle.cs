// ����һ���Զ���ؼ���ʾ����չʾ��һ�����ƿ��ص��л��ؼ���
// �ֶ��ĵ���https://docs.unity3d.com/Manual/UIE-slide-toggle.html

using UnityEngine;
using UnityEngine.UIElements;

namespace MyUILibrary
{
    /// <summary>
    /// һ����BaseField<bool>�������Զ������ƿ��ص��л��ؼ���
    /// </summary>
    [UxmlElement]
    public partial class SlideToggle : BaseField<bool>
    {
        // �л��ؼ��Ŀ顢Ԫ�غ�״̬������������ʹ��BEM��׼��

        public static readonly new string ussClassName = "slide-toggle";
        public static readonly new string inputUssClassName = "slide-toggle__input";
        public static readonly string inputKnobUssClassName = "slide-toggle__input-knob";
        public static readonly string inputCheckedUssClassName = "slide-toggle__input--checked";
        public static readonly string stateLabelUssClassName = "slide-toggle__state-label";

        bool m_IsOn;

        /// <summary>
        /// �л��ؼ����ڡ�����״̬ʱ��ʾ�ı�ǩ��
        /// </summary>
        public string OnLabel { get; set; } = "��"; // Ĭ��ֵ

        /// <summary>
        /// �л��ؼ����ڡ��ء�״̬ʱ��ʾ�ı�ǩ��
        /// </summary>
        public string OffLabel { get; set; } = "��"; // Ĭ��ֵ

        /// <summary>
        /// �л�״̬�����ԡ�
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

        VisualElement m_Input; // �л��ؼ������벿�֡�
        VisualElement m_Knob; // ��ťԪ�ء�
        Label m_StateLabel; // ��/�ر�ǩ��

        /// <summary>
        /// Ĭ�Ϲ��캯�������ô����е���һ�����캯������
        /// </summary>
        public SlideToggle() : this(null)
        {
        }

        /// <summary>
        /// ����ǩ�Ĺ��캯����
        /// </summary>
        /// <param name="label">�л��ؼ���ǩ���ı���</param>
        public SlideToggle(string label) : base(label, null)
        {
            // ������ʽ����
            AddToClassList(ussClassName);

            // ��ȡBaseField���Ӿ�����Ԫ�أ���������������ı�����
            m_Input = this.Q(className: BaseField<bool>.inputUssClassName);
            m_Input.AddToClassList(inputUssClassName);
            Add(m_Input);

            // ����һ������ť����Ԫ����Ϊ�����������л��ؼ���ʵ�ʻ��顣
            m_Knob = new();
            m_Knob.AddToClassList(inputKnobUssClassName);
            m_Input.Add(m_Knob);

            m_StateLabel = new Label();
            m_StateLabel.AddToClassList(stateLabelUssClassName);
            m_Input.Add(m_StateLabel);

            // ��������Ҫ��ʽ�������ͣ��SlideToggle�����������¼��������ʹ��
            // �Զ���ؼ����ʵ���������ľ�̬����ģʽ��

            // ��ָ�밴�º�̧�������з���ʱ��ClickEvent������
            RegisterCallback<ClickEvent>(evt => OnClick(evt));

            // ���ֶλ�ý������û����¼�ʱ��KeydownEvent������
            RegisterCallback<KeyDownEvent>(evt => OnKeydownEvent(evt));

            // NavigationSubmitEvent������ʱ������Լ��̡���Ϸ�ֱ��������豸�����롣
            RegisterCallback<NavigationSubmitEvent>(evt => OnSubmit(evt));

            // 
            UpdateStateLabel();
        }

        /// <summary>
        /// ����ָ�����¼����л����ء�
        /// </summary>
        /// <param name="evt"></param>
        static void OnClick(ClickEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;
            slideToggle.ToggleValue();

            evt.StopPropagation();
        }

        /// <summary>
        /// �����ύ�¼�������/��Ϸ�ֱ������л����ء�
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
        /// �������¼����л����ء�
        /// </summary>
        /// <param name="evt"></param>
        static void OnKeydownEvent(KeyDownEvent evt)
        {
            var slideToggle = evt.currentTarget as SlideToggle;

            if (slideToggle == null)
                return;

            // NavigationSubmitEvent�¼��Ѿ�����������ʱ�İ����¼�����˴˷�����Ӧ����
            // ���ǡ�
            if (slideToggle.panel.contextType == ContextType.Player)
                return;

            // �����û�����Enter��Return��Spaceʱ�л�ֵ��
            if (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.Space)
            {
                slideToggle.ToggleValue();
                evt.StopPropagation();
            }
        }

        /// <summary>
        /// ���������ص������ô˷�����
        /// </summary>
        void ToggleValue()
        {
            value = !value;
            UpdateStateLabel();
        }

        /// <summary>
        /// ���ÿ��ص�ֵ�������͸���֪ͨ���������л��ؼ����Ӿ�״̬��
        /// 
        /// ����ToggleValue���޸�ֵʱ���õ�ChangeEvent������
        /// </summary>
        /// <param name="newValue">���ص���״̬��</param>
        public override void SetValueWithoutNotify(bool newValue)
        {
            base.SetValueWithoutNotify(newValue);

            // �����������Ԫ�ص���ʽ��ʹ�俴�������û���á�
            m_Input.EnableInClassList(inputCheckedUssClassName, newValue);

            // ���¿�/���ı�
            UpdateStateLabel();
        }

        /// <summary>
        /// ���¿�/�ر�ǩ��
        /// </summary>
        void UpdateStateLabel()
        {
            if (value) // ����л��ؼ����ڿ�״̬
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