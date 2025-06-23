using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ��Ʒ�࣬�̳��� Item ���࣬����ɳ��۵�ũ��Ʒ
    /// </summary>
    [CreateAssetMenu(menuName = "2D Farming/Items/Product")]
    public class Product : Item
    {
        // ��Ʒ���ۼ۸�
        public int SellPrice = 1;

        /// <summary>
        /// ����Ʒ�Ƿ��ʹ�ã�ʼ�շ��� true��
        /// </summary>
        public override bool CanUse(Vector3Int target)
        {
            return true;
        }

        /// <summary>
        /// ʹ�ò�Ʒ��ʼ�շ��� true����ʵ��ʹ���߼���
        /// </summary>
        public override bool Use(Vector3Int target)
        {
            return true;
        }

        /// <summary>
        /// �жϲ�Ʒ�Ƿ���ҪĿ��λ�ã����� false����Ʒ����Ŀ�꣩
        /// </summary>
        public override bool NeedTarget()
        {
            return false;
        }
    }
}