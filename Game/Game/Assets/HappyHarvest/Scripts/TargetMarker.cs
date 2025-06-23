using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// Ŀ��������������ڳ����п��ӻ�Ŀ��λ��
    /// </summary>
    public class TargetMarker : MonoBehaviour
    {
        // ����״̬��ɫ
        [SerializeField]
        private Color _activeColor = Color.white;
        // �Ǽ���״̬��ɫ
        [SerializeField]
        private Color _inactiveColor = Color.gray;

        // ������Ⱦ�����
        private SpriteRenderer _renderer;

        private void Awake()
        {
            // ��ȡ������Ⱦ�����
            _renderer = GetComponent<SpriteRenderer>();
        }

        /// <summary>
        /// ����Ŀ���ǣ���ʾ������Ϊ������ɫ��
        /// </summary>
        public void Activate()
        {
            Show();
            _renderer.color = _activeColor;
        }

        /// <summary>
        /// ͣ��Ŀ���ǣ���ʾ������Ϊ�Ǽ�����ɫ��
        /// </summary>
        public void Deactivate()
        {
            Show();
            _renderer.color = _inactiveColor;
        }

        /// <summary>
        /// ����Ŀ����
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// ��ʾĿ����
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}