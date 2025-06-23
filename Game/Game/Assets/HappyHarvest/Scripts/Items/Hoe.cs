using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ��ͷ��Ʒ�࣬���ڷ����ؿ�
    /// </summary>
    [CreateAssetMenu(fileName = "Hoe", menuName = "2D Farming/Items/Hoe")]
    public class Hoe : Item
    {
        /// <summary>
        /// ���Ŀ��λ���Ƿ��ʹ�ó�ͷ
        /// </summary>
        /// <param name="target">Ŀ����������</param>
        /// <returns>�Ƿ��ʹ��</returns>
        public override bool CanUse(Vector3Int target)
        {
            // ���������ڵ��ι�������Ŀ��λ�ÿɷ���
            return GameManager.Instance?.Terrain != null && GameManager.Instance.Terrain.IsTillable(target);
        }

        /// <summary>
        /// ʹ�ó�ͷ�����ؿ�
        /// </summary>
        /// <param name="target">Ŀ����������</param>
        /// <returns>ʹ���Ƿ�ɹ�</returns>
        public override bool Use(Vector3Int target)
        {
            // ���õ��ι������ķ�������
            GameManager.Instance.Terrain.TillAt(target);
            return true;
        }
    }
}