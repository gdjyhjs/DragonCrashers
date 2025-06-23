using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace HappyHarvest
{
    /// <summary>
    /// GameManager是所有游戏系统的入口点。它的执行顺序设置得非常低，以确保
    /// 其Awake函数尽可能早地被调用，从而保证在其他脚本中实例是有效的。
    /// </summary>
    [DefaultExecutionOrder(-9999)]
    public class GameManager : MonoBehaviour
    {
        private static GameManager s_Instance;


#if UNITY_EDITOR
        //由于我们的管理器首先运行，当应用程序退出时它也会首先被销毁，这会导致s_Instance
        //变为null，因此在编辑模式下会触发另一个实例化（因为我们动态实例化管理器）
        //所以在销毁时将其设置为true，这样我们就不会重新实例化一个新的管理器
        private static bool s_IsQuitting = false;
#endif
        public static GameManager Instance
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying || s_IsQuitting)
                    return null;

                if (s_Instance == null)
                {
                    //在编辑器中，我们可以启动任何场景进行测试，因此我们不确定游戏管理器是否会被
                    //启动游戏的第一个场景创建。所以我们手动加载它。这个检查在
                    //玩家构建中是无用的，因为第一个场景会创建GameManager，所以它将始终存在。
                    Instantiate(Resources.Load<GameManager>("GameManager"));
                }
#endif
                return s_Instance;
            }
        }

        public TerrainManager Terrain { get; set; }
        public PlayerController Player { get; set; }
        public DayCycleHandler DayCycleHandler { get; set; }
        public WeatherSystem WeatherSystem { get; set; }
        public CinemachineCamera MainCamera { get; set; }
        public Tilemap WalkSurfaceTilemap { get; set; }

        public SceneData LoadedSceneData { get; set; }

        // 将返回当前天的时间比例，范围从0（00:00）到1（23:59）。
        public float CurrentDayRatio => m_CurrentTimeOfTheDay / DayDurationInSeconds;

        [Header("市场")]
        public Item[] MarketEntries;

        [Header("时间设置")]
        [Min(1.0f)]
        public float DayDurationInSeconds;
        public float StartingTime = 0.0f;

        [Header("数据")]
        public ItemDatabase ItemDatabase;
        public CropDatabase CropDatabase;

        public Storage Storage;

        private bool m_IsTicking;

        private List<DayEventHandler> m_EventHandlers = new();
        private List<SpawnPoint> m_ActiveTransitions = new List<SpawnPoint>();

        private float m_CurrentTimeOfTheDay;

        private void Awake()
        {
            s_Instance = this;
            DontDestroyOnLoad(gameObject);

            m_IsTicking = true;

            ItemDatabase.Init();
            CropDatabase.Init();

            Storage = new Storage();

            m_CurrentTimeOfTheDay = StartingTime;

            //我们需要确保日长不为0，否则我们将在更新中陷入无限循环
            //（日长为0是没有意义的）
            if (DayDurationInSeconds <= 0.0f)
            {
                DayDurationInSeconds = 1.0f;
                Debug.LogError("GameManager上的日长设置为0，日长需要设置为正值");
            }
        }

        private void Start()
        {
            m_CurrentTimeOfTheDay = StartingTime;

            UIHandler.SceneLoaded();
        }

#if UNITY_EDITOR
        private void OnDestroy()
        {
            s_IsQuitting = true;
        }
#endif

        private void Update()
        {
            if (m_IsTicking)
            {
                float previousRatio = CurrentDayRatio;
                m_CurrentTimeOfTheDay += Time.deltaTime;

                while (m_CurrentTimeOfTheDay > DayDurationInSeconds)
                    m_CurrentTimeOfTheDay -= DayDurationInSeconds;

                foreach (var handler in m_EventHandlers)
                {
                    foreach (var evt in handler.Events)
                    {
                        bool prev = evt.IsInRange(previousRatio);
                        bool current = evt.IsInRange(CurrentDayRatio);

                        if (prev && !current)
                        {
                            evt.OffEvent.Invoke();
                        }
                        else if (!prev && current)
                        {
                            evt.OnEvents.Invoke();
                        }
                    }
                }

                if (DayCycleHandler != null)
                    DayCycleHandler.Tick();
            }
        }

        public void Pause()
        {
            m_IsTicking = false;
            Player.ToggleControl(false);
        }

        public void Resume()
        {
            m_IsTicking = true;
            Player.ToggleControl(true);
        }

        public void RegisterSpawn(SpawnPoint spawn)
        {
            if (Player == null && spawn.SpawnIndex == 0)
            { //如果我们没有玩家，我们需要创建一个
                Instantiate(Resources.Load<PlayerController>("Character"));
                spawn.SpawnHere();
            }

            m_ActiveTransitions.Add(spawn);
        }

        public void UnregisterSpawn(SpawnPoint spawn)
        {
            m_ActiveTransitions.Remove(spawn);
        }

        public void MoveTo(string targetScene, int targetSpawn)
        {
            Pause();
            SaveSystem.SaveSceneData();
            UIHandler.FadeToBlack(() =>
            {
                var asyncop = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
                asyncop.completed += operation =>
                {
                    m_IsTicking = true;

                    foreach (var active in m_ActiveTransitions)
                    {
                        if (active.SpawnIndex == targetSpawn)
                        {
                            active.SpawnHere();
                            SaveSystem.LoadSceneData();
                        }
                    }

                    UIHandler.SceneLoaded();
                    UIHandler.FadeFromBlack(() =>
                    {
                        Player.ToggleControl(true);
                    });
                };
            });


        }

        /// <summary>
        /// 将以"xx:xx"格式返回当前时间的字符串
        /// </summary>
        /// <returns></returns>
        public string CurrentTimeAsString()
        {
            return GetTimeAsString(CurrentDayRatio);
        }

        /// <summary>
        /// 以"xx:xx"格式返回给定时间比例（0到1之间）的时间
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public static string GetTimeAsString(float ratio)
        {
            var hour = GetHourFromRatio(ratio);
            var minute = GetMinuteFromRatio(ratio);

            return $"{hour}:{minute:00}";
        }


        public static int GetHourFromRatio(float ratio)
        {
            var time = ratio * 24.0f;
            var hour = Mathf.FloorToInt(time);

            return hour;
        }

        public static int GetMinuteFromRatio(float ratio)
        {
            var time = ratio * 24.0f;
            var minute = Mathf.FloorToInt((time - Mathf.FloorToInt(time)) * 60.0f);

            return minute;
        }

        public static void RegisterEventHandler(DayEventHandler handler)
        {
            foreach (var evt in handler.Events)
            {
                if (evt.IsInRange(GameManager.Instance.CurrentDayRatio))
                {
                    evt.OnEvents.Invoke();
                }
                else
                {
                    evt.OffEvent.Invoke();
                }
            }

            Instance.m_EventHandlers.Add(handler);
        }

        public static void RemoveEventHandler(DayEventHandler handler)
        {
            Instance?.m_EventHandlers.Remove(handler);
        }
    }
}