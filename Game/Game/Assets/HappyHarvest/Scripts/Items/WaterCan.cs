using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ˮ����Ʒ�࣬���ڹ���ѷ����ĵؿ�
    /// </summary>
    [CreateAssetMenu(fileName = "WaterCan", menuName = "2D Farming/Items/Water Can")]
    public class WaterCan : Item
    {
        /// <summary>
        /// ���Ŀ��λ���Ƿ�ɹ��
        /// </summary>
        public override bool CanUse(Vector3Int target)
        {
            // ���������ι�����������Ŀ��ؿ��ѷ���
            return GameManager.Instance.Terrain.IsTilled(target);
        }

        /// <summary>
        /// ʹ��ˮ����ȵؿ�
        /// </summary>
        public override bool Use(Vector3Int target)
        {
            // ���õ��ι������Ĺ�ȷ���
            GameManager.Instance.Terrain.WaterAt(target);
            return true;
        }
    }
}