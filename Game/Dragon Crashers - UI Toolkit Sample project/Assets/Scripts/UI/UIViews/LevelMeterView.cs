using UnityEngine.UIElements;
using System.Threading.Tasks;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// ʹ���Զ��徶���������ʾ��ҵ��ܾ���ȼ��������ĸ���ɫ�ȼ����ܺͣ�
    /// </summary>
    public class LevelMeterView : UIView
    {
        // ��ʾ��ҵȼ��ľ��������������Ԫ��
        RadialProgress m_LevelMeterRadialProgressBar;

        // ��ʾ��ҵ�ǰ���ֵȼ��ı�ǩ
        Label m_LevelMeterNumberLabel;

        // ��ʾ��һ����ܵȼ��������ı�ǩ����ͣʱ�ɼ���
        Label m_LevelMeterRankLabel;

        // ������ҵĵȼ����ݣ������ݰ󶨵��û�����
        readonly LevelMeterData m_LevelMeterData;

        bool m_IsRankLabelVisible;
        bool m_IsCooldownActive;

        const int k_DelayInSeconds = 1;

        /// <summary>
        /// LevelMeterView�Ĺ��캯����ʹ���ṩ�����ݳ�ʼ���ȼ���������ע����ͣ�¼���
        /// </summary>
        /// <param name="topElement">�����ȼ��������û�����ĸ�VisualElement��</param>
        /// <param name="levelMeterData">����ܵȼ�������Դ��</param>
        public LevelMeterView(VisualElement topElement, LevelMeterData levelMeterData) : base(topElement)
        {
            // ������ɫ�ȼ��仯
            m_LevelMeterData = levelMeterData;

            // ע��ָ���뿪�¼�
            topElement.RegisterCallback<PointerLeaveEvent>(evt => OnPointerLeave());

            // ʹ��ָ�밴���¼��л�������ǩ�Ŀɼ���
            topElement.RegisterCallback<PointerDownEvent>(evt => OnPointerDown());

            // ����϶����ȼ��������ϣ�������������ǩ�ɼ���
            topElement.RegisterCallback<PointerEnterEvent>(evt => OnPointerEnter());
        }

        /// <summary>
        /// ���ÿ��ӻ�Ԫ�غ����ݰ󶨡�
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            // ���ÿ��ӻ�Ԫ��
            SetVisualElements();
            // �������ݰ�
            SetupDataBindings();

            // Ĭ������
            m_LevelMeterRankLabel.style.display = DisplayStyle.Flex;
            m_LevelMeterRankLabel.style.opacity = 0;
        }

        /// <summary>
        /// ��ѯ�ȼ��������Ķ���Ԫ���Է��䲻ͬ���û�����Ԫ��
        /// </summary>
        protected override void SetVisualElements()
        {
            base.SetVisualElements();

            // ����Բ�ν�����
            m_LevelMeterRadialProgressBar = m_TopElement.Q<RadialProgress>("level-meter__radial-progress");

            // ���õȼ����ֱ�ǩ
            m_LevelMeterNumberLabel = m_TopElement.Q<Label>("level-meter__number");

            m_LevelMeterRankLabel = m_TopElement.Q<Label>("level-meter__rank");
        }

        /// <summary>
        /// ʹ������ʱ���ݰ�ϵͳ�������ݰ�
        /// </summary>
        void SetupDataBindings()
        {
            // ΪTotalLevels�������ݰ�
            var totalLevelBinding = new DataBinding()
            {
                dataSource = m_LevelMeterData, // ������Դ��LevelMeterData��
                dataSourcePath =
                    new PropertyPath(nameof(m_LevelMeterData.TotalLevels)), // ָ��TotalLevels���Ե�·��
                bindingMode = BindingMode.ToTarget // ����󶨣����� -> �û����棩
            };

            // Ϊ���������Ӧ��ÿ���󶨵�ת������int��float��
            totalLevelBinding.sourceToUiConverters.AddConverter((ref int total) => (float)total);

            // �������������Progress���԰󶨵�TotalLevels
            m_LevelMeterRadialProgressBar.SetBinding("Progress", totalLevelBinding);

            // �������ı�
            m_LevelMeterNumberLabel.SetBinding("text", totalLevelBinding);

            // ʹ�ò�ͬ��ת��������ͬ������Դ���������ͣ�ڵȼ���������ʱ��ʾ���ı�
            var rankLevelBinding = new DataBinding()
            {
                dataSource = m_LevelMeterData, // ������Դ��LevelMeterData��
                dataSourcePath =
                    new PropertyPath(nameof(m_LevelMeterData.TotalLevels)), // ָ��TotalLevels���Ե�·��
                bindingMode = BindingMode.ToTarget // ����󶨣����� -> �û����棩
            };

            rankLevelBinding.sourceToUiConverters.AddConverter(
                (ref int total) => LevelMeterData.GetRankFromLevel(total));

            m_LevelMeterRankLabel.SetBinding("text", rankLevelBinding);
        }

        /// <summary>
        /// ��ָ�루�����������»����Ԫ��ʱ���á�
        /// </summary>
        private void OnPointerEnter()
        {
            if (!m_IsRankLabelVisible)
            {
                // ��ָ��������ʱ������ʾ��ǩ
                ShowRankLabel(true);
            }
        }

        /// <summary>
        /// ��ָ�루���������뿪Ԫ��ʱ����
        /// </summary>
        private async void OnPointerLeave()
        {
            if (m_IsRankLabelVisible && !m_IsCooldownActive)
            {
                await StartCooldown();
            }
        }

        /// <summary>
        /// ��ָ�루������������ʱ����
        /// </summary>
        private void OnPointerDown()
        {
            if (!m_IsRankLabelVisible)
            {
                // ������ʾ��ǩ
                ShowRankLabel(true);
            }
        }

        /// <summary>
        /// �л�������ǩ�Ŀɼ��ԡ�
        /// </summary>
        private void ShowRankLabel(bool state)
        {
            if (state)
            {
                m_LevelMeterRankLabel.style.display = DisplayStyle.Flex;
                m_LevelMeterRankLabel.style.opacity = 1f; // ��ʾ������ǩ
                m_IsRankLabelVisible = true;
            }
            else
            {
                m_LevelMeterRankLabel.style.opacity = 0f; // ����������ǩ
                m_IsRankLabelVisible = false;
            }
        }

        /// <summary>
        /// ������ȴʱ�䣬��n�������������ǩ
        /// </summary>
        private async Task StartCooldown()
        {
            m_IsCooldownActive = true;

            await Task.Delay(k_DelayInSeconds * 1000); // �ȴ�n��
            // ��ȴʱ����������ر�ǩ
            ShowRankLabel(false);

            m_IsCooldownActive = false;
        }
    }
}