using System;
using System.Collections;
using System.Collections.Generic;
using HappyHarvest;
using UnityEngine;
namespace HappyHarvest
{
    // �ӳ�ִ����ȷ����������ʵ����
    [DefaultExecutionOrder(999)]
    [ExecuteInEditMode]
    public class ShadowInstance : MonoBehaviour
    {
        // ������Ӱ���ȣ���Χ 0-10��
        [Range(0, 10f)] public float BaseLength = 1f;

        private void OnEnable()
        {
            // ����ҹѭ��������ע����Ӱʵ��
            DayCycleHandler.RegisterShadow(this);
        }

        private void OnDisable()
        {
            // ����ҹѭ��������ע����Ӱʵ��
            DayCycleHandler.UnregisterShadow(this);
        }
    }
}
