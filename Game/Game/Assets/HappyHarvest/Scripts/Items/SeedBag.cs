using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ���Ӵ���Ʒ�࣬�����ڸ�������ֲ����
    /// </summary>
    [CreateAssetMenu(fileName = "SeedBag", menuName = "2D Farming/Items/SeedBag")]
    public class SeedBag : Item
    {
        // ��ֲ����������
        public Crop PlantedCrop;

        /// <summary>
        /// ���Ŀ��λ���Ƿ����ֲ
        /// </summary>
        public override bool CanUse(Vector3Int target)
        {
            // ���������ι�����������Ŀ��λ�ÿ���ֲ
            return GameManager.Instance.Terrain.IsPlantable(target);
        }

        /// <summary>
        /// ʹ�����Ӵ���ֲ����
        /// </summary>
        public override bool Use(Vector3Int target)
        {
            // ���õ��ι���������ֲ����
            GameManager.Instance.Terrain.PlantAt(target, PlantedCrop);
            return true;
        }
    }
}