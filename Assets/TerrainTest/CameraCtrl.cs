using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public float moveSpeed = 10f; // 移动速度
    public float zoomSpeed = 1f; // 缩放速度

    // 按钮引用
    public bool moveUp = false;
    public bool moveDown = false;
    public bool moveLeft = false;
    public bool moveRight = false;
    public bool zoomIn = false;
    public bool zoomOut = false;

    void Update()
    {
        // 初始化移动向量
        Vector3 movement = Vector3.zero;
        float orthoSize = 0.0f;

        // 根据按钮状态更新移动向量
        if (moveUp)
        {
            movement += new Vector3(0, 0, 1);
        }
        if (moveDown)
        {
            movement += new Vector3(0, 0, -1);
        }
        if (moveLeft)
        {
            movement += new Vector3(-1, 0, 0);
        }
        if (moveRight)
        {
            movement += new Vector3(1, 0, 0);
        }

        if (zoomIn)
        {
            orthoSize = -1;
        }
        if (zoomOut)
        {
            orthoSize = 1;
        }

        // 根据移动速度移动相机
        if (Camera.main != null) Camera.main.transform.Translate(movement * moveSpeed * Time.deltaTime, Space.World);
        if (Camera.main != null) 
        {
            Camera.main.orthographicSize += orthoSize * zoomSpeed * Time.deltaTime;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, 5f, 8f);
        }
    }

    // 这些方法可以绑定到 UI 按钮的 onClick 事件上
    public void OnMoveUpButtonDown()
    {
        moveUp = true;
    }

    public void OnMoveUpButtonUp()
    {
        moveUp = false;
    }

    public void OnMoveDownButtonDown()
    {
        moveDown = true;
    }

    public void OnMoveDownButtonUp()
    {
        moveDown = false;
    }

    public void OnMoveLeftButtonDown()
    {
        moveLeft = true;
    }

    public void OnMoveLeftButtonUp()
    {
        moveLeft = false;
    }

    public void OnMoveRightButtonDown()
    {
        moveRight = true;
    }

    public void OnMoveRightButtonUp()
    {
        moveRight = false;
    }
    
    public void OnZoomInButtonDown()
    {
        zoomIn = true;
    }
    
    public void OnZoomInButtonUp()
    {
        zoomIn = false;
    }
    
    public void OnZoomOutButtonDown()
    {
        zoomOut = true;
    }
    
    public void OnZoomOutButtonUp()
    {
        zoomOut = false;
    }
}
