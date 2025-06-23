using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HappyHarvest
{
    /// <summary>
    /// 演示用途的辅助类。将在指定单元格中按给定生长阶段自动种植指定作物
    /// </summary>
    public class CropInitializer : MonoBehaviour
    {
        /// <summary>
        /// 作物初始化数据结构
        /// </summary>
        [Serializable]
        public struct CropInitData
        {
            public Vector2Int Cell; // 单元格坐标
            public Crop CropToPlant; // 要种植的作物
            public int StartingStage; // 起始生长阶段
        }

        // 作物初始化数据列表
        public CropInitData[] InitList;

        private void Start()
        {
            var terrain = GameManager.Instance.Terrain;

            // 遍历所有初始化数据
            foreach (var initData in InitList)
            {
                Vector3Int target = (Vector3Int)initData.Cell;
                // 检查地块是否可翻耕或已翻耕
                if (terrain.IsTillable(target) || terrain.IsTilled(target))
                {
                    var data = GameManager.Instance.Terrain.GetCropDataAt(target);
                    if (data == null)
                    {
                        // 地块无作物时执行初始化种植
                        terrain.TillAt(target); // 翻耕地块
                        terrain.WaterAt(target); // 灌溉地块
                        terrain.PlantAt(target, initData.CropToPlant); // 种植作物

                        // 覆盖生长阶段为指定起始阶段
                        terrain.OverrideGrowthStage(target, initData.StartingStage);
                    }
                }
            }
        }
    }
}
