using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// ���������������ڿ���Unity��������Ĳ���
    /// </summary>
    public class StartAnimation : MonoBehaviour
    {
        // Ŀ�궯�����
        public Animation Animation;

        /// <summary>
        /// �����������ŵĹ�������
        /// </summary>
        public void Trigger()
        {
            // ���Ŷ�������е�Ĭ�϶���
            Animation.Play();
        }
    }
}