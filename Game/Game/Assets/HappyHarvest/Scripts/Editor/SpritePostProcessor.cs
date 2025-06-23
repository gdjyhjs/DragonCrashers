#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

/// <summary>
/// ������Դ�������������Զ�������Ԥ����Ĳ����滻Ϊָ������
/// </summary>
public class SpritePostprocessor : AssetPostprocessor
{
    // Ĭ�Ͼ����������
    private const string SpritesDefaultMaterialName = "Sprites-Default";
    // �Զ������·����URP������ղ��ʣ�
    private const string CustomMaterialPath = "Packages/com.unity.render-pipelines.universal/Runtime/Materials/Sprite-Lit-Default.mat";

    /// <summary>
    /// Ԥ���嵼����������Զ��滻����
    /// </summary>
    void OnPostprocessPrefab(GameObject prefab)
    {
        // �����Զ������
        Material customMaterial = AssetDatabase.LoadAssetAtPath<Material>(CustomMaterialPath);

        if (customMaterial == null)
        {
            Debug.LogWarning($"δ��·�� {CustomMaterialPath} �ҵ��Զ������");
            return;
        }

        // ���Ԥ������������Ⱦ��
        Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
        bool materialChanged = false;

        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++)
            {
                // �滻Ĭ�Ͼ������Ϊ�Զ������
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
            Debug.Log($"��ΪԤ���� {assetPath} ���� Sprite-Lit ����");
        }
    }
}
#endif