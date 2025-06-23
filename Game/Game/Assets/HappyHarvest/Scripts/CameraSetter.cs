using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ���������ӵ���������Ϊ����Ϸ������� CinemachineVirtualCamera �ϡ�
    /// ��������ʱ����ҽ�ͨ���˽ű�����������Ϊ�������Ŀ��
    /// </summary>
    [DefaultExecutionOrder(100)]
    public class CameraSetter : MonoBehaviour
    {
        private void Awake()
        {
            // ��ȡ Cinemachine ��������
            var cam = GetComponent<CinemachineCamera>();
            // ����ǰ���������Ϊ GameManager �е��������
            GameManager.Instance.MainCamera = cam;
        }
    }
}