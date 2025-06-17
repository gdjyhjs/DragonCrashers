using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{
    // 注意：此代码仅为演示目的使用 JsonUtility；在生产环境中，考虑使用更高效的解决方案，如 MessagePack (https://msgpack.org/index.html) 
    // 或 Protocol Buffers (https://developers.google.com/protocol-buffers)
    // 

    [RequireComponent(typeof(GameDataManager))]
    public class SaveManager : MonoBehaviour
    {
        // 当游戏数据加载完成时触发的事件
        public static event Action<GameData> GameDataLoaded;

        [Tooltip("用于保存游戏和设置数据的文件名")]
        [SerializeField] string m_SaveFilename = "savegame.dat";
        [Tooltip("显示调试信息。")]
        [SerializeField] bool m_Debug;

        GameDataManager m_GameDataManager;

        // 在脚本实例被唤醒时调用
        void Awake()
        {
            m_GameDataManager = GetComponent<GameDataManager>();
        }

        // 当应用程序退出时调用
        void OnApplicationQuit()
        {
            // 保存游戏数据
            SaveGame();
        }

        // 当脚本启用时，注册事件监听器
        void OnEnable()
        {
            SettingsEvents.SettingsShown += OnSettingsShown;
            SettingsEvents.SettingsUpdated += OnSettingsUpdated;

            GameplayEvents.SettingsUpdated += OnSettingsUpdated;

        }

        // 当脚本禁用时，移除事件监听器
        void OnDisable()
        {
            SettingsEvents.SettingsShown -= OnSettingsShown;
            SettingsEvents.SettingsUpdated -= OnSettingsUpdated;

            GameplayEvents.SettingsUpdated -= OnSettingsUpdated;

        }

        // 创建新的游戏数据
        public GameData NewGame()
        {
            return new GameData();
        }

        // 加载游戏数据
        public void LoadGame()
        {
            // 从文件数据处理程序中加载保存的数据

            if (m_GameDataManager.GameData == null)
            {
                if (m_Debug)
                {
                    // 记录初始化游戏数据的调试信息
                    Debug.Log("游戏数据管理器 LoadGame: 正在初始化游戏数据。");
                }

                m_GameDataManager.GameData = NewGame();
            }
            else if (FileManager.LoadFromFile(m_SaveFilename, out var jsonString))
            {
                m_GameDataManager.GameData.LoadJson(jsonString);

                if (m_Debug)
                {
                    // 记录加载的 JSON 字符串的调试信息
                    Debug.Log("保存管理器.LoadGame: " + m_SaveFilename + " JSON 字符串: " + jsonString);
                }
            }

            // 通知其他游戏对象 
            if (m_GameDataManager.GameData != null)
            {
                GameDataLoaded?.Invoke(m_GameDataManager.GameData);
            }
        }

        // 保存游戏数据
        public void SaveGame()
        {
            string jsonFile = m_GameDataManager.GameData.ToJson();

            // 使用文件数据处理程序将数据保存到磁盘
            if (FileManager.WriteToFile(m_SaveFilename, jsonFile) && m_Debug)
            {
                // 记录保存的 JSON 字符串的调试信息
                Debug.Log("保存管理器.SaveGame: " + m_SaveFilename + " JSON 字符串: " + jsonFile);
            }
        }

        // 加载保存的游戏数据并显示在设置屏幕上
        void OnSettingsShown()
        {
            if (m_GameDataManager.GameData != null)
            {
                GameDataLoaded?.Invoke(m_GameDataManager.GameData);
            }
        }

        // 更新游戏数据管理器的数据并保存
        void OnSettingsUpdated(GameData gameData)
        {
            m_GameDataManager.GameData = gameData;
            SaveGame();
        }
    }
}