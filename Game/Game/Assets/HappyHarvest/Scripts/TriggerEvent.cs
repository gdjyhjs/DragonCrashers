using System;
using UnityEngine;
using UnityEngine.Events;
namespace HappyHarvest
{
    /// <summary>
    /// 触发器事件组件，用于在碰撞体进入或离开触发器时触发 Unity 事件
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class TriggerEvent : MonoBehaviour
    {
        // 进入触发器时触发的事件
        public UnityEvent OnEnter;
        // 离开触发器时触发的事件
        public UnityEvent OnExit;

        /// <summary>
        /// 当碰撞体进入触发器时调用 OnEnter 事件
        /// </summary>
        private void OnTriggerEnter2D(Collider2D col)
        {
            OnEnter.Invoke();
        }

        /// <summary>
        /// 当碰撞体离开触发器时调用 OnExit 事件
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            OnExit.Invoke();
        }
    }
}
