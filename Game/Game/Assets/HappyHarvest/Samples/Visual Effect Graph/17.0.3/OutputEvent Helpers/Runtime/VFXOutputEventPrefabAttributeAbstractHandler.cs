namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph输出事件预制体属性处理抽象基类 
    /// 用于处理VFX事件触发时预制体实例的属性设置
    /// </summary>
    public abstract class VFXOutputEventPrefabAttributeAbstractHandler : MonoBehaviour
    {
        /// <summary>
        /// 当VFX触发输出事件时调用的抽象方法
        /// 由子类实现具体的属性处理逻辑
        /// </summary>
        /// <param name="eventAttribute">VFX事件属性数据</param>
        /// <param name="visualEffect">触发事件的VisualEffect组件</param>
        public abstract void OnVFXEventAttribute(VFXEventAttribute eventAttribute, VisualEffect visualEffect);
    }
}
