using UnityEngine;

namespace HappyHarvest
{
    /// <summary>
    /// �������ݿ⣬���ڴ洢�͹������п��õ���������
    /// ͨ��CreateAssetMenu���Կ���Unity�༭���д��������ݿ��ʵ��
    /// </summary>
    [CreateAssetMenu(fileName = "CropDatabase", menuName = "2D Farming/Crop Database")]
    public class CropDatabase : BaseDatabase<Crop>
    {
        // �̳�BaseDatabase�����й��ܣ��������ʵ��
    }
}