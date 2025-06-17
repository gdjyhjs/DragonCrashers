using System;
using UnityEngine.UIElements;

namespace UIToolkitDemo
{
    public static class GameplayEvents
    {
        // 战斗胜利后触发
        public static Action WinScreenShown;

        // 战斗失败后触发
        public static Action LoseScreenShown;

        // 角色死亡时触发
        public static Action<UnitController> CharacterCardHidden;

        // 设置更新时触发
        public static Action<GameData> SettingsUpdated;

        // 使用游戏数据加载音乐和音效音量级别
        public static Action SettingsLoaded;

        // 通知监听器在指定秒数后暂停游戏
        public static Action<float> GamePaused;

        // 从暂停界面恢复游戏
        public static Action GameResumed;

        // 从暂停界面退出游戏
        public static Action GameQuit;

        // 从暂停界面重新开始游戏
        public static Action GameRestarted;

        // 在游戏过程中调整音乐音量
        public static Action<float> MusicVolumeChanged;

        // 在游戏过程中调整音效音量
        public static Action<float> SfxVolumeChanged;

        // 将治疗药剂拖放到特定的治疗槽VisualElement上
        public static Action<VisualElement> SlotHealed;

        // 更新治疗药剂的数量
        public static Action<int> HealingPotionUpdated;

    }
}