using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ������Ʒ�࣬�����ջ��������
    /// </summary>
    [CreateAssetMenu(menuName = "2D Farming/Items/Basket")]
    public class Basket : Item
    {
        /// <summary>
        /// ���Ŀ��λ���Ƿ��ʹ������
        /// </summary>
        /// <param name="target">Ŀ����������</param>
        /// <returns>�Ƿ��ʹ��</returns>
        public override bool CanUse(Vector3Int target)
        {
            // ��ȡĿ��λ�õ���������
            var data = GameManager.Instance.Terrain.GetCropDataAt(target);
            // �����������������ݡ��������е������������Ϊ 100%
            return data != null && data.GrowingCrop != null && Mathf.Approximately(data.GrowthRatio, 1.0f);
        }

        /// <summary>
        /// ʹ�������ջ�����
        /// </summary>
        /// <param name="target">Ŀ����������</param>
        /// <returns>ʹ���Ƿ�ɹ�</returns>
        public override bool Use(Vector3Int target)
        {
            // ��ȡĿ��λ�õ���������
            var data = GameManager.Instance.Terrain.GetCropDataAt(target);
            // ��鱳���Ƿ��������ջ������
            if (!GameManager.Instance.Player.CanFitInInventory(data.GrowingCrop.Produce,
            data.GrowingCrop.ProductPerHarvest))
                return false;

            // �ջ�����
            var product = GameManager.Instance.Terrain.HarvestAt(target);

            if (product != null)
            {
                // ���ջ��������ӵ���ұ���
                for (int i = 0; i < product.ProductPerHarvest; ++i)
                {
                    GameManager.Instance.Player.AddItem(product.Produce);
                }

                return true;
            }

            return false;
        }
    }
}