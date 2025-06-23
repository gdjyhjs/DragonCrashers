using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace HappyHarvest
{
    /// <summary>
    /// ���������ӵ�����������߱���Ĵ�ש��ͼ�ϡ�����ϵͳʹ�ô������ȷ��
    /// �������ʱӦ������Щ������
    /// </summary>
    [RequireComponent(typeof(Tilemap))]
    public class WalkableSurface : MonoBehaviour
    {
        private void Awake()
        {
            // ����ǰ��ש��ͼ�����ֵ����Ϸ�����������߱�������
            GameManager.Instance.WalkSurfaceTilemap = GetComponent<Tilemap>();
        }
    }
}