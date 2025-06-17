using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace UIToolkitDemo
{
    // ����Ļ�ķ�UI�߼�
    public class HomeScreenController : MonoBehaviour
    {

        [Header("�ؿ�����")]
        [Tooltip("��ScriptableObject�����й���Ϸ�ؿ�����Ϣ������ؿ����������ƺͱ���ͼ��")]
        [SerializeField] LevelSO m_LevelData;

        [Header("��������")]
        [SerializeField] string m_ChatResourcePath = "��Ϸ����/����";

        [Tooltip("��ScriptableObject�б�洢��ʾ������Ϣ")]
        [SerializeField] List<ChatSO> m_ChatData = new List<ChatSO>();

        void Awake()
        {
            m_ChatData.AddRange(Resources.LoadAll<ChatSO>(m_ChatResourcePath));
        }

        void OnEnable()
        {
            HomeEvents.PlayButtonClicked += OnPlayGameLevel;
        }

        void OnDisable()
        {
            HomeEvents.PlayButtonClicked -= OnPlayGameLevel;
        }

        void Start()
        {
            HomeEvents.LevelInfoShown?.Invoke(m_LevelData);
            HomeEvents.ChatWindowShown?.Invoke(m_ChatData);
        }

        /// <summary>
        /// ��ʼ��Ϸ�ؿ���
        /// </summary>
        public void OnPlayGameLevel()
        {
            // û�йؿ�������ִ���κβ���
            if (m_LevelData == null)
                return;

            // ֪ͨ���˵��˳�
            HomeEvents.MainMenuExited?.Invoke();

            // �����Unity�༭�������������У�����عؿ�����
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                SceneManager.LoadSceneAsync(m_LevelData.SceneName);
        }
    }
}