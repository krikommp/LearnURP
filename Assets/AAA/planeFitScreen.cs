using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class planeFitScreen : MonoBehaviour
{
    public Camera m_camera;
    public float offset = 0.1f;
    
    void OnEnable()
    {
        if (m_camera == null)
        {
            Debug.LogError("planeFitScreen: 摄像机为空");
            return;
        }

        if (!m_camera.orthographic)
        {
            //摄像机是透视摄像机
            float pos = (m_camera.nearClipPlane + 0.01f);

            transform.position = m_camera.transform.position + m_camera.transform.forward * pos;
            transform.LookAt(m_camera.transform);
            transform.Rotate(90.0f, 0.0f, 0.0f);

            float h = (Mathf.Tan(m_camera.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) / 10.0f;

            transform.localScale = new Vector3(h * m_camera.aspect, 1.0f, h);
        }
        else
        {
            //摄像机必须是正交的
            float height = m_camera.orthographicSize * 2.0f;
            float width = height * m_camera.aspect;
            transform.localScale = new Vector3(width, height, 0.1f);

            var vector3 = transform.localPosition;
            vector3.z = offset;
            transform.localPosition = vector3;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
