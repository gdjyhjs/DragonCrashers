using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// ����ϵͳԪ����������ڸ��ݵ�ǰ����״̬����������Ϸ����
    /// ���磺�����ЧӦ����Ϊ����������ױ�����ʱ����
    /// </summary>
    [DefaultExecutionOrder(999)]
    [ExecuteInEditMode]
    public class WeatherSystemElement : MonoBehaviour
    {
        // ��Ԫ�ض�Ӧ����������
        public WeatherSystem.WeatherType WeatherType;

        /// <summary>
        /// ��������ʱ������ϵͳ��ע����Ԫ��
        /// </summary>
        private void OnDestroy()
        {
            WeatherSystem.UnregisterElement(this);
        }
    }
}