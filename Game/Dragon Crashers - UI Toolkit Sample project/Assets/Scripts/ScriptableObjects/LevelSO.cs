using Unity.Properties;
using UnityEngine;
using UnityEngine.Localization;

namespace UIToolkitDemo
{
    /// <summary>
    /// 保存基本的关卡信息（标签名称、关卡编号、要加载的场景名称、用于显示的缩略图等）
    /// </summary>
    [CreateAssetMenu(fileName = "Assets/Resources/GameData/Levels/LevelData", menuName = "UIToolkitDemo/Level", order = 11)]
    public class LevelSO : ScriptableObject
    {

        [Tooltip("背景图片")]
        [SerializeField] Sprite thumbnail;  // 缩略图

        [Tooltip("关卡编号")]
        [SerializeField] int levelNumber;  // 关卡编号

        [Tooltip("关卡的描述性名称")]
        [SerializeField] string levelLabel;  // 关卡标签

        [Tooltip("要加载的场景名称")]
        [SerializeField] string sceneName;  // 场景名称


        [SerializeField] LocalizedString m_LocalizedLevelNamePrefix;  // 本地化的关卡名称前缀
        [SerializeField] LocalizedString m_LocalizedLevelSubtitle;  // 本地化的关卡副标题

        public Sprite Thumbnail => thumbnail;  // 缩略图
        public string SceneName => sceneName;  // 场景名称

        /// <summary>
        /// 如果可用，返回关卡名称描述的本地化版本（例如 "The Dragon's Lair"）。否则，返回空字符串。
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
                return string.Empty;  // 回退到空字符串
            }
        }

        /// <summary>
        /// 返回表示关卡编号的本地化字符串（例如 "Level 1", "Nivel 1", "Nivieu 1"）
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

                return "Level 1"; // 回退
            }
        }
    }
}