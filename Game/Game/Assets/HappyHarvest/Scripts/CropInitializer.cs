using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// ��ʾ��;�ĸ����ࡣ����ָ����Ԫ���а����������׶��Զ���ֲָ������
    /// </summary>
    public class CropInitializer : MonoBehaviour
    {
        /// <summary>
        /// �����ʼ�����ݽṹ
        /// </summary>
        [Serializable]
        public struct CropInitData
        {
            public Vector2Int Cell; // ��Ԫ������
            public Crop CropToPlant; // Ҫ��ֲ������
            public int StartingStage; // ��ʼ�����׶�
        }

        // �����ʼ�������б�
        public CropInitData[] InitList;

        private void Start()
        {
            var terrain = GameManager.Instance.Terrain;

            // �������г�ʼ������
            foreach (var initData in InitList)
            {
                Vector3Int target = (Vector3Int)initData.Cell;
                // ���ؿ��Ƿ�ɷ������ѷ���
                if (terrain.IsTillable(target) || terrain.IsTilled(target))
                {
                    var data = GameManager.Instance.Terrain.GetCropDataAt(target);
                    if (data == null)
                    {
                        // �ؿ�������ʱִ�г�ʼ����ֲ
                        terrain.TillAt(target); // �����ؿ�
                        terrain.WaterAt(target); // ��ȵؿ�
                        terrain.PlantAt(target, initData.CropToPlant); // ��ֲ����

                        // ���������׶�Ϊָ����ʼ�׶�
                        terrain.OverrideGrowthStage(target, initData.StartingStage);
                    }
                }
            }
        }
    }
}
