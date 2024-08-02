using UnityEngine;

public class RotateAroundPoint : MonoBehaviour
{
    public float speed = 10f; // 旋转速度
    public Vector3 point = new Vector3(0, 0, 0); // 旋转中心点

    private void FixedUpdate()
    {
        // 计算旋转角度
        float angle = speed * Time.fixedDeltaTime;

        // 绕指定点旋转
        transform.RotateAround(point, Vector3.up, angle);
    }
}