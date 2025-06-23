using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 将此组件添加到场景中作为主游戏摄像机的 CinemachineVirtualCamera 上。
    /// 场景加载时，玩家将通过此脚本将自身设置为摄像机的目标
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class CameraSetter : MonoBehaviour
    {
        private void Awake()
        {
            // 获取 Cinemachine 摄像机组件
            var cam = GetComponent<CinemachineCamera>();
            // 将当前摄像机设置为 GameManager 中的主摄像机
            GameManager.Instance.MainCamera = cam;
        }
    }
}