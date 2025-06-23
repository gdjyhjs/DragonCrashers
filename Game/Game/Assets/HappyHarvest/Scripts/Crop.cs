using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;
namespace HappyHarvest
{
    /// <summary>
    /// ���ڶ����ͼ������������ࡣ�洢��������������׶Ρ�����ʱ�����Ϣ
    /// </summary>
    [CreateAssetMenu(fileName = "Crop", menuName = "2D Farming/Crop")]
    public class Crop : ScriptableObject, IDatabaseEntry
    {
        /// <summary>
        /// ���ݿ����ʵ�� IDatabaseEntry �ӿڣ�
        /// </summary>
        public string Key => UniqueID;

        // ����Ψһ��ʶ��
        public string UniqueID = "";

        // �����׶ζ�Ӧ��ͼ������
        public TileBase[] GrowthStagesTiles;

        // �ջ�ʱ�����Ĳ�Ʒ
        public Product Produce;

        // ������������ʱ��
        public float GrowthTime = 1.0f;
        // ���ջ����
        public int NumberOfHarvest = 1;
        // �ջ�����������׶�
        public int StageAfterHarvest = 1;
        // ÿ���ջ�Ĳ�Ʒ����
        public int ProductPerHarvest = 1;
        // �ɺ�������ʱ�����룩
        public float DryDeathTimer = 30.0f;
        // �ջ�ʱ���Ӿ�Ч��
        public VisualEffect HarvestEffect;

        /// <summary>
        /// ��������������ȡ��ǰ�����׶�
        /// </summary>
        /// <param name="growRatio">����������0-1��</param>
        /// <returns>��ǰ�����׶�����</returns>
        public int GetGrowthStage(float growRatio)
        {
            return Mathf.FloorToInt(growRatio * (GrowthStagesTiles.Length - 1));
        }
    }
}