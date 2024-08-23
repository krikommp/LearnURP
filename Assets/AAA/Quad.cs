using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Quad : MonoBehaviour
{
    public Material m_material;

    public GameObject m_quad;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (m_quad != null)
        {
            DestroyImmediate(m_quad);
        }
        
        // 创建一个 Mesh
        Mesh mesh = new Mesh();

        // 设置顶点
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(-410.1875f, -682.991f, -1.38477f);
        vertices[1] = new Vector3(-410.1875f, 701.99994f, -1.38477f);
        vertices[2] = new Vector3(410.1875f, 701.99994f, -1.38477f);
        vertices[3] = new Vector3(410.1875f, -682.992f, -1.38477f);
        mesh.vertices = vertices;

        // 设置三角形
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;
        mesh.triangles = triangles;

        // 设置顶点色
        Color[] colors = new Color[4];
        colors[0] = new Color(1, 1, 1, 0.00392f);
        colors[1] = new Color(1, 1, 1, 0.00392f);
        colors[2] = new Color(1, 1, 1, 0.00392f);
        colors[3] = new Color(1, 1, 1, 0.00392f);
        mesh.colors = colors;

        // 设置 UV
        Vector2[] uv = new Vector2[4];
        uv[0] = new Vector2(0.0437f, 0.0116f);
        uv[1] = new Vector2(0.0437f, 0.97656f);
        uv[2] = new Vector2(0.95313f, 0.97656f);
        uv[3] = new Vector2(0.95313f, 0.0116f);
        mesh.uv = uv;

        // 创建一个 GameObject 并将 Mesh 和 Material 添加到 MeshRenderer 组件中
        m_quad = new GameObject("Quad");
        MeshFilter meshFilter = m_quad.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = m_quad.AddComponent<MeshRenderer>();
        meshRenderer.material = m_material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
