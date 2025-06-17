using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    // 在游戏中使用，代表一个带有进度条的角色头像
    public class CharacterCard : MonoBehaviour
    {
        public TemplateContainer CharacterTemplate;
        public HealthBarComponent CooldownBar;

        const string k_CooldownBar = "game-char__progress-bar";

        UnitInfoData m_HeroData;
        VisualTreeAsset m_CharacterVisualTreeAsset;

        public UnitInfoData HeroData
        {
            get => m_HeroData;
            set
            {
                m_HeroData = value;
                VisualElement charPortraitImage = CharacterTemplate.Q<VisualElement>("game-char__image");
                charPortraitImage.style.backgroundImage = new StyleBackground(m_HeroData.unitAvatar);
            }
        }

        public VisualTreeAsset CharacterVisualTreeAsset
        {
            get => m_CharacterVisualTreeAsset;
            set => SetupCard(value);
        }

        // 设置卡片
        void SetupCard(VisualTreeAsset visualTree)
        {
            m_CharacterVisualTreeAsset = visualTree;
            CharacterTemplate = m_CharacterVisualTreeAsset.CloneTree();
            CooldownBar = CharacterTemplate.Q<HealthBarComponent>(k_CooldownBar);

            CooldownBar.HealthData = new HealthData();
        }
    }
}