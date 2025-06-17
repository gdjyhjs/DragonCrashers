using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Utilities.Inspector
{
    [System.Serializable]
    public class SceneField
    {
        // 场景资源对象
        [SerializeField] private Object sceneAsset;
        // 场景名称
        [SerializeField] private string sceneName = "";

        // 场景名称属性
        public string SceneName
        {
            get { return sceneName; }
        }

        // 隐式转换为字符串，使其能与现有Unity方法（LoadLevel/LoadScene）配合使用
        public static implicit operator string(SceneField sceneField)
        {
            return sceneField.SceneName;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            // 查找场景资源的序列化属性
            var sceneAsset = property.FindPropertyRelative("sceneAsset");
            // 查找场景名称的序列化属性
            var sceneName = property.FindPropertyRelative("sceneName");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (sceneAsset != null)
            {
                EditorGUI.BeginChangeCheck();
                var value = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);
                if (EditorGUI.EndChangeCheck())
                {
                    sceneAsset.objectReferenceValue = value;
                    if (sceneAsset.objectReferenceValue != null)
                    {
                        // 获取场景资源的路径
                        var scenePath = AssetDatabase.GetAssetPath(sceneAsset.objectReferenceValue);
                        var assetsIndex = scenePath.IndexOf("Assets", StringComparison.Ordinal) + 7;
                        var extensionIndex = scenePath.LastIndexOf(".unity", StringComparison.Ordinal);
                        // 提取场景名称
                        scenePath = scenePath.Substring(assetsIndex, extensionIndex - assetsIndex);
                        sceneName.stringValue = scenePath;
                    }
                }
            }
            EditorGUI.EndProperty();
        }
    }
#endif
}