using System;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// 与更改主题相关的公共静态委托。
    /// 可以通知任何监听季节性或横向/纵向主题更改的组件。
    ///
    /// 注意：从概念上讲，这些是“事件”，而非严格意义上的C#事件。
    /// </summary>
    public static class ThemeEvents
    {
        // 更改主题的事件（字符串表示主题名称）
        public static Action<string> ThemeChanged;

        // 更新主题相机时触发的事件
        public static Action<Camera> CameraUpdated;
    }
}