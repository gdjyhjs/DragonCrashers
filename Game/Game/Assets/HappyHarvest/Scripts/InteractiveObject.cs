using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// �����пɵ����������Ļ��ࡣʵ�ִ˽ӿڵĶ���ɱ���ҽ���
    /// </summary>
    public abstract class InteractiveObject : MonoBehaviour
    {
        /// <summary>
        /// �����󱻽���ʱ���õķ������������ʵ�ִ˷���
        /// </summary>
        public abstract void InteractedWith();

        /// <summary>
        /// �����ʼ��ʱ����ͼ��Ϊ31����������㣩
        /// </summary>
        protected void Awake()
        {
            // ��ҿ�����ͨ�����߼���31����ʶ��ɽ�������
            gameObject.layer = 31;
        }
    }
}