using System;

namespace UnityEngine.Tilemaps
{
    /// <summary>
    /// ͼ���ɫ��ͼ��ƫ�����ã����ڻ�ȡͼ�����Ͷ�Ӧ��ͼ��·��
    /// </summary>
    internal static class TilePaletteIconsPreference
    {
        /// <summary>
        /// ��ȡָ��ͼ�����͵�ͼ������·��
        /// </summary>
        /// <param name="tileType">ͼ������</param>
        /// <returns>ͼ������·�������������Ч��ͼ�������򷵻ؿ��ַ���</returns>
        public static string GetTexturePath(Type tileType)
        {
            // ��������Ƿ�̳���TileBase
            if (!tileType.IsSubclassOf(typeof(TileBase)))
                return String.Empty;

            // ����Ĭ�ϵ�ͼ��ͼ��·��
            return "UnityEngine/Tilemaps/Tile Icon";
        }
    }
}