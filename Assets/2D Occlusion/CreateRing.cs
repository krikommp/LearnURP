using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class CreateRing : MonoBehaviour
{
    private MeshFilter m_meshFilter;

    private void OnEnable()
    {
        m_meshFilter = GetComponent<MeshFilter>();

        StartCoroutine(CreateMesh());
    }

    IEnumerator CreateMesh()
    {
        yield return new WaitForNextFrameUnit();

        m_meshFilter.mesh = CreateTorus3DMesh(3, 3, 32, 0.5f);
    }

    /// <summary>
    /// 创建一个3维圆环体Mesh。
    /// </summary>
    /// <param name="outerRadius">外半径。</param>
    /// <param name="innerRadius">内半径。</param>
    /// <param name="numSegments">圆环体段数。值越大，圆环体越圆润。</param>
    /// <param name="numSides">圆环体截面边数。值越大，圆环体截面越圆润。</param>
    /// <returns></returns>
    public static Mesh CreateTorus3DMesh(float outerRadius, float innerRadius, int segments, float height)
    {
        // 创建环状网格
        Mesh mesh = new Mesh();
        mesh.name = "Ring";

        int vertexCount = segments * 4;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];

        float angleStep = 2.0f * Mathf.PI / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            // Bottom vertices
            vertices[i * 4] = new Vector3(cos * innerRadius, 0f, sin * innerRadius);
            vertices[i * 4 + 1] = new Vector3(cos * outerRadius, 0f, sin * outerRadius);
            // Top vertices
            vertices[i * 4 + 2] = new Vector3(cos * innerRadius, height, sin * innerRadius);
            vertices[i * 4 + 3] = new Vector3(cos * outerRadius, height, sin * outerRadius);

            // Normals (pointing outwards)
            normals[i * 4] = new Vector3(cos, 0f, sin);
            normals[i * 4 + 1] = new Vector3(cos, 0f, sin);
            normals[i * 4 + 2] = new Vector3(cos, 0f, sin);
            normals[i * 4 + 3] = new Vector3(cos, 0f, sin);

            // UVs
            uv[i * 4] = new Vector2(i / (float)segments, 0f);
            uv[i * 4 + 1] = new Vector2((i + 1) / (float)segments, 0f);
            uv[i * 4 + 2] = new Vector2(i / (float)segments, 1f);
            uv[i * 4 + 3] = new Vector2((i + 1) / (float)segments, 1f);
        }

        int[] triangles = new int[segments * 12];
        for (int i = 0; i < segments; i++)
        {
            int vertIndex = i * 4;
            int triIndex = i * 12;

            // Inner surface
            triangles[triIndex] = vertIndex;
            triangles[triIndex + 1] = vertIndex + 2;
            triangles[triIndex + 2] = (vertIndex + 4) % vertexCount;

            triangles[triIndex + 3] = vertIndex + 2;
            triangles[triIndex + 4] = (vertIndex + 6) % vertexCount;
            triangles[triIndex + 5] = (vertIndex + 4) % vertexCount;

            // Outer surface
            // triangles[triIndex + 6] = vertIndex + 1;
            // triangles[triIndex + 7] = (vertIndex + 5) % vertexCount;
            // triangles[triIndex + 8] = vertIndex + 3;
            //
            // triangles[triIndex + 9] = vertIndex + 3;
            // triangles[triIndex + 10] = (vertIndex + 5) % vertexCount;
            // triangles[triIndex + 11] = (vertIndex + 7) % vertexCount;
        }

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        ;

        return mesh;
    }
}