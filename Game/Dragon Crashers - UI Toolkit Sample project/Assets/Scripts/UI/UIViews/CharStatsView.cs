using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// ����CharView��Ļ�Ľ�ɫͳ�Ʋ���
    /// </summary>
    public class CharStatsView : UIView
    {
        // �����ȴʱ��
        const float k_ClickCooldown = 0.2f;

        // �´οɵ����ť��ʱ��
        float m_TimeToNextClick = 0f;

        // ��Ϸͼ������
        GameIconsSO m_GameIconsData;

        // ��ɫ����
        CharacterData m_CharacterData;

        // ��̬��������
        CharacterBaseSO m_BaseStats;

        // �ȼ���ǩ
        Label m_LevelLabel;

        // ����
        VisualElement m_ActiveFrame;
        // �����
        int m_ActiveIndex;

        // ����ͼ������
        VisualElement[] m_SkillIcons = new VisualElement[3];
        // ������������
        SkillSO[] m_BaseSkills = new SkillSO[3];

        // ��ɫ���ǩ
        Label m_CharacterClass;
        // ϡ�жȱ�ǩ
        Label m_Rarity;
        // �������ͱ�ǩ
        Label m_AttackType;

        // ��һ�����ܰ�ť
        Button m_NextSkillButton;
        // ��һ�����ܰ�ť
        Button m_LastSkillButton;

        // ���Ǳ����ǩ
        Label m_BioTitle;

        // ���ú��������ڷ���

        /// <summary>
        /// ʹ��ָ���Ķ���UIԪ�س�ʼ��CharStatsView����ʵ����
        /// </summary>
        /// <param name="topElement">UI�ĸ��Ӿ�Ԫ�ء�</param>
        public CharStatsView(VisualElement topElement) : base(topElement)
        {
        }

        /// <summary>
        /// ע���κλص���
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            // ע���ص�
            UnregisterCallbacks();
        }

        /// <summary>
        /// ��ѯ����ʼ�����б�����Ӿ�Ԫ�ء�
        /// </summary>
        protected override void SetVisualElements()
        {
            // ��ȡ�ȼ���ǩ
            m_LevelLabel = m_TopElement.Q<Label>("char-stats__level-number");

            // ��ȡ��ɫ���ǩ
            m_CharacterClass = m_TopElement.Q<Label>("char-stats__class-label");
            // ��ȡϡ�жȱ�ǩ
            m_Rarity = m_TopElement.Q<Label>("char-stats__rarity-label");
            // ��ȡ�������ͱ�ǩ
            m_AttackType = m_TopElement.Q<Label>("char-stats__attack-type-label");

            // ��ȡ����ͼ��
            m_SkillIcons[0] = m_TopElement.Q("char-skills__icon1");
            m_SkillIcons[1] = m_TopElement.Q("char-skills__icon2");
            m_SkillIcons[2] = m_TopElement.Q("char-skills__icon3");

            // ��ȡ��һ�����ܰ�ť
            m_NextSkillButton = m_TopElement.Q<Button>("char-skills__next-button");
            // ��ȡ��һ�����ܰ�ť
            m_LastSkillButton = m_TopElement.Q<Button>("char-skills__last-button");

            // ��ȡ���Ǳ����ǩ
            m_BioTitle = m_TopElement.Q<Label>("char-bio__title");

            // ��ȡ����
            m_ActiveFrame = m_TopElement.Q("char-skills__active");

            // ȷ���ڰ�֮ǰ����m_GameIconsData
            m_GameIconsData = Resources.Load("GameData/GameIcons") as GameIconsSO;
            if (m_GameIconsData == null)
            {
                Debug.LogError("[CharStatsView] SetVisualElements: �޷���Resources����GameIconsSO��");
            }
        }

        /// <summary>
        /// Ϊ����ʽUIԪ��ע���¼��ص���
        /// </summary>
        protected override void RegisterButtonCallbacks()
        {
            // ���õ���¼�
            m_NextSkillButton.RegisterCallback<ClickEvent>(SelectNextSkill);
            m_LastSkillButton.RegisterCallback<ClickEvent>(SelectLastSkill);
            m_SkillIcons[0].RegisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[1].RegisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[2].RegisterCallback<ClickEvent>(SelectSkill);

            // ���ֹ������ʼ������λ��
            m_SkillIcons[0].RegisterCallback<GeometryChangedEvent>(InitializeSkillMarker);
        }

        // ���·���/���ݰ�

        /// <summary>
        /// �л���ɫʱ���´��ڡ���CharView���ⲿ���á�
        /// </summary>
        /// <param name="charData">Ҫ��ʾ�Ľ�ɫ���ݡ�</param>
        public void UpdateCharacterStats(CharacterData charData)
        {
            m_CharacterData = charData;

            // ���漼��
            m_BaseStats = charData.CharacterBaseData;
            m_BaseSkills[0] = m_BaseStats.Skill1;
            m_BaseSkills[1] = m_BaseStats.Skill2;
            m_BaseSkills[2] = m_BaseStats.Skill3;

            // ʹ�õ�ǰ�ȼ��������м���
            foreach (var skill in m_BaseSkills)
            {
                if (skill != null)
                {
                    // ���¼��ܵȼ�
                    skill.UpdateLevel(charData.CurrentLevel);
                }
            }

            // ���ý�ɫͳ�ƺʹ��ǵ�����Դ
            SetCharDataSource();

            // ���°󶨽�ɫ�ʹ������ݣ���һ���͵�������ǩҳ��
            BindCharacterData();

            // ������ѡ����
            UpdateSkillView(0);
        }

        /// <summary>
        /// ���ý�ɫͳ�ƺʹ���Ԫ�ص�����Դ��
        /// </summary>
        void SetCharDataSource()
        {
            // ȷ����ɫ����������Ҫ����Դ
            m_LevelLabel.dataSource = m_CharacterData;

            // ���ý�ɫͳ�Ƶ�����Դ
            VisualElement statsContainer = m_TopElement.Q<VisualElement>("char-data-stats-content");
            statsContainer.dataSource = m_BaseStats;

            // ���ô��Ǻͱ��������Դ
            VisualElement bioContainer = m_TopElement.Q<VisualElement>("char-data-bio-content");
            bioContainer.dataSource = m_BaseStats;

            // ���½�ɫͼ��
            m_GameIconsData.UpdateIcons(m_BaseStats.CharacterClass, m_BaseStats.Rarity, m_BaseStats.AttackType);

            // ���ü���ͼ�������Դ
            m_SkillIcons[0].dataSource = m_BaseStats;
            m_SkillIcons[1].dataSource = m_BaseStats;
            m_SkillIcons[2].dataSource = m_BaseStats;
        }

        /// <summary>
        /// ���ü���Ԫ�ص�����Դ��
        /// </summary>
        /// <param name="newSkillDataSource"></param>
        void SetSkillDataSources(SkillSO newSkillDataSource)
        {
            VisualElement skillContainer = m_TopElement.Q<VisualElement>("char-data-skills-content");
            skillContainer.dataSource = newSkillDataSource;
        }

        /// <summary>
        /// ����/���°���ҪԤ���������LocalizedStrings�� 
        /// ��ЩLocalizedStrings����ö�ٲ��ڱ��ػ�֮ǰת��Ϊ�ַ�����
        /// </summary>
        void BindCharacterData()
        {
            if (m_BaseStats == null)
            {
                Debug.LogWarning("[CharStatsView] BindCharacterData: m_BaseStatsΪ��");
                return;
            }

            // ��LocalizedStrings�󶨵���Ҫ��ö��ֵ����Ԥ�����UIԪ��
            m_CharacterClass.SetBinding("text", m_BaseStats.CharacterClassLocalized);
            m_Rarity.SetBinding("text", m_BaseStats.RarityLocalized);
            m_AttackType.SetBinding("text", m_BaseStats.AttackTypeLocalized);
            m_BioTitle.SetBinding("text", m_BaseStats.BioTitleLocalized);
        }

        /// <summary>
        /// �ӽ���ʽUIԪ��ע���¼��ص��Է�ֹ�ڴ�й©��
        /// </summary>
        void UnregisterCallbacks()
        {
            m_NextSkillButton.UnregisterCallback<ClickEvent>(SelectNextSkill);
            m_LastSkillButton.UnregisterCallback<ClickEvent>(SelectLastSkill);
            m_SkillIcons[0].UnregisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[1].UnregisterCallback<ClickEvent>(SelectSkill);
            m_SkillIcons[2].UnregisterCallback<ClickEvent>(SelectSkill);

            // ���ֹ������ʼ������λ��
            m_SkillIcons[0].UnregisterCallback<GeometryChangedEvent>(InitializeSkillMarker);
        }

        /// <summary>
        /// ����Ӱ�ťѡ����һ�����ܵĵ���¼���
        /// </summary>
        /// <param name="evt"></param>
        void SelectLastSkill(ClickEvent evt)
        {
            if (Time.time < m_TimeToNextClick)
                return;

            m_TimeToNextClick = Time.time + k_ClickCooldown;

            // ����ֱ�ӵ���Ӿ�Ԫ��ʱѡ��
            m_ActiveIndex--;
            if (m_ActiveIndex < 0)
            {
                m_ActiveIndex = 2;
            }

            // ���¼�����ͼ
            UpdateSkillView(m_ActiveIndex);
            // ����Ĭ�ϰ�ť��Ч
            AudioManager.PlayDefaultButtonSound();
        }

        /// <summary>
        /// ����Ӱ�ťѡ����һ�����ܵĵ���¼���
        /// </summary>
        /// <param name="evt"></param>
        void SelectNextSkill(ClickEvent evt)
        {
            if (Time.time < m_TimeToNextClick)
                return;

            m_TimeToNextClick = Time.time + k_ClickCooldown;
            m_ActiveIndex++;

            if (m_ActiveIndex > 2)
            {
                m_ActiveIndex = 0;
            }

            // ���¼�����ͼ
            UpdateSkillView(m_ActiveIndex);
            // ����Ĭ�ϰ�ť��Ч
            AudioManager.PlayDefaultButtonSound();
        }

        /// <summary>
        /// ���ݵ��Ԫ�ص�����ȷ��������ĸ�����ͼ�ꡣ
        /// </summary>
        /// <param name="evt">���ܰ�ť�ĵ���¼���</param>
        void SelectSkill(ClickEvent evt)
        {
            VisualElement clickedElement = evt.target as VisualElement;

            if (clickedElement == null)
                return;

            // �ӵ��Ԫ�ص������л�ȡ���һ���ַ�������char-skills__icon1, char-skills__icon2�ȣ�
            // ������ת��Ϊ����
            int index = (int)Char.GetNumericValue(clickedElement.name[clickedElement.name.Length - 1]) - 1;
            index = Mathf.Clamp(index, 0, m_BaseSkills.Length - 1);

            // ��ʾ��Ӧ��ScriptableObject����
            UpdateSkillView(index);

            // ���ŵ����Ч
            AudioManager.PlayAltButtonSound();
        }

        /// <summary>
        /// ����UI����ʾָ�����������ܵ���Ϣ��
        /// </summary>
        /// <param name="index">Ҫ��ʾ�ļ��ܵ�������</param>
        void UpdateSkillView(int index)
        {
            // û����Ч��ɫ����ʱ��ֹ
            if (m_CharacterData == null || m_BaseSkills == null || m_BaseSkills[index] == null)
                return;

            SkillSO newSkillDataSource = m_BaseSkills[index];

            // ������Ϊ����Դ֮ǰ���¼��ܵĵȼ�
            newSkillDataSource.UpdateLevel(m_CharacterData.CurrentLevel);

            // ��������Դ��ʹ�õ�ǰѡ��ļ���
            SetSkillDataSources(newSkillDataSource);

            // ͻ����ʾ��ѡ����ͼ��
            MarkTargetElement(m_SkillIcons[index], 300);

            // �����µĻ����
            m_ActiveIndex = index;
        }

        /// <summary>
        /// UI���ֹ������ʼ������ܱ�ǵ�λ�á�
        /// </summary>
        /// <param name="evt">���α仯�¼����ݡ�</param>
        // ���ֹ��������û���
        void InitializeSkillMarker(GeometryChangedEvent evt)
        {
            // ����λ�������ڵ�һ��ͼ����
            MarkTargetElement(m_SkillIcons[0], 0);
        }

        /// <summary>
        /// ������������ͻ����ʾָ����Ŀ��Ԫ�ء�
        /// </summary>
        /// <param name="targetElement">Ҫͻ����ʾ��UIԪ�ء�</param>
        /// <param name="duration">��������ʱ�䣨���룩��</param>
        void MarkTargetElement(VisualElement targetElement, int duration = 200)
        {
            // Ŀ��Ԫ�أ�ת��Ϊ���ܵĸ��ռ�
            Vector3 targetInRootSpace = GetRelativePosition(targetElement, m_ActiveFrame);

            // ���ƫ��
            Vector3 offset = new Vector3(10, 10, 0f);

            // ����������λ��
            m_ActiveFrame.experimental.animation.Position(targetInRootSpace - offset, duration);
        }

        /// <summary>
        /// ��Ԫ�ص�λ��ת��Ϊ��һ��Ԫ�صĸ�Ԫ�ص�����ռ䡣
        /// </summary>
        /// <param name="elementToConvert">Ҫת����Ԫ�ء�</param>
        /// <param name="targetElement">Ŀ��Ԫ�ء�</param>
        /// <returns>Ŀ��Ԫ�����¸��ռ��е�λ�á�</returns>
        static Vector3 GetRelativePosition(VisualElement elementToConvert, VisualElement targetElement)
        {
            Vector2 worldSpacePosition = elementToConvert.parent.LocalToWorld(elementToConvert.layout.position);
            VisualElement newRoot = targetElement.parent;
            return newRoot.WorldToLocal(worldSpacePosition);
        }
    }
}