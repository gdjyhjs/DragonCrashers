using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    /// <summary>
    /// ��������Ĺؿ���Ϣ����ǩ���ơ��ؿ���š�Ҫ���صĳ������ơ�������ʾ������ͼ�ȣ�
    /// </summary>
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Levels/LevelData", menuName = "UIToolkitDemo/Level", order = 11)]
    public class LevelSO : ScriptableObject
    {

        [Tooltip("����ͼƬ")]
        [SerializeField] Sprite thumbnail;  // ����ͼ

        [Tooltip("�ؿ����")]
        [SerializeField] int levelNumber;  // �ؿ����

        [Tooltip("�ؿ�������������")]
        [SerializeField] string levelLabel;  // �ؿ���ǩ

        [Tooltip("Ҫ���صĳ�������")]
        [SerializeField] string sceneName;  // ��������


        [SerializeField] LocalizedString m_LocalizedLevelNamePrefix;  // ���ػ��Ĺؿ�����ǰ׺
        [SerializeField] LocalizedString m_LocalizedLevelSubtitle;  // ���ػ��Ĺؿ�������

        public Sprite Thumbnail => thumbnail;  // ����ͼ
        public string SceneName => sceneName;  // ��������

        /// <summary>
        /// ������ã����عؿ����������ı��ػ��汾������ "The Dragon's Lair"�������򣬷��ؿ��ַ�����
        /// </summary>
        [CreateProperty]
        public string LevelSubtitle
        {
            get
            {
                if (m_LocalizedLevelSubtitle != null && !string.IsNullOrEmpty(m_LocalizedLevelSubtitle.GetLocalizedString()))
                {
                    return m_LocalizedLevelSubtitle.GetLocalizedString();
                }
                return string.Empty;  // ���˵����ַ���
            }
        }

        /// <summary>
        /// ���ر�ʾ�ؿ���ŵı��ػ��ַ��������� "Level 1", "Nivel 1", "Nivieu 1"��
        /// </summary>
        [CreateProperty]
        public string LevelNumberFormatted
        {
            get
            {
                if (m_LocalizedLevelNamePrefix != null &&
                    !string.IsNullOrEmpty(m_LocalizedLevelNamePrefix.GetLocalizedString()))
                {
                    return m_LocalizedLevelNamePrefix.GetLocalizedString() + " " + levelNumber;
                }

                return "Level 1"; // ����
            }
        }
    }
}