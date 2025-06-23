using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 目标标记组件，用于在场景中可视化目标位置
    /// </summary>
    public class TargetMarker : MonoBehaviour
    {
        // 激活状态颜色
        [SerializeField]
        private Color _activeColor = Color.white;
        // 非激活状态颜色
        [SerializeField]
        private Color _inactiveColor = Color.gray;

        // 精灵渲染器组件
        private SpriteRenderer _renderer;

        private void Awake()
        {
            // 获取精灵渲染器组件
            _renderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// 激活目标标记（显示并设置为激活颜色）
        /// </summary>
        public void Activate()
        {
            Show();
            _renderer.color = _activeColor;
        }

        /// <summary>
        /// 停用目标标记（显示并设置为非激活颜色）
        /// </summary>
        public void Deactivate()
        {
            Show();
            _renderer.color = _inactiveColor;
        }

        /// <summary>
        /// 隐藏目标标记
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示目标标记
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}