using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace HappyHarvest
{
    /// <summary>
    /// ��Ϸ����ϵͳ��������Ϸ���ݵ����л�������ͼ���
    /// </summary>
    public class SaveSystem
    {
        // ��ǰ��������
        private static SaveData s_CurrentData = new SaveData();

        /// <summary>
        /// ��Ϸ�������ݽṹ������������ݡ�ʱ�����ݺͳ�������
        /// </summary>
        [System.Serializable]
        public struct SaveData
        {
            public PlayerSaveData PlayerData; // �������
            public DayCycleHandlerSaveData TimeSaveData; // ��ҹѭ������
            public SaveData[] ScenesData; // ������������
        }

        /// <summary>
        /// �����������ݽṹ�������������ƺ͵�������
        /// </summary>
        [System.Serializable]
        public struct SceneData
        {
            public string SceneName; // ��������
            public TerrainDataSave TerrainData; // ��������
        }

        // �������ݲ����ֵ䣬���ڿ��ٷ��ʳ�������
        private static Dictionary<string, SceneData> s_ScenesDataLookup = new Dictionary<string, SceneData>();

        /// <summary>
        /// ������Ϸ���ݵ��ļ�
        /// </summary>
        public static void Save()
        {
            // ����������ݺ���ҹѭ������
            GameManager.Instance.Player.Save(ref s_CurrentData.PlayerData);
            GameManager.Instance.DayCycleHandler.Save(ref s_CurrentData.TimeSaveData);

            // �����ļ�·��
            string savefile = Application.persistentDataPath + "/save.sav";
            // ���������л�Ϊ JSON ��д���ļ�
            File.WriteAllText(savefile, JsonUtility.ToJson(s_CurrentData));
        }

        /// <summary>
        /// ���ļ�������Ϸ����
        /// </summary>
        public static void Load()
        {
            // �����ļ�·��
            string savefile = Application.persistentDataPath + "/save.sav";
            // ��ȡ�ļ�����
            string content = File.ReadAllText(savefile);

            // �� JSON �����л���������
            s_CurrentData = JsonUtility.FromJson<SaveData>(content);

            // ע�᳡����������¼�
            SceneManager.sceneLoaded += SceneLoaded;
            // ���¼��ص�ǰ����
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        /// <summary>
        /// ����������ɻص�����
        /// </summary>
        static void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // ����������ݺ���ҹѭ������
            GameManager.Instance.Player.Load(s_CurrentData.PlayerData);
            GameManager.Instance.DayCycleHandler.Load(s_CurrentData.TimeSaveData);
            //GameManager.Instance.Terrain.Load (s_CurrentData.TerrainData);

            // ע��������������¼�
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        /// <summary>
        /// ���浱ǰ��������
        /// </summary>
        public static void SaveSceneData()
        {
            if (GameManager.Instance.Terrain != null)
            {
                // ��ȡ��ǰ��������
                var sceneName = GameManager.Instance.LoadedSceneData.UniqueSceneName;
                var data = new TerrainDataSave();
                // �����������
                GameManager.Instance.Terrain.Save(ref data);

                // ������������ӵ������ֵ�
                s_ScenesDataLookup[sceneName] = new SceneData()
                {
                    SceneName = sceneName,
                    TerrainData = data
                };
            }
        }

        /// <summary>
        /// ���ص�ǰ��������
        /// </summary>
        public static void LoadSceneData()
        {
            // �Ӳ����ֵ��ȡ��ǰ��������
            if (s_ScenesDataLookup.TryGetValue(GameManager.Instance.LoadedSceneData.UniqueSceneName, out var data))
            {
                // ���ص�������
                GameManager.Instance.Terrain.Load(data.TerrainData);
            }
        }
    }
}
