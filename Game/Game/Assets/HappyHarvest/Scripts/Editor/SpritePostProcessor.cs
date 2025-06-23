#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

/// <summary>
/// 精灵资源后处理器，用于自动将精灵预制体的材质替换为指定材质
/// </summary>
public class SpritePostprocessor : AssetPostprocessor
{
    // 默认精灵材质名称
    private const string SpritesDefaultMaterialName = "Sprites-Default";
    // 自定义材质路径（URP精灵光照材质）
    private const string CustomMaterialPath = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Lit-Default.mat";

    /// <summary>
    /// 预制体导入后处理方法，自动替换材质
    /// </summary>
    void OnPostprocessPrefab(GameObject prefab)
    {
        // 加载自定义材质
        Material customMaterial = AssetDatabase.LoadAssetAtPath<Material>(CustomMaterialPath);

        if (customMaterial == null)
        {
            Debug.LogWarning($"未在路径 {CustomMaterialPath} 找到自定义材质");
            return;
        }

        // 检查预制体中所有渲染器
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
        bool materialChanged = false;

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                // 替换默认精灵材质为自定义材质
                if (materials[i] != null && materials[i].name == SpritesDefaultMaterialName)
                {
                    materials[i] = customMaterial;
                    materialChanged = true;
                }
            }

            if (materialChanged)
            {
                renderer.sharedMaterials = materials;
            }
        }

        if (materialChanged)
        {
            Debug.Log($"已为预制体 {assetPath} 分配 Sprite-Lit 材质");
        }
    }
}
#endif