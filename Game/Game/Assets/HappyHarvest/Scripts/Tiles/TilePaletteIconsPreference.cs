using System;

namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// 图块调色板图标偏好设置，用于获取图块类型对应的图标路径
    /// </summary>
    internal static class TilePaletteIconsPreference
    {
        /// <summary>
        /// 获取指定图块类型的图标纹理路径
        /// </summary>
        /// <param name="tileType">图块类型</param>
        /// <returns>图标纹理路径，如果不是有效的图块类型则返回空字符串</returns>
        public static string GetTexturePath(Type tileType)
        {
            // 检查类型是否继承自TileBase
            if (!tileType.IsSubclassOf(typeof(TileBase)))
                return String.Empty;

            // 返回默认的图块图标路径
            return "UnityEngine/Tilemaps/Tile Icon";
        }
    }
}