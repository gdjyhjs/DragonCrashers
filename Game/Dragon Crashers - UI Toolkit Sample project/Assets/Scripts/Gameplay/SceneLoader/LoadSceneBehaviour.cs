using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Inspector;
using UnityEngine.SceneManagement;

// 加载场景行为类
public class LoadSceneBehaviour : MonoBehaviour
{

    public SceneField sceneToLoad; // 要加载的场景

    // 加载场景
    public void LoadScene()
    {
        NextSceneLoader sceneLoader = new NextSceneLoader();
        sceneLoader.LoadNextScene(sceneToLoad);
    }

}