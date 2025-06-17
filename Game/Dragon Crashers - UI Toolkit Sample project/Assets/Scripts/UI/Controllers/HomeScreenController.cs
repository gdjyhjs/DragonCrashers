using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace UIToolkitDemo
{
    // 主屏幕的非UI逻辑
    public class HomeScreenController : MonoBehaviour
    {

        [Header("关卡数据")]
        [Tooltip("此ScriptableObject包含有关游戏关卡的信息（例如关卡索引、名称和背景图像）")]
        [SerializeField] LevelSO m_LevelData;

        [Header("聊天数据")]
        [SerializeField] string m_ChatResourcePath = "游戏数据/聊天";

        [Tooltip("此ScriptableObject列表存储演示聊天消息")]
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
        /// 开始游戏关卡。
        /// </summary>
        public void OnPlayGameLevel()
        {
            // 没有关卡数据则不执行任何操作
            if (m_LevelData == null)
                return;

            // 通知主菜单退出
            HomeEvents.MainMenuExited?.Invoke();

            // 如果在Unity编辑器中且正在运行，则加载关卡场景
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                SceneManager.LoadSceneAsync(m_LevelData.SceneName);
        }
    }
}