using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HappyHarvest
{
    [DefaultExecutionOrder(1000)]
    public class SpawnPoint : MonoBehaviour
    {
        // 生成点索引（用于标识不同的生成位置）
        public int SpawnIndex;
        // 当对象启用时调用（注册到游戏管理器）
        private void OnEnable()
        {
            GameManager.Instance.RegisterSpawn(this);
        }
        // 当对象禁用时调用（从游戏管理器注销）
        private void OnDisable()
        {
            GameManager.Instance?.UnregisterSpawn(this);
        }
        // 在当前位置生成玩家
        public void SpawnHere()
        {
            var playerTransform = GameManager.Instance.Player.transform;
            // 设置玩家位置为当前生成点位置
            playerTransform.position = transform.position;
            // 如果存在主摄像机，则更新摄像机跟随和注视目标
            if (GameManager.Instance.MainCamera != null)
            {
                GameManager.Instance.MainCamera.Follow = playerTransform;
                GameManager.Instance.MainCamera.LookAt = playerTransform;
                GameManager.Instance.MainCamera.ForceCameraPosition(playerTransform.position, Quaternion.identity);
            }
        }
    }
#if UNITY_EDITOR
    // 自定义编辑器：用于检查生成点索引的唯一性
    [CustomEditor(typeof(SpawnPoint))]
    public class SpawnPointEditor : Editor
    {
        // 重写检视面板绘制逻辑
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            // 获取场景中所有生成点对象
            SpawnPoint[] transitions = GameObject.FindObjectsByType<SpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            var local = target as SpawnPoint;
            // 检查当前生成点索引是否唯一
            foreach (var transition in transitions)
            {
                if (transition == local)
                    continue;
                if (transition.SpawnIndex == local.SpawnIndex)
                {
                    // 显示错误提示：索引重复
                    EditorGUILayout.HelpBox(
                    $"生成点索引必须唯一，此索引已被 {transition.gameObject.name} 使用",
                    MessageType.Error);
                }
            }
        }
    }
#endif
}