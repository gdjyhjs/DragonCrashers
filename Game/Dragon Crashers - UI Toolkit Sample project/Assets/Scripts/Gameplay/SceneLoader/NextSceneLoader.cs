using UnityEngine;
using Utilities.Inspector;
using UnityEngine.SceneManagement;

// ��һ������������
public class NextSceneLoader
{
    // ������һ������
    public void LoadNextScene(SceneField sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}