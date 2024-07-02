using UnityEngine;
using UnityEditor;

public class GridDuplicatorEditor : EditorWindow
{
    private GameObject centerObject;
    private float cellWidth = 1.0f;
    private float cellHeight = 1.0f;

    [MenuItem("Tools/Grid Duplicator")]
    public static void ShowWindow()
    {
        GetWindow<GridDuplicatorEditor>("Grid Duplicator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Duplicator Settings", EditorStyles.boldLabel);

        centerObject = (GameObject)EditorGUILayout.ObjectField("Center Object", centerObject, typeof(GameObject), true);
        cellWidth = EditorGUILayout.FloatField("Cell Width", cellWidth);
        cellHeight = EditorGUILayout.FloatField("Cell Height", cellHeight);

        if (GUILayout.Button("Duplicate in Grid"))
        {
            DuplicateInGrid();
        }

        if (GUILayout.Button("Delete Duplicates"))
        {
            DeleteDuplicates();
        }
    }

    private void DuplicateInGrid()
    {
        if (centerObject == null)
        {
            Debug.LogError("Center object is not assigned.");
            return;
        }

        // 获取中心对象的位置
        Vector3 centerPosition = centerObject.transform.position;

        // 复制并排列对象
        for (int row = -1; row <= 1; row++)
        {
            for (int col = -1; col <= 1; col++)
            {
                // 跳过中心位置
                if (row == 0 && col == 0)
                    continue;

                // 计算新对象的位置
                Vector3 newPosition = new Vector3(
                    centerPosition.x + col * cellWidth,
                    centerPosition.y,
                    centerPosition.z + row * cellHeight
                );

                // 复制对象
                GameObject newObject = Instantiate(centerObject, newPosition, Quaternion.identity);

                // 可选：给新对象命名以便识别
                newObject.name = centerObject.name + "_Copy_" + row + "_" + col;

                // 记录操作以便撤销
                Undo.RegisterCreatedObjectUndo(newObject, "Duplicate in Grid");
            }
        }

        // 记录整个操作，以便撤销
        Undo.RegisterCompleteObjectUndo(centerObject, "Duplicate in Grid");
    }

    private void DeleteDuplicates()
    {
        if (centerObject == null)
        {
            Debug.LogError("Center object is not assigned.");
            return;
        }

        // 获取所有场景中的 GameObject
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // 删除所有名字包含 "_Copy_" 的对象
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains(centerObject.name + "_Copy_"))
            {
                // 记录操作以便撤销
                Undo.DestroyObjectImmediate(obj);
            }
        }
    }
}