using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// 动画触发器，用于控制Unity动画组件的播放
    /// </summary>
    public class StartAnimation : MonoBehaviour
    {
        // 目标动画组件
        public Animation Animation;

        /// <summary>
        /// 触发动画播放的公共方法
        /// </summary>
        public void Trigger()
        {
            // 播放动画组件中的默认动画
            Animation.Play();
        }
    }
}