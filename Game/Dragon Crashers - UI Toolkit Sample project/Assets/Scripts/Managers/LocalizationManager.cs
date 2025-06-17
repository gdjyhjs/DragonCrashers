using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEditor;

namespace UIToolkitDemo
{
    /// <summary>
    /// ���ڰ��������ػ����õ��ࡣ
    /// </summary>
    public class LocalizationManager
    {
        [SerializeField] bool m_Debug;

        /// <summary>
        /// ���캯����
        /// </summary>
        /// <param name="debug">��־λ�������ڿ���̨��¼������Ϣ��</param>
        public LocalizationManager(bool debug = false)
        {
            m_Debug = debug;

#if UNITY_EDITOR
            // ��������ģʽ״̬�ĸı�
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
        }

#if UNITY_EDITOR
        // ������ģʽ״̬�ı�ʱ����
        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            // ���˳�����ģʽʱ�����ñ��ػ�ϵͳ�Է�ֹ����
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                LocalizationSettings.Instance.ResetState();
            }
        }

        /// <summary>
        /// ����������
        /// </summary>
        ~LocalizationManager()
        {
            // �Ƴ�����ģʽ״̬�ı�ļ���
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }
#endif
        // ���õ�ǰ�����Ի���
        public void SetLocale(string localeName)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;

            // ȥ����������Ի�������ǰ��Ŀհ��ַ�
            string inputLocaleName = localeName.Trim();

            foreach (var locale in locales)
            {
                // ��ȡ��ʾ���ƣ�ֱ����һ�� '(' Ϊֹ����ȥ��ǰ��Ŀհ��ַ�
                string localeDisplayName = locale.name.Split('(')[0].Trim();

                // ������Ի�������ʾ�����Ƿ�����������Ի������ƿ�ͷ�������ִ�Сд��
                if (localeDisplayName.StartsWith(inputLocaleName, StringComparison.OrdinalIgnoreCase))
                {
                    LocalizationSettings.SelectedLocale = locale;

                    if (m_Debug)
                        // ��¼���Ի����Ѹ��ĵĵ�����Ϣ
                        Debug.Log("���Ի����Ѹ���Ϊ: " + locale.name);
                    return;
                }
            }

            // ���δ�ҵ�ƥ������Ի������ҵ���ģʽ���������¼������Ϣ
            if (m_Debug)
                Debug.LogWarning("δ�ҵ�����Ϊ: " + localeName + " �����Ի���");
        }

        /// <summary>
        /// ���Ի���ʹ����ʽ���Ƽ��������ڵ�˫��ĸ���룬���� "English (en)"��
        /// �˷����ڽ��ṩ���Ի������Ƶĵ�һ����ʱ����˫��ĸ���루"English" -> "en"��
        /// </summary>
        /// <param name="localeName">Ҫת�������Ի������ơ�</param>
        /// <returns>���Ի�����˫��ĸ���룬���δ�ҵ��򷵻ؿ��ַ�����</returns>
        // �����Ի��������л�ȡ���Ի�������ķ���
        public static string GetLocaleCode(string localeName)
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;

            // ȥ�������ڵĲ��֣����磬"English (en)" -> "English"��
            string strippedLocaleName = localeName.Split('(')[0].Trim();

            foreach (var locale in locales)
            {
                string localeDisplayName = locale.name.Split('(')[0].Trim();

                if (localeDisplayName.Equals(strippedLocaleName, StringComparison.OrdinalIgnoreCase))
                {
                    return locale.Identifier.Code; // �������Ի������루���磬"en", "fr"��
                }
            }

            return string.Empty; // δ�ҵ�ƥ��ʱ���ؿ��ַ���
        }

        /// <summary>
        /// ��¼���õ����Ի����Խ��е���
        /// </summary>
        // ��¼�������Ի����ĵ�����Ϣ
        public void LogAvailableLocales()
        {
            var locales = LocalizationSettings.AvailableLocales.Locales;

            Debug.Log("���õ����Ի���:");
            foreach (var locale in locales)
            {
                Debug.Log($"���Ի�������: {locale.name}, ���Ի�������: {locale.Identifier.Code}");
            }
        }
    }
}