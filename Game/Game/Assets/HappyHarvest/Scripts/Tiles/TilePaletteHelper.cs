using System;
namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// 图块调色板辅助工具，用于在编辑器中显示图块图标
    /// </summary>
    [ExecuteInEditMode]
    public class TilePaletteHelper : MonoBehaviour
    {
        private Tilemap tilemap;
        private TileBase[] tiles;

#if UNITY_EDITOR
        private void Start()
        {
            // 获取子对象中的 Tilemap 组件
            tilemap = GetComponentInChildren<Tilemap>();
            // 压缩图块地图的边界以优化性能
            tilemap.CompressBounds();
        }

        private void OnDrawGizmos()
        {
            if (tilemap == null)
                return;

            // 获取图块地图的单元格边界
            var bounds = tilemap.cellBounds;
            var boundsSize = bounds.size.x * bounds.size.y * bounds.size.z;
            // 调整图块数组大小以匹配边界内的单元格数量
            if (tiles == null || boundsSize != tiles.Length)
                Array.Resize(ref tiles, boundsSize);
            // 非分配方式获取图块块数据
            tilemap.GetTilesBlockNonAlloc(bounds, tiles);

            var i = 0;
            // 遍历边界内的所有单元格位置
            foreach (var position in bounds.allPositionsWithin)
            {
                var tile = tiles[i++];
                if (tile == null)
                    continue;

                // 跳过已有精灵的图块
                if (tilemap.GetSprite(position) != null)
                    continue;

                // 将单元格位置转换为局部坐标并添加图块锚点偏移
                var localPosition = tilemap.CellToLocalInterpolated(position + tilemap.tileAnchor);
                // 在场景中绘制图块类型对应的图标
                Gizmos.DrawIcon(localPosition, TilePaletteIconsPreference.GetTexturePath(tile.GetType()));
            }
        }
#endif
    }
}
