using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace HappyHarvest
{
    /// <summary>
    /// ������ͼ������˵�������������ͼ�飬���������ƥ�����ͼ�鲻ͬ������ͼ�����
    /// �й�ʾ������μ�����ͼ�顣
    /// </summary>
    [CreateAssetMenu]
    public class AdjacentRuleTile : RuleTile<AdjacentRuleTile.Neighbor>
    {
        // ����ͼ���ھӹ�����
        public class Neighbor : RuleTile.TilingRule.Neighbor
        {
            // ����ͼ������ʶ
            public const int Adjacent = 3;
        }

        // ����ͼ�����飨��ƥ�������ͼ�飩
        public TileBase[] AdjacentTiles;

        /// <summary>
        /// ���ͼ������Ƿ�ƥ��
        /// </summary>
        /// <param name="neighbor">�ھӹ�������</param>
        /// <param name="other">����ͼ��</param>
        /// <returns>�Ƿ�ƥ�����</returns>
        public override bool RuleMatch(int neighbor, TileBase other)
        {
            // ��������ͼ�����
            switch (neighbor)
            {
                case Neighbor.Adjacent:
                    // �������ͼ���Ƿ������������ͼ��������
                    return AdjacentTiles.Contains(other);
            }

            // ���û��෽��������������
            return base.RuleMatch(neighbor, other);
        }
    }
}