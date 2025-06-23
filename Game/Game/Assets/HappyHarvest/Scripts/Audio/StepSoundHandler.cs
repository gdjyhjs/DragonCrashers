using System;
using System.Collections;
using System.Collections.Generic;
using HappyHarvest;
using Template2DCommon;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace HappyHarvest
{
    /// <summary>
    /// �����߶����ڼ䲥������Ų������������������ҵ�Animator���λ��ͬһGameObject�ϣ�
    /// ��Ϊ����Ҫ�����������߶�����PlayStepSound�¼���
    /// ����һ����Ƭ����Ч��ӳ���б��ɸ�����ҽ��µ���Ƭ���Ͳ��Ų�ͬ����Ч��
    /// ע�⣺���ڼ�����������Ƭ��Tilemap�������WalkableSurface�����
    /// </summary>
    public class StepSoundHandler : MonoBehaviour
    {
        [Serializable]
        public class TileSoundMapping
        {
            // ��������Ƭ����
            public TileBase[] Tiles;
            // ��Ӧ�ĽŲ�������
            public AudioClip[] StepSounds;
        }

        // Ĭ�ϽŲ�������
        public AudioClip[] DefaultStepSounds;
        // ��Ƭ��Чӳ������
        public TileSoundMapping[] SoundMappings;

        // ��Ƭ���Ų�����ӳ���ֵ�
        private Dictionary<TileBase, AudioClip[]> m_Mapping = new();

        void Start()
        {
            // ��ʼ����Ƭ����Ч��ӳ���ϵ
            foreach (var mapping in SoundMappings)
            {
                foreach (var tile in mapping.Tiles)
                {
                    m_Mapping[tile] = mapping.StepSounds;
                }
            }
        }

        // �ɽ�ɫ���߶����Ķ����¼�����
        public void PlayStepSound()
        {
            // ��ȡ��ҽ��µ���Ƭ����
            var underCell = GameManager.Instance.WalkSurfaceTilemap.WorldToCell(transform.position);
            // ��ȡ��Ƭ����
            var tile = GameManager.Instance.WalkSurfaceTilemap.GetTile(underCell);

            // ������Ƭ���Ͳ��Ŷ�Ӧ������Ų���
            SoundManager.Instance.PlaySFXAt(transform.position,
                (tile != null && m_Mapping.ContainsKey(tile))
                    ? GetRandomEntry(m_Mapping[tile])
                    : GetRandomEntry(DefaultStepSounds), false);
        }

        // �������������ȡһ����ƵƬ��
        AudioClip GetRandomEntry(AudioClip[] clips)
        {
            return clips[Random.Range(0, clips.Length)];
        }
    }
}