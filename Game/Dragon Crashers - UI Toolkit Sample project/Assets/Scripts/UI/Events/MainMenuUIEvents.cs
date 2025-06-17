using System;

namespace UIToolkitDemo
{
    /// <summary>
    /// 用于管理主菜单UI更改的公共静态委托。
    ///
    /// 注意：从概念上讲，这些是“事件”，而非严格意义上的C#事件。
    /// </summary>
    public static class MainMenuUIEvents
    {
        // 显示主界面以开始游戏
        public static Action HomeScreenShown;

        // 显示角色界面以选择角色和装备
        public static Action CharScreenShown;

        // 显示信息界面，包含资源链接
        public static Action InfoScreenShown;

        // 显示商店界面以购买金币/宝石/药剂
        public static Action ShopScreenShown;

        // 从选项栏显示商店界面
        public static Action OptionsBarShopScreenShown;

        // 显示邮件界面
        public static Action MailScreenShown;

        // 显示设置界面覆盖层
        public static Action SettingsScreenShown;

        // 显示库存界面
        public static Action InventoryScreenShown;

        // 隐藏设置界面
        public static Action SettingsScreenHidden;

        // 隐藏库存界面
        public static Action InventoryScreenHidden;

        // 显示游戏界面以进行游戏
        public static Action GameScreenShown;

        // 显示新的菜单界面时触发
        public static Action<MenuScreen> CurrentScreenChanged;

        // 当前视图更改时触发
        public static Action<string> CurrentViewChanged;

        // 通知标签式菜单重置/选择第一个标签
        public static Action<string> TabbedUIReset;
    }
}