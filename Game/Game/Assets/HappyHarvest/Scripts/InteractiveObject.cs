using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// 场景中可点击交互对象的基类。实现此接口的对象可被玩家交互
    /// </summary>
    public abstract class InteractiveObject : MonoBehaviour
    {
        /// <summary>
        /// 当对象被交互时调用的方法，子类必须实现此方法
        /// </summary>
        public abstract void InteractedWith();

        /// <summary>
        /// 对象初始化时设置图层为31（交互对象层）
        /// </summary>
        protected void Awake()
        {
            // 玩家控制器通过射线检测第31层来识别可交互对象
            gameObject.layer = 31;
        }
    }
}