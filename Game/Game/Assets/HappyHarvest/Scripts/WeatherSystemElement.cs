using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// 天气系统元素组件，用于根据当前天气状态激活或禁用游戏对象
    /// 例如：雨滴特效应设置为仅在雨天或雷暴天气时启用
    /// </summary>
    [DefaultExecutionOrder(999)]
    [ExecuteInEditMode]
    public class WeatherSystemElement : MonoBehaviour
    {
        // 该元素对应的天气类型
        public WeatherSystem.WeatherType WeatherType;

        /// <summary>
        /// 对象销毁时从天气系统中注销该元素
        /// </summary>
        private void OnDestroy()
        {
            WeatherSystem.UnregisterElement(this);
        }
    }
}