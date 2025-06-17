#if UNITY_EDITOR

namespace UnityEditor.U2D.PSD
{
    // 自定义PSD导入器覆盖编辑器
    [CustomEditor(typeof(UnityEditor.U2D.PSD.PSDImporterOverride))]
    internal class PSDImporterOverrideEditor : PSDImporterEditor
    {
    }

}

#endif