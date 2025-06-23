using System;
using System.Collections;
using System.Collections.Generic;
using HappyHarvest;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 允许锁定和解锁玩家控制器的操作。动画事件使用此功能在例如工具动画期间停止移动。
    /// 此组件应添加到包含动画控制器的同一对象上，以便接收动画事件。
    /// </summary>
    public class CharacterAnimationEventHandler : MonoBehaviour
    {
        // 玩家控制器引用
        private PlayerController m_Controller;

        private void Awake()
        {
            // 从父对象中获取控制器（因为此组件位于带有动画器的游戏对象上，而控制器位于角色根对象）
            m_Controller = GetComponentInParent<PlayerController>();
        }

        /// <summary>
        /// 锁定玩家控制（通过动画事件调用）
        /// </summary>
        void LockControl()
        {
            m_Controller.ToggleControl(false);
        }

        /// <summary>
        /// 解锁玩家控制（通过动画事件调用）
        /// </summary>
        void UnlockControl()
        {
            m_Controller.ToggleControl(true);
        }
    }
}