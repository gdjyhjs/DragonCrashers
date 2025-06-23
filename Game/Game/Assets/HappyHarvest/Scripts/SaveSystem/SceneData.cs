using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// 每个场景必需的组件。用于定义场景的唯一名称，保存系统使用该名称标识场景。
    /// 这意味着场景可以移动、重命名或更改其构建 ID，而不会破坏保存数据。
    /// </summary>
    public class SceneData : MonoBehaviour
    {
        // 场景的唯一名称（用于保存系统标识场景）
        public string UniqueSceneName;

        private void OnEnable()
        {
            // 当组件启用时，将当前场景数据赋值给游戏管理器
            GameManager.Instance.LoadedSceneData = this;
        }

        private void OnDisable()
        {
            // 当组件禁用时，从游戏管理器中移除当前场景数据引用
            if (GameManager.Instance?.LoadedSceneData == this)
                GameManager.Instance.LoadedSceneData = null;
        }
    }
}