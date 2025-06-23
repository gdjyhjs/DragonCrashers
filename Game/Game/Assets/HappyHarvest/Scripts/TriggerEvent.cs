using System;
using UnityEngine;
using UnityEngine.Events;
namespace HappyHarvest
{
    /// <summary>
    /// �������¼��������������ײ�������뿪������ʱ���� Unity �¼�
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class TriggerEvent : MonoBehaviour
    {
        // ���봥����ʱ�������¼�
        public UnityEvent OnEnter;
        // �뿪������ʱ�������¼�
        public UnityEvent OnExit;

        /// <summary>
        /// ����ײ����봥����ʱ���� OnEnter �¼�
        /// </summary>
        private void OnTriggerEnter2D(Collider2D col)
        {
            OnEnter.Invoke();
        }

        /// <summary>
        /// ����ײ���뿪������ʱ���� OnExit �¼�
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            OnExit.Invoke();
        }
    }
}
