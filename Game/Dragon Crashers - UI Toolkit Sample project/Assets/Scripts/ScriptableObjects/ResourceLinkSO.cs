using UnityEngine;
using Unity.Properties;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    /// <summary>
    /// �˽ű�������һ��Ψһ��ʶ�����ı���ǩ�͹����� URL������ UI Toolkit��
    /// �������ڲ�Ӱ�� UXML �ļ�����ؽű�������¸��� URL ��Ԫ�ء�
    /// </summary>
    [CreateAssetMenu(fileName = "ResourceLinkData", menuName = "UIToolkitDemo/Resource Link")]
    public class ResourceLinkSO : ScriptableObject
    {
        // ���Ա�ʶ�������ڸ���֪ͨ
        // public static readonly BindingId buttonLabelProperty = nameof(buttonLabel);
        public static readonly BindingId TargetURLProperty = nameof(TargetURL);

        // ���ֶ�
        [SerializeField] string m_ButtonElementId;  // ��ťԪ�� ID
        [SerializeField] LocalizedString m_ButtonLabel;  // ��ť��ǩ
        [SerializeField] string m_TargetURL;  // Ŀ�� URL

        [Tooltip("UXML �а�ťԪ�ص�Ψһ��ʶ��")]
        public string ButtonElementId
        {
            get => m_ButtonElementId;
            set => m_ButtonElementId = value;
        }

        [CreateProperty]
        [Tooltip("��ť���ı���ǩ")]
        public LocalizedString ButtonLabel
        {
            get => m_ButtonLabel;
            set => m_ButtonLabel = value;
        }

        [CreateProperty]
        [Tooltip("��������д򿪵� URL")]
        public string TargetURL
        {
            get => m_TargetURL;
            set => m_TargetURL = value;

        }
    }
}