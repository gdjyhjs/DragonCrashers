using UnityEngine;
using Utilities.Inspector;
using UnityEngine.SceneManagement;

// 下一个场景加载器
public class NextSceneLoader
{
    // 加载下一个场景
    public void LoadNextScene(SceneField sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}