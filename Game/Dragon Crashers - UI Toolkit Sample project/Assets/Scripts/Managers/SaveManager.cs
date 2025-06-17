using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace UIToolkitDemo
{
    // ע�⣺�˴����Ϊ��ʾĿ��ʹ�� JsonUtility�������������У�����ʹ�ø���Ч�Ľ���������� MessagePack (https://msgpack.org/index.html) 
    // �� Protocol Buffers (https://developers.google.com/protocol-buffers)
    // 

    [RequireComponent(typeof(GameDataManager))]
    public class SaveManager : MonoBehaviour
    {
        // ����Ϸ���ݼ������ʱ�������¼�
        public static event Action<GameData> GameDataLoaded;

        [Tooltip("���ڱ�����Ϸ���������ݵ��ļ���")]
        [SerializeField] string m_SaveFilename = "savegame.dat";
        [Tooltip("��ʾ������Ϣ��")]
        [SerializeField] bool m_Debug;

        GameDataManager m_GameDataManager;

        // �ڽű�ʵ��������ʱ����
        void Awake()
        {
            m_GameDataManager = GetComponent<GameDataManager>();
        }

        // ��Ӧ�ó����˳�ʱ����
        void OnApplicationQuit()
        {
            // ������Ϸ����
            SaveGame();
        }

        // ���ű�����ʱ��ע���¼�������
        void OnEnable()
        {
            SettingsEvents.SettingsShown += OnSettingsShown;
            SettingsEvents.SettingsUpdated += OnSettingsUpdated;

            GameplayEvents.SettingsUpdated += OnSettingsUpdated;

        }

        // ���ű�����ʱ���Ƴ��¼�������
        void OnDisable()
        {
            SettingsEvents.SettingsShown -= OnSettingsShown;
            SettingsEvents.SettingsUpdated -= OnSettingsUpdated;

            GameplayEvents.SettingsUpdated -= OnSettingsUpdated;

        }

        // �����µ���Ϸ����
        public GameData NewGame()
        {
            return new GameData();
        }

        // ������Ϸ����
        public void LoadGame()
        {
            // ���ļ����ݴ�������м��ر��������

            if (m_GameDataManager.GameData == null)
            {
                if (m_Debug)
                {
                    // ��¼��ʼ����Ϸ���ݵĵ�����Ϣ
                    Debug.Log("��Ϸ���ݹ����� LoadGame: ���ڳ�ʼ����Ϸ���ݡ�");
                }

                m_GameDataManager.GameData = NewGame();
            }
            else if (FileManager.LoadFromFile(m_SaveFilename, out var jsonString))
            {
                m_GameDataManager.GameData.LoadJson(jsonString);

                if (m_Debug)
                {
                    // ��¼���ص� JSON �ַ����ĵ�����Ϣ
                    Debug.Log("���������.LoadGame: " + m_SaveFilename + " JSON �ַ���: " + jsonString);
                }
            }

            // ֪ͨ������Ϸ���� 
            if (m_GameDataManager.GameData != null)
            {
                GameDataLoaded?.Invoke(m_GameDataManager.GameData);
            }
        }

        // ������Ϸ����
        public void SaveGame()
        {
            string jsonFile = m_GameDataManager.GameData.ToJson();

            // ʹ���ļ����ݴ���������ݱ��浽����
            if (FileManager.WriteToFile(m_SaveFilename, jsonFile) && m_Debug)
            {
                // ��¼����� JSON �ַ����ĵ�����Ϣ
                Debug.Log("���������.SaveGame: " + m_SaveFilename + " JSON �ַ���: " + jsonFile);
            }
        }

        // ���ر������Ϸ���ݲ���ʾ��������Ļ��
        void OnSettingsShown()
        {
            if (m_GameDataManager.GameData != null)
            {
                GameDataLoaded?.Invoke(m_GameDataManager.GameData);
            }
        }

        // ������Ϸ���ݹ����������ݲ�����
        void OnSettingsUpdated(GameData gameData)
        {
            m_GameDataManager.GameData = gameData;
            SaveGame();
        }
    }
}