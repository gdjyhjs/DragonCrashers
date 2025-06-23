using UnityEngine.Events;

namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph输出事件处理组件，用于将VFX事件转换为UnityEvent
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VisualEffect))]
    class VFXOutputEventUnityEvent : VFXOutputEventAbstractHandler
    {
        // 指示是否可在编辑器模式下执行（此处禁用）
        public override bool canExecuteInEditor => false;

        // 当VFX触发事件时调用的UnityEvent
        public UnityEvent onEvent;

        /// <summary>
        /// 当Visual Effect Graph触发输出事件时调用
        /// </summary>
        /// <param name="eventAttribute">事件属性数据</param>
        public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
        {
            // 触发UnityEvent
            onEvent?.Invoke();
        }
    }
}