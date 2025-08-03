using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.WorldGenerator.Conf
{
    public class ConfBuild : ScriptableObject
    {
        [MenuItem("工具/我的工具/测试C#代码")]
        static void DoIt()
        {
            Debug.Log("执行了代码");
        }
    }
}