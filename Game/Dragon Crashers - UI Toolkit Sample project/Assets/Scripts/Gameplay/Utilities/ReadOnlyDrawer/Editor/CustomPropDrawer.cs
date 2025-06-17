using UnityEditor;
using UnityEngine;
using Utilities.Inspector;

namespace Utilities.Editor
{
    /// <summary>
    /// 使字段在编辑器中变为只读。此功能的脚本应放在Unity编辑器文件夹之内。
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        // =============================================================================================================
        // 获取属性高度的方法
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
        // =============================================================================================================
        // 在GUI上绘制属性的方法
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
        // =============================================================================================================
    }
}