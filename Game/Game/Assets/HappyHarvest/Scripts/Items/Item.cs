using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ��Ʒ���࣬������Ʒ��Ӧ�̳��Ը���
    /// ʵ���� IDatabaseEntry �ӿ���֧�����ݿ����
    /// </summary>
    public abstract class Item : ScriptableObject, IDatabaseEntry
    {
        /// <summary>
        /// ���ݿ����ʵ�� IDatabaseEntry �ӿڣ�
        /// </summary>
        public string Key => UniqueID;

        [Tooltip("���ݿ������ڱ�ʶ����Ʒ�����ƣ�����ϵͳʹ��")]
        // ��ƷΨһ��ʶ��
        public string UniqueID = "DefaultID";

        // ��Ʒ��ʾ����
        public string DisplayName;
        // ��Ʒͼ��
        public Sprite ItemSprite;
        // ���ѵ�����
        public int MaxStackSize = 10;
        // �Ƿ�Ϊ����Ʒ
        public bool Consumable = true;
        // ����۸�-1 ��ʾ���ɹ���
        public int BuyPrice = -1;

        [Tooltip("װ��ʱ���������ʵ������Ԥ����")]
        // װ��ʱ���Ӿ�Ԥ����
        public GameObject VisualPrefab;
        // ʹ����Ʒʱ��������Ҷ�������������
        public string PlayerAnimatorTriggerUse = "GenericToolSwing";

        [Tooltip("ʹ����Ʒʱ��������Ч")]
        // ʹ����Ʒʱ����Ч����
        public AudioClip[] UseSound;

        /// <summary>
        /// �����Ʒ�Ƿ����Ŀ��λ��ʹ��
        /// </summary>
        /// <param name="target">Ŀ����������</param>
        /// <returns>�Ƿ��ʹ��</returns>
        public abstract bool CanUse(Vector3Int target);

        /// <summary>
        /// ʹ����Ʒ��������ʵ�־����߼���
        /// </summary>
        /// <param name="target">Ŀ����������</param>
        /// <returns>ʹ���Ƿ�ɹ�</returns>
        public abstract bool Use(Vector3Int target);

        /// <summary>
        /// �ж���Ʒ�Ƿ���ҪĿ��λ�ã�Ĭ����Ҫ��
        /// ����Ŀ�����Ʒ�����ֱ��ʹ�õ�����Ʒ������д�˷���
        /// </summary>
        public virtual bool NeedTarget()
        {
            return true;
        }
    }
}