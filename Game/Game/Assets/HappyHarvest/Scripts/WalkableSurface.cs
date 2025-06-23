using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HappyHarvest
{
    /// <summary>
    /// 将此组件添加到定义玩家行走表面的瓷砖地图上。声音系统使用此组件来确定
    /// 玩家行走时应播放哪些声音。
    /// </summary>
    [RequireComponent(typeof(Tilemap))]
    public class WalkableSurface : MonoBehaviour
    {
        private void Awake()
        {
            // 将当前瓷砖地图组件赋值给游戏管理器的行走表面引用
            GameManager.Instance.WalkSurfaceTilemap = GetComponent<Tilemap>();
        }
    }
}