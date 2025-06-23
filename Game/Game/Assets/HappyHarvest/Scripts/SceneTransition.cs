using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace HappyHarvest
{
    /// <summary>
    /// 场景过渡触发器，用于实现场景切换功能
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class SceneTransition : MonoBehaviour
    {
        // 目标场景的构建索引
        public string TargetSceneBuildIndex = "World";
        // 目标场景中的生成点索引
        public int TargetSpawnIndex;

        /// <summary>
        /// 当碰撞体进入触发器时触发场景过渡
        /// </summary>
        private void OnTriggerEnter2D(Collider2D col)
        {
            // 调用 GameManager 的场景切换方法
            GameManager.Instance.MoveTo(TargetSceneBuildIndex, TargetSpawnIndex);
        }
    }
}