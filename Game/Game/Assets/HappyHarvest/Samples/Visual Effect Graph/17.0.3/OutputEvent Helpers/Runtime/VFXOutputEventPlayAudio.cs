#if VFX_OUTPUTEVENT_AUDIO
using UnityEngine.Events;
namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph 输出事件音频播放器，用于在 VFX 事件触发时播放音频
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(VisualEffect))]
    class VFXOutputEventPlayAudio : VFXOutputEventAbstractHandler
    {
        // 允许在编辑器模式下执行
        public override bool canExecuteInEditor => true;

        // 要播放音频的 AudioSource 组件
        public AudioSource audioSource;

        /// <summary>
        /// 当 Visual Effect Graph 触发输出事件时调用，播放音频
        /// </summary>
        /// <param name="eventAttribute">事件属性数据</param>
        public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
        {
            if (audioSource != null)
                audioSource.Play();
        }
    }
}
#endif