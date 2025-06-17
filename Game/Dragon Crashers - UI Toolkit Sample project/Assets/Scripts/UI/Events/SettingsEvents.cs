using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// 与SettingsScreen/SettingsScreenController相关的公共静态委托。
    ///
    /// 注意：从概念上讲，这些是“事件”，而非严格意义上的C#事件。
    /// </summary>
    public static class SettingsEvents
    {
        // 重置玩家资金时触发
        public static Action PlayerFundsReset;
        // 重置玩家等级时触发
        public static Action PlayerLevelReset;

        // 设置界面显示时触发
        public static Action SettingsShown;

        // 主题被选中时触发，参数为主题名称
        public static Action<string> ThemeSelected;

        // 从SettingsScreenController同步之前保存的数据到SettingsScreen UI
        public static Action<GameData> GameDataLoaded;

        // 将更新后的游戏数据副本从UI传递给控制器
        public static Action<GameData> UIGameDataUpdated;

        // 将更新后的数据从控制器发送给监听器（例如GameDataManager、AudioManager等）
        public static Action<GameData> SettingsUpdated;

        // 切换FPS计数器时触发
        public static Action<bool> FpsCounterToggled;

        // 设置目标帧率时触发
        public static Action<int> TargetFrameRateSet;
    }
}