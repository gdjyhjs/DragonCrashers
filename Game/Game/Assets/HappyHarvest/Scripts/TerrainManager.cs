using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;

namespace HappyHarvest
{
    /// <summary>
    /// 管理与种植作物的地形相关的所有内容。保存单元格中作物状态的数据，
    /// 处理翻耕和浇水等操作时的瓷砖切换等事宜。
    /// </summary>
    public class TerrainManager : MonoBehaviour
    {
        [System.Serializable]
        public class GroundData
        {
            // 浇水持续时间（秒）
            public const float WaterDuration = 60 * 1.0f;
            // 浇水计时器
            public float WaterTimer;
        }
        public class CropData
        {
            [Serializable]
            public struct SaveData
            {
                // 作物 ID
                public string CropId;
                // 当前生长阶段
                public int Stage;
                // 生长进度比例
                public float GrowthRatio;
                // 生长计时器
                public float GrowthTimer;
                // 收获次数
                public int HarvestCount;
                // 枯萎计时器
                public float DyingTimer;
            }
            // 正在生长的作物
            public Crop GrowingCrop = null;
            // 当前生长阶段
            public int CurrentGrowthStage = 0;
            // 生长进度比例
            public float GrowthRatio = 0.0f;
            // 生长计时器
            public float GrowthTimer = 0.0f;
            // 收获次数
            public int HarvestCount = 0;
            // 枯萎计时器
            public float DyingTimer;
            // 是否已完成所有收获
            public bool HarvestDone => HarvestCount == GrowingCrop.NumberOfHarvest;
            // 初始化作物数据
            public void Init()
            {
                GrowingCrop = null;
                GrowthRatio = 0.0f;
                GrowthTimer = 0.0f;
                CurrentGrowthStage = 0;
                HarvestCount = 0;
                DyingTimer = 0.0f;
            }
            // 收获作物
            public Crop Harvest()
            {
                var crop = GrowingCrop;
                HarvestCount += 1;
                CurrentGrowthStage = GrowingCrop.StageAfterHarvest;
                GrowthRatio = CurrentGrowthStage / (float)GrowingCrop.GrowthStagesTiles.Length;
                GrowthTimer = GrowingCrop.GrowthTime * GrowthRatio;
                return crop;
            }
            // 保存作物数据
            public void Save(ref SaveData data)
            {
                data.Stage = CurrentGrowthStage;
                data.CropId = GrowingCrop.Key;
                data.DyingTimer = DyingTimer;
                data.GrowthRatio = GrowthRatio;
                data.GrowthTimer = GrowthTimer;
                data.HarvestCount = HarvestCount;
            }
            // 加载作物数据
            public void Load(SaveData data)
            {
                CurrentGrowthStage = data.Stage;
                GrowingCrop = GameManager.Instance.CropDatabase.GetFromID(data.CropId);
                DyingTimer = data.DyingTimer;
                GrowthRatio = data.GrowthRatio;
                GrowthTimer = data.GrowthTimer;
                HarvestCount = data.HarvestCount;
            }
        }
        // 网格引用
        public Grid Grid;
        // 地面瓷砖地图
        public Tilemap GroundTilemap;
        // 作物瓷砖地图
        public Tilemap CropTilemap;
        [Header("浇水设置")]
        // 浇水瓷砖地图
        public Tilemap WaterTilemap;
        // 已浇水的瓷砖
        public TileBase WateredTile;
        [Header("翻耕设置")]
        // 可翻耕的瓷砖
        public TileBase TilleableTile;
        // 已翻耕的瓷砖
        public TileBase TilledTile;
        // 翻耕效果预制体
        public VisualEffect TillingEffectPrefab;
        // 地面数据字典（坐标 -> 地面数据）
        private Dictionary<Vector3Int, GroundData> m_GroundData = new();
        // 作物数据字典（坐标 -> 作物数据）
        private Dictionary<Vector3Int, CropData> m_CropData = new();
        // 收获效果池（作物 -> 效果列表）
        private Dictionary<Crop, List<VisualEffect>> m_HarvestEffectPool = new();
        // 翻耕效果池
        private List<VisualEffect> m_TillingEffectPool = new();
        // 检查指定位置是否可翻耕
        public bool IsTillable(Vector3Int target)
        {
            return GroundTilemap.GetTile(target) == TilleableTile;
        }
        // 检查指定位置是否可种植
        public bool IsPlantable(Vector3Int target)
        {
            return IsTilled(target) && !m_CropData.ContainsKey(target);
        }
        // 检查指定位置是否已翻耕
        public bool IsTilled(Vector3Int target)
        {
            return m_GroundData.ContainsKey(target);
        }
        // 在指定位置翻耕
        public void TillAt(Vector3Int target)
        {
            if (IsTilled(target))
                return;
            GroundTilemap.SetTile(target, TilledTile);
            m_GroundData.Add(target, new GroundData());
            var inst = m_TillingEffectPool[0];
            m_TillingEffectPool.RemoveAt(0);
            m_TillingEffectPool.Add(inst);
            inst.gameObject.transform.position = Grid.GetCellCenterWorld(target);
            inst.Stop();
            inst.Play();
        }
        // 在指定位置种植作物
        public void PlantAt(Vector3Int target, Crop cropToPlant)
        {
            var cropData = new CropData();
            cropData.GrowingCrop = cropToPlant;
            cropData.GrowthTimer = 0.0f;
            cropData.CurrentGrowthStage = 0;
            m_CropData.Add(target, cropData);
            UpdateCropVisual(target);
            if (!m_HarvestEffectPool.ContainsKey(cropToPlant))
            {
                InitHarvestEffect(cropToPlant);
            }
        }
        // 初始化作物收获效果池
        public void InitHarvestEffect(Crop crop)
        {
            m_HarvestEffectPool[crop] = new List<VisualEffect>();
            for (int i = 0; i < 4; ++i)
            {
                var inst = Instantiate(crop.HarvestEffect);
                inst.Stop();
                m_HarvestEffectPool[crop].Add(inst);
            }
        }
        // 在指定位置浇水
        public void WaterAt(Vector3Int target)
        {
            var groundData = m_GroundData[target];
            groundData.WaterTimer = GroundData.WaterDuration;
            WaterTilemap.SetTile(target, WateredTile);
        }
        // 在指定位置收获作物
        public Crop HarvestAt(Vector3Int target)
        {
            m_CropData.TryGetValue(target, out var data);
            if (data == null || !Mathf.Approximately(data.GrowthRatio, 1.0f)) return null;
            var produce = data.Harvest();
            if (data.HarvestDone)
            {
                m_CropData.Remove(target);
            }
            UpdateCropVisual(target);
            var effect = m_HarvestEffectPool[data.GrowingCrop][0];
            effect.transform.position = Grid.GetCellCenterWorld(target);
            m_HarvestEffectPool[data.GrowingCrop].RemoveAt(0);
            m_HarvestEffectPool[data.GrowingCrop].Add(effect);
            effect.Play();
            return produce;
        }
        // 获取指定位置的作物数据
        public CropData GetCropDataAt(Vector3Int target)
        {
            m_CropData.TryGetValue(target, out var data);
            return data;
        }
        // 覆盖指定位置的作物生长阶段
        public void OverrideGrowthStage(Vector3Int target, int newGrowthStage)
        {
            var data = GetCropDataAt(target);
            data.GrowthRatio = Mathf.Clamp01((newGrowthStage + 1) / (float)data.GrowingCrop.GrowthStagesTiles.Length);
            data.GrowthTimer = data.GrowthRatio * data.GrowingCrop.GrowthTime;
            data.CurrentGrowthStage = newGrowthStage;
            UpdateCropVisual(target);
        }
        private void Awake()
        {
            GameManager.Instance.Terrain = this;
            for (int i = 0; i < 4; ++i)
            {
                var effect = Instantiate(TillingEffectPrefab);
                effect.gameObject.SetActive(true);
                effect.Stop();
                m_TillingEffectPool.Add(effect);
            }
        }
        private void Update()
        {
            foreach (var (cell, groundData) in m_GroundData)
            {
                if (groundData.WaterTimer > 0.0f)
                {
                    groundData.WaterTimer -= Time.deltaTime;
                    if (groundData.WaterTimer <= 0.0f)
                    {
                        WaterTilemap.SetTile(cell, null);
                    }
                }
                if (m_CropData.TryGetValue(cell, out var cropData))
                {
                    if (groundData.WaterTimer <= 0.0f)
                    {
                        cropData.DyingTimer += Time.deltaTime;
                        if (cropData.DyingTimer > cropData.GrowingCrop.DryDeathTimer)
                        {
                            m_CropData.Remove(cell);
                            UpdateCropVisual(cell);
                        }
                    }
                    else
                    {
                        cropData.DyingTimer = 0.0f;
                        cropData.GrowthTimer = Mathf.Clamp(cropData.GrowthTimer + Time.deltaTime, 0.0f,
                        cropData.GrowingCrop.GrowthTime);
                        cropData.GrowthRatio = cropData.GrowthTimer / cropData.GrowingCrop.GrowthTime;
                        int growthStage = cropData.GrowingCrop.GetGrowthStage(cropData.GrowthRatio);
                        if (growthStage != cropData.CurrentGrowthStage)
                        {
                            cropData.CurrentGrowthStage = growthStage;
                            UpdateCropVisual(cell);
                        }
                    }
                }
            }
        }
        // 更新作物视觉效果
        void UpdateCropVisual(Vector3Int target)
        {
            if (!m_CropData.TryGetValue(target, out var data))
            {
                CropTilemap.SetTile(target, null);
            }
            else
            {
                CropTilemap.SetTile(target, data.GrowingCrop.GrowthStagesTiles[data.CurrentGrowthStage]);
            }
        }
        // 保存地形数据
        public void Save(ref TerrainDataSave data)
        {
            data.GroundDatas = new List<GroundData>();
            data.GroundDataPositions = new List<Vector3Int>();
            foreach (var groundData in m_GroundData)
            {
                data.GroundDataPositions.Add(groundData.Key);
                data.GroundDatas.Add(groundData.Value);
            }
            data.CropDatas = new List<CropData.SaveData>();
            data.CropDataPositions = new List<Vector3Int>();
            foreach (var cropData in m_CropData)
            {
                data.CropDataPositions.Add(cropData.Key);
                var saveData = new CropData.SaveData();
                cropData.Value.Save(ref saveData);
                data.CropDatas.Add(saveData);
            }
        }
        // 加载地形数据
        public void Load(TerrainDataSave data)
        {
            m_GroundData = new Dictionary<Vector3Int, GroundData>();
            for (int i = 0; i < data.GroundDatas.Count; ++i)
            {
                var pos = data.GroundDataPositions[i];
                m_GroundData.Add(pos, data.GroundDatas[i]);
                GroundTilemap.SetTile(pos, TilledTile);
                WaterTilemap.SetTile(data.GroundDataPositions[i], data.GroundDatas[i].WaterTimer > 0.0f ? WateredTile : null);
            }
            // 清除所有现有效果，以便重新加载新效果
            foreach (var pool in m_HarvestEffectPool)
            {
                if (pool.Value != null)
                {
                    foreach (var effect in pool.Value)
                    {
                        Destroy(effect.gameObject);
                    }
                }
            }
            m_CropData = new Dictionary<Vector3Int, CropData>();
            for (int i = 0; i < data.CropDatas.Count; ++i)
            {
                CropData newData = new CropData();
                newData.Load(data.CropDatas[i]);
                m_CropData.Add(data.CropDataPositions[i], newData);
                UpdateCropVisual(data.CropDataPositions[i]);
                if (!m_HarvestEffectPool.ContainsKey(newData.GrowingCrop))
                {
                    InitHarvestEffect(newData.GrowingCrop);
                }
            }
        }
    }
    [Serializable]
    /// <summary>
    /// 地形数据保存结构
    /// </summary>
    public struct TerrainDataSave
    {
        // 地面数据位置列表
        public List<Vector3Int> GroundDataPositions;
        // 地面数据列表
        public List<TerrainManager.GroundData> GroundDatas;
        // 作物数据位置列表
        public List<Vector3Int> CropDataPositions;
        // 作物保存数据列表
        public List<TerrainManager.CropData.SaveData> CropDatas;
    }
}