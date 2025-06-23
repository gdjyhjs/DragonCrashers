using System;
using System.Collections;
using System.Collections.Generic;
using HappyHarvest;
using UnityEngine;
namespace HappyHarvest
{
    // 延迟执行以确保管理器已实例化
    [DefaultExecutionOrder(999)]
    [ExecuteInEditMode]
    public class ShadowInstance : MonoBehaviour
    {
        // 基础阴影长度（范围 0-10）
        [Range(0, 10f)] public float BaseLength = 1f;

        private void OnEnable()
        {
            // 向昼夜循环处理器注册阴影实例
            DayCycleHandler.RegisterShadow(this);
        }

        private void OnDisable()
        {
            // 从昼夜循环处理器注销阴影实例
            DayCycleHandler.UnregisterShadow(this);
        }
    }
}
