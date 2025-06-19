using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Properties;

namespace UIToolkitDemo
{
    /// <summary>
    /// InfoView����һ�����ӵ��ⲿ��Դ�İ�ť��ʹ��"GameData/Links"�е�ScriptableObjects��Inspector�ж��尴ť��ϸ��Ϣ��Ԫ�����ơ���ǩ��URL����
    /// ������ǳ���Ա�������ʦ���ڲ����Ĵ����������޸���Щֵ��
    /// </summary>
    public class InfoView : UIView
    {
        // ��ť�б�
        List<Button> m_Buttons;
        // ��Դ�����б�
        List<ResourceLinkSO> m_ResourceLinks;

        // ��Դ·��
        const string k_ResourcePath = "GameData/Links";

        /// <summary>
        /// InfoView�Ĺ��캯����������Դ�������ݲ���̬���ð�ť��
        /// </summary>
        /// <param name="topElement">��VisualElement��</param>
        public InfoView(VisualElement topElement) : base(topElement)
        {
            // ������Դ��������
            m_ResourceLinks = Resources.LoadAll<ResourceLinkSO>(k_ResourcePath).ToList();
            m_Buttons = new List<Button>();

            // ʹ�õ����ķ������ð�ť����Ϊ�����SetVisualElements��RegisterButtonCallbacks�Ѿ�����
            SetupButtons();
        }

        // ע�⣺ע����ť�ص��ǿ�ѡ�ģ������������ʡ�ԡ���Ҫʱʹ��UnregisterCallback��UnregisterValueChangedCallback����ע���ص���

        /// <summary>
        /// ͨ�������ı���ǩ��ע�����¼������ð�ť
        /// </summary>
        void SetupButtons()
        {
            // Ϊÿ����ťע��URL
            for (int i = 0; i < m_ResourceLinks.Count; i++)
            {
                // ���������Ա���lambda���ʽ�еıհ�
                int index = i;

                // ʹ��ResourceLink�е�Ԫ��ID���Ӿ����ж�λ��ť
                m_Buttons.Add(m_TopElement.Q<Button>(m_ResourceLinks[index].ButtonElementId));

                // ����ҵ���ťԪ�أ����������ǩ�͵���¼�
                if (m_Buttons[index] != null)
                {
                    // �󶨰�ť��ǩ
                    BindButtonLabel(m_Buttons[index], m_ResourceLinks[index]);
                    // �󶨰�ťURL
                    BindURL(m_Buttons[index], m_ResourceLinks[index]);
                }
            }
        }

        /// <summary>
        /// ����ť�ı�ǩ��LocalizedText���ԣ��󶨵�ResourceLink��ButtonText���ԡ�
        /// </summary>
        /// <param name="button">Ҫ�󶨱�ǩ�İ�ť��</param>
        /// <param name="resourceLink">Ҫ�󶨱�ǩ��ResourceLinkSO��</param>
        void BindButtonLabel(Button button, ResourceLinkSO resourceLink)
        {
            if (resourceLink.ButtonLabel != null)
            {
                // ����LocalizedString��Ϊ�󶨣�����������£�����Դ����ʽ�ģ���ΪLocalizedString�����������Դ
                button.SetBinding("text", resourceLink.ButtonLabel);
            }
        }

        /// <summary>
        /// ����ť�ĵ���¼��󶨵�ResourceLinkSO��TargetURL���ԡ�����Inspector�н��и���ʱ���°�ť��clickEventĿ��URL��
        /// </summary>
        /// <param name="button">�����¼������󶨵İ�ť��</param>
        /// <param name="resourceLink">�ṩURL���ݵ�ResourceLinkSO��</param>
        void BindURL(Button button, ResourceLinkSO resourceLink)
        {
            var dataBinding = new DataBinding
            {
                // Ҫ�󶨵Ķ���
                dataSource = resourceLink,
                // �����ڵ��ض�����
                dataSourcePath = new PropertyPath(nameof(resourceLink.TargetURL)),
                // ������Դ��UI�ĵ����
                bindingMode = BindingMode.ToTarget
            };

            // ȷ��ֻע������ĵ���¼�����ʹ�ø��º��TargetURL
            button.UnregisterCallback<ClickEvent>(evt => OpenURL(resourceLink.TargetURL));
            button.RegisterCallback<ClickEvent>(evt => OpenURL(resourceLink.TargetURL));

            // ����Ŀ�갴ť������Դ֮��İ�
            button.SetBinding("clickEvent", dataBinding);
        }

        /// <summary>
        /// ��Ĭ��������д�ָ����URL�����ŵ����Ч��
        /// </summary>
        /// <param name="URL">Ҫ�򿪵�URL��</param>
        static void OpenURL(string URL)
        {
            // ����Ĭ�ϰ�ť��Ч
            AudioManager.PlayDefaultButtonSound();
            // ��URL
            Application.OpenURL("https://www.yellowshange.com");
        }
    }
}