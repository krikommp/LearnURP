using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class TestLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Addressables.LoadAssetAsync<GameObject>("Assets/AAA/Cube.prefab").Completed += OnCompleted;
    }

    private void OnCompleted(AsyncOperationHandle<GameObject> obj)
    {
        Debug.Log(obj.Result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
