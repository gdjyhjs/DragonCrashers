using System;
using UnityEngine;

namespace UIToolkitDemo
{
    /// <summary>
    /// 与更改屏幕分辨率和宽高比相关的公共静态委托。
    /// 用于通知任何监听横向/纵向切换的组件。
    ///
    /// 注意：从概念上讲，这些是“事件”，而非严格意义上的C#事件。
    /// </summary>
    public class MediaQueryEvents
    {
        // 屏幕尺寸更改时触发
        public static Action<Vector2> ResolutionUpdated;

        // 宽高比更新时触发
        public static Action<MediaAspectRatio> AspectRatioUpdated;

        // 相机大小调整时触发
        public static Action CameraResized;

        // 应用安全区域时触发
        public static Action SafeAreaApplied;
    }
}