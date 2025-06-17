using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Inspector;
using UnityEngine.SceneManagement;

// ���س�����Ϊ��
public class LoadSceneBehaviour : MonoBehaviour
{

    public SceneField sceneToLoad; // Ҫ���صĳ���

    // ���س���
    public void LoadScene()
    {
        NextSceneLoader sceneLoader = new NextSceneLoader();
        sceneLoader.LoadNextScene(sceneToLoad);
    }

}