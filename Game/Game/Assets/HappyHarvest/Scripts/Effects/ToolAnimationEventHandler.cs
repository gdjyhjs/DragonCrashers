using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
namespace HappyHarvest
{
    /// <summary>
    /// 工具动画可在正确帧调用不同事件以触发设置的视觉效果
    /// 此脚本需要添加到工具的 Animator 所在的同一 GameObject 上，以便接收动画事件。
    /// </summary>
    public class ToolAnimationEventHandler : MonoBehaviour
    {
        [Header("前方")]
        // 前方视觉效果
        public VisualEffect FrontEffect;
        // 前方效果事件 ID
        public string FrontEffectId;

        [Header("上方")]
        // 上方视觉效果
        public VisualEffect UpEffect;
        // 上方效果事件 ID
        public string UpEffectId;

        [Header("侧面")]
        // 侧面视觉效果
        public VisualEffect SideEffect;
        // 侧面效果事件 ID
        public string SideEffectId;

        /// <summary>
        /// 触发前方视觉效果
        /// </summary>
        public void TriggerFrontVFX()
        {
            // 激活前方效果，禁用其他方向效果
            SideEffect.gameObject.SetActive(false);
            UpEffect.gameObject.SetActive(false);
            FrontEffect.gameObject.SetActive(true);

            // 发送效果触发事件
            FrontEffect.SendEvent(FrontEffectId);
        }

        /// <summary>
        /// 触发侧面视觉效果
        /// </summary>
        public void TriggerSideVFX()
        {
            // 激活侧面效果，禁用其他方向效果
            SideEffect.gameObject.SetActive(true);
            UpEffect.gameObject.SetActive(false);
            FrontEffect.gameObject.SetActive(false);

            // 发送效果触发事件
            SideEffect.SendEvent(SideEffectId);
        }

        /// <summary>
        /// 触发上方视觉效果
        /// </summary>
        public void TriggerUpVFX()
        {
            // 激活上方效果，禁用其他方向效果
            SideEffect.gameObject.SetActive(false);
            UpEffect.gameObject.SetActive(true);
            FrontEffect.gameObject.SetActive(false);

            // 发送效果触发事件
            UpEffect.SendEvent(UpEffectId);
        }
    }
}