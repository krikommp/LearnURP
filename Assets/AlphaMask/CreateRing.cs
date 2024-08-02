using System;
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
    }

    private void OnValidate()
    {
        m_meshFilter.mesh = CreateTorus3DMesh(3, 4, 32, 16);
    }

    /// <summary>
    /// 创建一个3维圆环体Mesh。
    /// </summary>
    /// <param name="outerRadius">外半径。</param>
    /// <param name="innerRadius">内半径。</param>
    /// <param name="numSegments">圆环体段数。值越大，圆环体越圆润。</param>
    /// <param name="numSides">圆环体截面边数。值越大，圆环体截面越圆润。</param>
    /// <returns></returns>
    public static Mesh CreateTorus3DMesh(float outerRadius, float innerRadius, int numSegments = 32, int numSides = 16)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Torus";

        Vector3[] vertices = new Vector3[(numSegments + 1) * (numSides + 1)];
        Vector3[] normals = new Vector3[(numSegments + 1) * (numSides + 1)];
        Vector2[] uv = new Vector2[(numSegments + 1) * (numSides + 1)];
        int[] triangles = new int[numSegments * numSides * 6];

        float angleStep = 2 * Mathf.PI / numSegments;
        float sideAngleStep = 2 * Mathf.PI / numSides;

        float avgRadius = (outerRadius + innerRadius) / 2; // 平均半径
        float secRadius = (outerRadius - innerRadius) / 2; // 截面半径

        int vertexIndex = 0;
        for (int i = 0; i <= numSegments; i++)
        {
            float segAngle = i * angleStep; // 圆环体分段角

            for (int j = 0; j <= numSides; j++)
            {
                float sideAngle = j * sideAngleStep; // 圆环体截面角

                // 从right方向开始生成顶点
                Vector3 vertex = new Vector3
                {
                    x = (avgRadius + secRadius * Mathf.Cos(sideAngle)) * Mathf.Cos(segAngle),
                    z = (avgRadius + secRadius * Mathf.Cos(sideAngle)) * Mathf.Sin(segAngle),
                    y = secRadius * Mathf.Sin(sideAngle)
                };

                Vector3 secCenter = new Vector3
                {
                    x = avgRadius * Mathf.Cos(segAngle),
                    z = avgRadius * Mathf.Sin(segAngle),
                    y = 0,
                };

                vertices[vertexIndex] = vertex;
                normals[vertexIndex] = (vertex - secCenter).normalized;
                uv[vertexIndex] = new Vector2((float)i / numSegments, (float)j / numSides);

                vertexIndex++;
            }
        }

        int triangleIndex = 0;
        for (int i = 0; i < numSegments; i++)
        {
            for (int j = 0; j < numSides; j++)
            {
                int topLeft = (i + 1) * (numSides + 1) + j;
                int topRight = topLeft + 1;
                int bottomLeft = i * (numSides + 1) + j;
                int bottomRight = bottomLeft + 1;

                triangles[triangleIndex] = topLeft;
                triangles[triangleIndex + 1] = bottomLeft;
                triangles[triangleIndex + 2] = topRight;
                triangles[triangleIndex + 3] = topRight;
                triangles[triangleIndex + 4] = bottomLeft;
                triangles[triangleIndex + 5] = bottomRight;

                triangleIndex += 6;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        return mesh;
    }
}