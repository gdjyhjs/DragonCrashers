using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;

namespace HappyHarvest
{
    /// <summary>
    /// ��������ֲ����ĵ�����ص��������ݡ����浥Ԫ��������״̬�����ݣ�
    /// �������ͽ�ˮ�Ȳ���ʱ�Ĵ�ש�л������ˡ�
    /// </summary>
    public class TerrainManager : MonoBehaviour
    {
        [System.Serializable]
        public class GroundData
        {
            // ��ˮ����ʱ�䣨�룩
            public const float WaterDuration = 60 * 1.0f;
            // ��ˮ��ʱ��
            public float WaterTimer;
        }
        public class CropData
        {
            [Serializable]
            public struct SaveData
            {
                // ���� ID
                public string CropId;
                // ��ǰ�����׶�
                public int Stage;
                // �������ȱ���
                public float GrowthRatio;
                // ������ʱ��
                public float GrowthTimer;
                // �ջ����
                public int HarvestCount;
                // ��ή��ʱ��
                public float DyingTimer;
            }
            // ��������������
            public Crop GrowingCrop = null;
            // ��ǰ�����׶�
            public int CurrentGrowthStage = 0;
            // �������ȱ���
            public float GrowthRatio = 0.0f;
            // ������ʱ��
            public float GrowthTimer = 0.0f;
            // �ջ����
            public int HarvestCount = 0;
            // ��ή��ʱ��
            public float DyingTimer;
            // �Ƿ�����������ջ�
            public bool HarvestDone => HarvestCount == GrowingCrop.NumberOfHarvest;
            // ��ʼ����������
            public void Init()
            {
                GrowingCrop = null;
                GrowthRatio = 0.0f;
                GrowthTimer = 0.0f;
                CurrentGrowthStage = 0;
                HarvestCount = 0;
                DyingTimer = 0.0f;
            }
            // �ջ�����
            public Crop Harvest()
            {
                var crop = GrowingCrop;
                HarvestCount += 1;
                CurrentGrowthStage = GrowingCrop.StageAfterHarvest;
                GrowthRatio = CurrentGrowthStage / (float)GrowingCrop.GrowthStagesTiles.Length;
                GrowthTimer = GrowingCrop.GrowthTime * GrowthRatio;
                return crop;
            }
            // ������������
            public void Save(ref SaveData data)
            {
                data.Stage = CurrentGrowthStage;
                data.CropId = GrowingCrop.Key;
                data.DyingTimer = DyingTimer;
                data.GrowthRatio = GrowthRatio;
                data.GrowthTimer = GrowthTimer;
                data.HarvestCount = HarvestCount;
            }
            // ������������
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
        // ��������
        public Grid Grid;
        // �����ש��ͼ
        public Tilemap GroundTilemap;
        // �����ש��ͼ
        public Tilemap CropTilemap;
        [Header("��ˮ����")]
        // ��ˮ��ש��ͼ
        public Tilemap WaterTilemap;
        // �ѽ�ˮ�Ĵ�ש
        public TileBase WateredTile;
        [Header("��������")]
        // �ɷ����Ĵ�ש
        public TileBase TilleableTile;
        // �ѷ����Ĵ�ש
        public TileBase TilledTile;
        // ����Ч��Ԥ����
        public VisualEffect TillingEffectPrefab;
        // ���������ֵ䣨���� -> �������ݣ�
        private Dictionary<Vector3Int, GroundData> m_GroundData = new();
        // ���������ֵ䣨���� -> �������ݣ�
        private Dictionary<Vector3Int, CropData> m_CropData = new();
        // �ջ�Ч���أ����� -> Ч���б�
        private Dictionary<Crop, List<VisualEffect>> m_HarvestEffectPool = new();
        // ����Ч����
        private List<VisualEffect> m_TillingEffectPool = new();
        // ���ָ��λ���Ƿ�ɷ���
        public bool IsTillable(Vector3Int target)
        {
            return GroundTilemap.GetTile(target) == TilleableTile;
        }
        // ���ָ��λ���Ƿ����ֲ
        public bool IsPlantable(Vector3Int target)
        {
            return IsTilled(target) && !m_CropData.ContainsKey(target);
        }
        // ���ָ��λ���Ƿ��ѷ���
        public bool IsTilled(Vector3Int target)
        {
            return m_GroundData.ContainsKey(target);
        }
        // ��ָ��λ�÷���
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
        // ��ָ��λ����ֲ����
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
        // ��ʼ�������ջ�Ч����
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
        // ��ָ��λ�ý�ˮ
        public void WaterAt(Vector3Int target)
        {
            var groundData = m_GroundData[target];
            groundData.WaterTimer = GroundData.WaterDuration;
            WaterTilemap.SetTile(target, WateredTile);
        }
        // ��ָ��λ���ջ�����
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
        // ��ȡָ��λ�õ���������
        public CropData GetCropDataAt(Vector3Int target)
        {
            m_CropData.TryGetValue(target, out var data);
            return data;
        }
        // ����ָ��λ�õ����������׶�
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
        // ���������Ӿ�Ч��
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
        // �����������
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
        // ���ص�������
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
            // �����������Ч�����Ա����¼�����Ч��
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
    /// �������ݱ���ṹ
    /// </summary>
    public struct TerrainDataSave
    {
        // ��������λ���б�
        public List<Vector3Int> GroundDataPositions;
        // ���������б�
        public List<TerrainManager.GroundData> GroundDatas;
        // ��������λ���б�
        public List<Vector3Int> CropDataPositions;
        // ���ﱣ�������б�
        public List<TerrainManager.CropData.SaveData> CropDatas;
    }
}