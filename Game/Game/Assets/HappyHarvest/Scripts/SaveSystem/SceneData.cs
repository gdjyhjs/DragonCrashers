using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// ÿ�������������������ڶ��峡����Ψһ���ƣ�����ϵͳʹ�ø����Ʊ�ʶ������
    /// ����ζ�ų��������ƶ���������������乹�� ID���������ƻ��������ݡ�
    /// </summary>
    public class SceneData : MonoBehaviour
    {
        // ������Ψһ���ƣ����ڱ���ϵͳ��ʶ������
        public string UniqueSceneName;

        private void OnEnable()
        {
            // ���������ʱ������ǰ�������ݸ�ֵ����Ϸ������
            GameManager.Instance.LoadedSceneData = this;
        }

        private void OnDisable()
        {
            // ���������ʱ������Ϸ���������Ƴ���ǰ������������
            if (GameManager.Instance?.LoadedSceneData == this)
                GameManager.Instance.LoadedSceneData = null;
        }
    }
}