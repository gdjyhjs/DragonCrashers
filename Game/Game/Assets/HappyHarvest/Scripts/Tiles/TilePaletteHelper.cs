using System;
namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// ͼ���ɫ�帨�����ߣ������ڱ༭������ʾͼ��ͼ��
    /// </summary>
    [ExecuteInEditMode]
    public class TilePaletteHelper : MonoBehaviour
    {
        private Tilemap tilemap;
        private TileBase[] tiles;

#if UNITY_EDITOR
        private void Start()
        {
            // ��ȡ�Ӷ����е� Tilemap ���
            tilemap = GetComponentInChildren<Tilemap>();
            // ѹ��ͼ���ͼ�ı߽����Ż�����
            tilemap.CompressBounds();
        }

        private void OnDrawGizmos()
        {
            if (tilemap == null)
                return;

            // ��ȡͼ���ͼ�ĵ�Ԫ��߽�
            var bounds = tilemap.cellBounds;
            var boundsSize = bounds.size.x * bounds.size.y * bounds.size.z;
            // ����ͼ�������С��ƥ��߽��ڵĵ�Ԫ������
            if (tiles == null || boundsSize != tiles.Length)
                Array.Resize(ref tiles, boundsSize);
            // �Ƿ��䷽ʽ��ȡͼ�������
            tilemap.GetTilesBlockNonAlloc(bounds, tiles);

            var i = 0;
            // �����߽��ڵ����е�Ԫ��λ��
            foreach (var position in bounds.allPositionsWithin)
            {
                var tile = tiles[i++];
                if (tile == null)
                    continue;

                // �������о����ͼ��
                if (tilemap.GetSprite(position) != null)
                    continue;

                // ����Ԫ��λ��ת��Ϊ�ֲ����겢���ͼ��ê��ƫ��
                var localPosition = tilemap.CellToLocalInterpolated(position + tilemap.tileAnchor);
                // �ڳ����л���ͼ�����Ͷ�Ӧ��ͼ��
                Gizmos.DrawIcon(localPosition, TilePaletteIconsPreference.GetTexturePath(tile.GetType()));
            }
        }
#endif
    }
}
