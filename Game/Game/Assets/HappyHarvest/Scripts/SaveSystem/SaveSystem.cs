using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace HappyHarvest
{
    /// <summary>
    /// 游戏保存系统，负责游戏数据的序列化、保存和加载
    /// </summary>
    public class SaveSystem
    {
        // 当前保存数据
        private static SaveData s_CurrentData = new SaveData();

        /// <summary>
        /// 游戏保存数据结构，包含玩家数据、时间数据和场景数据
        /// </summary>
        [System.Serializable]
        public struct SaveData
        {
            public PlayerSaveData PlayerData; // 玩家数据
            public DayCycleHandlerSaveData TimeSaveData; // 昼夜循环数据
            public SaveData[] ScenesData; // 场景数据数组
        }

        /// <summary>
        /// 场景保存数据结构，包含场景名称和地形数据
        /// </summary>
        [System.Serializable]
        public struct SceneData
        {
            public string SceneName; // 场景名称
            public TerrainDataSave TerrainData; // 地形数据
        }

        // 场景数据查找字典，用于快速访问场景数据
        private static Dictionary<string, SceneData> s_ScenesDataLookup = new Dictionary<string, SceneData>();

        /// <summary>
        /// 保存游戏数据到文件
        /// </summary>
        public static void Save()
        {
            // 保存玩家数据和昼夜循环数据
            GameManager.Instance.Player.Save(ref s_CurrentData.PlayerData);
            GameManager.Instance.DayCycleHandler.Save(ref s_CurrentData.TimeSaveData);

            // 保存文件路径
            string savefile = Application.persistentDataPath + "/save.sav";
            // 将数据序列化为 JSON 并写入文件
            File.WriteAllText(savefile, JsonUtility.ToJson(s_CurrentData));
        }

        /// <summary>
        /// 从文件加载游戏数据
        /// </summary>
        public static void Load()
        {
            // 保存文件路径
            string savefile = Application.persistentDataPath + "/save.sav";
            // 读取文件内容
            string content = File.ReadAllText(savefile);

            // 从 JSON 反序列化保存数据
            s_CurrentData = JsonUtility.FromJson<SaveData>(content);

            // 注册场景加载完成事件
            SceneManager.sceneLoaded += SceneLoaded;
            // 重新加载当前场景
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

        /// <summary>
        /// 场景加载完成回调函数
        /// </summary>
        static void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // 加载玩家数据和昼夜循环数据
            GameManager.Instance.Player.Load(s_CurrentData.PlayerData);
            GameManager.Instance.DayCycleHandler.Load(s_CurrentData.TimeSaveData);
            //GameManager.Instance.Terrain.Load (s_CurrentData.TerrainData);

            // 注销场景加载完成事件
            SceneManager.sceneLoaded -= SceneLoaded;
        }

        /// <summary>
        /// 保存当前场景数据
        /// </summary>
        public static void SaveSceneData()
        {
            if (GameManager.Instance.Terrain != null)
            {
                // 获取当前场景名称
                var sceneName = GameManager.Instance.LoadedSceneData.UniqueSceneName;
                var data = new TerrainDataSave();
                // 保存地形数据
                GameManager.Instance.Terrain.Save(ref data);

                // 将场景数据添加到查找字典
                s_ScenesDataLookup[sceneName] = new SceneData()
                {
                    SceneName = sceneName,
                    TerrainData = data
                };
            }
        }

        /// <summary>
        /// 加载当前场景数据
        /// </summary>
        public static void LoadSceneData()
        {
            // 从查找字典获取当前场景数据
            if (s_ScenesDataLookup.TryGetValue(GameManager.Instance.LoadedSceneData.UniqueSceneName, out var data))
            {
                // 加载地形数据
                GameManager.Instance.Terrain.Load(data.TerrainData);
            }
        }
    }
}
