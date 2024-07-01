using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EntryTest : MonoBehaviour
{
    public ScrollRect scrollRect;
    public GameObject buttonPrefab; // 预制的按钮
    
    public List<int> sceneIndexList = new List<int>();

    private void Start()
    {
        // 确保 ScrollRect 和 Button 预制体已经在 Inspector 中分配
        if (scrollRect == null || buttonPrefab == null)
        {
            Debug.LogError("ScrollRect or ButtonPrefab is not assigned.");
            return;
        }
        
        for (int i = 0; i <sceneIndexList.Count; i++)
        {
            CreateButton(sceneIndexList[i]);
        }
    }

    void CreateButton(int sceneIndex)
    {
        // 实例化按钮预制体
        Button newButton = Instantiate(buttonPrefab).GetComponent<Button>();

        // 设置按钮的父对象为 ScrollRect 的 Content 对象
        newButton.transform.SetParent(scrollRect.content, false);

        // 设置按钮的文本
        TextMeshPro buttonTextComponent = newButton.GetComponentInChildren<TextMeshPro>();
        if (buttonTextComponent != null)
        {
            buttonTextComponent.text = $"{sceneIndex}";
        }

        // 添加按钮点击事件
        newButton.onClick.AddListener(() => OnButtonClick(sceneIndex));
    }

    void OnButtonClick(int sceneIndex)
    {
        LoadSceneAsyncByIdx(sceneIndex);
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