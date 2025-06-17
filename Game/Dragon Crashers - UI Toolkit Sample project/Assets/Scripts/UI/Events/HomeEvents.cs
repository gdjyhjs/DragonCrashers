using System.Collections.Generic;
using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// 与HomeScreen/HomeScreenController相关的公共静态委托。
    ///
    /// 注意：从概念上讲，这些是“事件”，而非严格意义上的C#事件。
    /// </summary>
    public static class HomeEvents
    {
        // 主界面出现时显示欢迎消息
        public static Action<string> HomeMessageShown;

        // 显示关卡信息的事件
        public static Action<LevelSO> LevelInfoShown;

        // 更新/显示聊天窗口内容的事件
        public static Action<List<ChatSO>> ChatWindowShown;

        // 退出主菜单时触发的事件
        public static Action MainMenuExited;

        // 点击开始按钮时触发的事件
        public static Action PlayButtonClicked;
    }
}