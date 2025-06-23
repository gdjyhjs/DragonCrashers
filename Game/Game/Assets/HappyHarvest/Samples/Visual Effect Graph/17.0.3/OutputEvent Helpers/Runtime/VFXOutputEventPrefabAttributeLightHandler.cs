using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if VFX_OUTPUTEVENT_HDRP_10_0_0_OR_NEWER 
using UnityEngine.Rendering.HighDefinition; 
#endif
namespace UnityEngine.VFX.Utility
{
    /// <summary>
    /// Visual Effect Graph ����¼�Ԥ�������Դ����������ڸ��� VFX �¼����õƹ�����
    /// �� VFX �����¼�ʱ�����¼��е���ɫ����ת��Ϊ�ƹ����ɫ��ǿ��
    /// </summary>
    [RequireComponent(typeof(Light))]
#if VFX_OUTPUTEVENT_HDRP_10_0_0_OR_NEWER
[RequireComponent (typeof (HDAdditionalLightData))]
#endif
    class VFXOutputEventPrefabAttributeLightHandler : VFXOutputEventPrefabAttributeAbstractHandler
    {
        // ��������ϵ�������ڵ�����������
        public float brightnessScale = 1.0f;
        // VFX ���� ID����ɫ
        static readonly int k_Color = Shader.PropertyToID("color");

        /// <summary>
        /// �� VFX ��������¼�ʱ���ã�����ƹ���������
        /// </summary>
        /// <param name="eventAttribute">VFX �¼���������</param>
        /// <param name="visualEffect">Visual Effect �������</param>
        public override void OnVFXEventAttribute(VFXEventAttribute eventAttribute, VisualEffect visualEffect)
        {
            // �� VFX �¼��л�ȡ��ɫ���ԣ�Vector3 ��ʾ RGB ��ɫ��
            var color = eventAttribute.GetVector3(k_Color);
            // ��ɫ������ģ����Ϊ��ʼǿ��
            var intensity = color.magnitude;
            // ��һ����ɫ������������ɫ��ǿ��
            var c = new Color(color.x, color.y, color.z) / intensity;
            // Ӧ����������ϵ��
            intensity *= brightnessScale;

            // ������Ⱦ�����������õƹ�����
#if VFX_OUTPUTEVENT_HDRP_10_0_0_OR_NEWER
// HDRP ����ר���߼�
var hdlight = GetComponent<HDAdditionalLightData>();
hdlight.SetColor (c);
hdlight.SetIntensity (intensity);
#else
            // ��׼�����߼�
            var light = GetComponent<Light>();
            light.color = c;
            light.intensity = intensity;
#endif
        }
    }
}