using UnityEngine;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    [RequireComponent(typeof(UIDocument))]
    public class HealthBarController : MonoBehaviour
    {

        [Header("生命值条元素")]
        [SerializeField] string m_HealthBarName = "生命值条基础";
        [SerializeField] string m_CharacterName = "角色名称";

        [SerializeField] Vector2 m_WorldSize = new Vector2(1.2f, 0.6f);
        [SerializeField] bool m_ShowStat = true;
        [SerializeField] bool m_ShowNameplate = true;
        [SerializeField] StyleSheet m_StyleSheetOverride;

        [SerializeField] float m_LowHPPercent = 25;
        [SerializeField] Sprite m_LowHPImage;
        [SerializeField] Transform transformToFollow;

        // 字符串ID
        const string k_NamePlate = "生命值条标题背景";
        const string k_Stat = "生命值条统计信息";
        const string k_HPFillImage = "生命值条进度";

        HealthBarComponent m_HealthBar;
        StyleBackground m_OriginalHPImage;
        UIDocument m_HealthBarDoc;


        void OnEnable()
        {
            MediaQueryEvents.CameraResized += OnCameraResized;
        }

        void OnDisable()
        {
            MediaQueryEvents.CameraResized -= OnCameraResized;
        }

        void HealthBarSetup()
        {
            m_HealthBarDoc = GetComponent<UIDocument>();

            VisualElement rootElement = m_HealthBarDoc.rootVisualElement;

            if (m_StyleSheetOverride != null)
            {
                rootElement.styleSheets.Clear();
                rootElement.styleSheets.Add(m_StyleSheetOverride);
            }

            m_HealthBar = rootElement.Q<HealthBarComponent>(m_HealthBarName);
            m_HealthBar.HealthBarTitle = m_CharacterName;

            m_HealthBar.HealthData = new HealthData();

            // 显示名称和统计信息
            ShowNameAndStats(m_ShowNameplate, m_ShowStat);
            MoveToWorldPosition(m_HealthBar, transformToFollow.position, m_WorldSize);

        }

        public void DisplayHealthBar(bool state)
        {
            if (m_HealthBarDoc == null)
                return;

            VisualElement rootElement = m_HealthBarDoc.rootVisualElement;
            rootElement.style.display = (state) ? DisplayStyle.Flex : DisplayStyle.None;
        }


        /// <summary>
        /// 设置生命值条
        /// </summary>
        /// <param name="health"></param>
        /// <param name="maxHealth"></param>
        public void SetHealth(int health, int maxHealth)
        {
            if (m_HealthBar == null)
            {
                HealthBarSetup();
            }

            m_HealthBar.HealthData.CurrentHealth = health;
            m_HealthBar.HealthData.MaximumHealth = maxHealth;
        }


        // 低生命值时切换生命值条精灵
        public void UpdateHealth(int health)
        {
            if (m_OriginalHPImage == null)
            {
                // 存储原始背景样式以重置生命值条精灵
                m_OriginalHPImage = m_HealthBar.Q<VisualElement>(k_HPFillImage).style.backgroundImage;
            }

            float lowHealth = m_HealthBar.HealthData.MaximumHealth * m_LowHPPercent / 100;
            VisualElement fill = m_HealthBar.Q<VisualElement>(k_HPFillImage);

            if (health < lowHealth && m_LowHPImage != null)
            {
                fill.style.backgroundImage = new StyleBackground(m_LowHPImage);
            }
            else
            {
                fill.style.backgroundImage = m_OriginalHPImage;
            }

            m_HealthBar.HealthData.CurrentHealth = health;
        }

        void ShowNameAndStats(bool nameVisible, bool statVisible)
        {
            VisualElement nameplate = m_HealthBar.Q<VisualElement>(k_NamePlate);
            VisualElement stat = m_HealthBar.Q<Label>(k_Stat);

            if (nameplate != null)
            {
                nameplate.visible = nameVisible;
            }

            if (stat != null)
            {
                stat.visible = statVisible;
            }

        }

        // 将生命值条移动到匹配世界位置
        void MoveToWorldPosition(VisualElement element, Vector3 worldPosition, Vector2 worldSize)
        {
            Rect rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(element.panel, worldPosition, worldSize, Camera.main);
            element.transform.position = rect.position;

        }

        // 相机更新时刷新生命值条设置
        void OnCameraResized()
        {
            UpdateHealthBar();
        }

        void UpdateHealthBar()
        {
            ShowNameAndStats(m_ShowNameplate, m_ShowStat);
            MoveToWorldPosition(m_HealthBar, transformToFollow.position, m_WorldSize);
        }

        void LateUpdate()
        {
            UpdateHealthBar();
        }
    }
}