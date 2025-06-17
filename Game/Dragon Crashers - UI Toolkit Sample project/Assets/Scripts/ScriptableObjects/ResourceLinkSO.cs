using UnityEngine;
using Unity.Properties;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// 此脚本公开了一个唯一标识符、文本标签和关联的 URL，用于 UI Toolkit。
    /// 这允许在不影响 UXML 文件或相关脚本的情况下更改 URL 和元素。
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceLinkData", menuName = "UIToolkitDemo/Resource Link")]
    public class ResourceLinkSO : ScriptableObject
    {
        // 属性标识符，用于更改通知
        // public static readonly BindingId buttonLabelProperty = nameof(buttonLabel);
        public static readonly BindingId TargetURLProperty = nameof(TargetURL);

        // 后备字段
        [SerializeField] string m_ButtonElementId;  // 按钮元素 ID
        [SerializeField] LocalizedString m_ButtonLabel;  // 按钮标签
        [SerializeField] string m_TargetURL;  // 目标 URL

        [Tooltip("UXML 中按钮元素的唯一标识符")]
        public string ButtonElementId
        {
            get => m_ButtonElementId;
            set => m_ButtonElementId = value;
        }

        [CreateProperty]
        [Tooltip("按钮的文本标签")]
        public LocalizedString ButtonLabel
        {
            get => m_ButtonLabel;
            set => m_ButtonLabel = value;
        }

        [CreateProperty]
        [Tooltip("在浏览器中打开的 URL")]
        public string TargetURL
        {
            get => m_TargetURL;
            set => m_TargetURL = value;

        }
    }
}