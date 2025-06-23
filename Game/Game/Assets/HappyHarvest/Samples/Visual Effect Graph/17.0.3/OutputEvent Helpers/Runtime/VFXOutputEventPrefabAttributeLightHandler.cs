using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if VFX_OUTPUTEVENT_HDRP_10_0_0_OR_NEWER 
using UnityEngine.Rendering.HighDefinition; 
#endif
namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph 输出事件预制体属性处理器，用于根据 VFX 事件设置灯光属性
    /// 当 VFX 触发事件时，将事件中的颜色属性转换为灯光的颜色和强度
    /// </summary>
    [RequireComponent(typeof(Light))]
#if VFX_OUTPUTEVENT_HDRP_10_0_0_OR_NEWER
[RequireComponent (typeof (HDAdditionalLightData))]
#endif
    class VFXOutputEventPrefabAttributeLightHandler : VFXOutputEventPrefabAttributeAbstractHandler
    {
        // 亮度缩放系数，用于调整最终亮度
        public float brightnessScale = 1.0f;
        // VFX 属性 ID：颜色
        static readonly int k_Color = Shader.PropertyToID("color");

        /// <summary>
        /// 当 VFX 触发输出事件时调用，处理灯光属性设置
        /// </summary>
        /// <param name="eventAttribute">VFX 事件属性数据</param>
        /// <param name="visualEffect">Visual Effect 组件引用</param>
        public override void OnVFXEventAttribute(VFXEventAttribute eventAttribute, VisualEffect visualEffect)
        {
            // 从 VFX 事件中获取颜色属性（Vector3 表示 RGB 颜色）
            var color = eventAttribute.GetVector3(k_Color);
            // 颜色向量的模长作为初始强度
            var intensity = color.magnitude;
            // 归一化颜色分量，分离颜色和强度
            var c = new Color(color.x, color.y, color.z) / intensity;
            // 应用亮度缩放系数
            intensity *= brightnessScale;

            // 根据渲染管线类型设置灯光属性
#if VFX_OUTPUTEVENT_HDRP_10_0_0_OR_NEWER
// HDRP 管线专用逻辑
var hdlight = GetComponent<HDAdditionalLightData>();
hdlight.SetColor (c);
hdlight.SetIntensity (intensity);
#else
            // 标准管线逻辑
            var light = GetComponent<Light>();
            light.color = c;
            light.intensity = intensity;
#endif
        }
    }
}