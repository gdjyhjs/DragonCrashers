using System;
using System.Collections;
using System.Collections.Generic;
using HappyHarvest;
using Template2DCommon;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace HappyHarvest
{
    /// <summary>
    /// 在行走动画期间播放随机脚步声。该组件必须与玩家的Animator组件位于同一GameObject上，
    /// 因为它需要接收来自行走动画的PlayStepSound事件。
    /// 包含一个瓦片与音效的映射列表，可根据玩家脚下的瓦片类型播放不同的音效。
    /// 注意：用于检测玩家行走瓦片的Tilemap必须包含WalkableSurface组件。
    /// </summary>
    public class StepSoundHandler : MonoBehaviour
    {
        [Serializable]
        public class TileSoundMapping
        {
            // 关联的瓦片数组
            public TileBase[] Tiles;
            // 对应的脚步声数组
            public AudioClip[] StepSounds;
        }

        // 默认脚步声数组
        public AudioClip[] DefaultStepSounds;
        // 瓦片音效映射数组
        public TileSoundMapping[] SoundMappings;

        // 瓦片到脚步声的映射字典
        private Dictionary<TileBase, AudioClip[]> m_Mapping = new();

        void Start()
        {
            // 初始化瓦片到音效的映射关系
            foreach (var mapping in SoundMappings)
            {
                foreach (var tile in mapping.Tiles)
                {
                    m_Mapping[tile] = mapping.StepSounds;
                }
            }
        }

        // 由角色行走动画的动画事件调用
        public void PlayStepSound()
        {
            // 获取玩家脚下的瓦片坐标
            var underCell = GameManager.Instance.WalkSurfaceTilemap.WorldToCell(transform.position);
            // 获取瓦片对象
            var tile = GameManager.Instance.WalkSurfaceTilemap.GetTile(underCell);

            // 根据瓦片类型播放对应的随机脚步声
            SoundManager.Instance.PlaySFXAt(transform.position,
                (tile != null && m_Mapping.ContainsKey(tile))
                    ? GetRandomEntry(m_Mapping[tile])
                    : GetRandomEntry(DefaultStepSounds), false);
        }

        // 从数组中随机获取一个音频片段
        AudioClip GetRandomEntry(AudioClip[] clips)
        {
            return clips[Random.Range(0, clips.Length)];
        }
    }
}