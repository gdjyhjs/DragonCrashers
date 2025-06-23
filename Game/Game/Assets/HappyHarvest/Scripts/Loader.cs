using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace HappyHarvest
{
    /// <summary>
    /// 场景加载器，用于加载指定目标场景
    /// 主要用于确保在构建版本中正确创建 GameManager
    /// </summary>
    public class Loader : MonoBehaviour
    {
        // 目标场景索引
        public string TargetScene = "World";

        private void Start()
        {
            // 加载目标场景
            SceneManager.LoadScene(TargetScene);
        }
    }
}
