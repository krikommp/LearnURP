using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Exit : MonoBehaviour
{
    public void OnButtonClick()
    {
        LoadSceneAsyncByIdx(0);
    }
    
    void LoadSceneAsyncByIdx(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneIndex));
    }

    private IEnumerator LoadSceneAsyncCoroutine(int sceneIndex)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncOperation.isDone)
        {
            // 可选：在这里更新进度条等
            Debug.Log("Loading progress: " + (asyncOperation.progress * 100) + "%");
            yield return null;
        }
    }
}
