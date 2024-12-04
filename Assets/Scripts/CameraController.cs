using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f; // Скорость перемещения камеры
    public float panBorderThickness = 10f; // Толщина зоны у края экрана для перемещения
    public float scrollSpeed = 20f; // Скорость зума
    public float minY = 10f; // Минимальная высота камеры
    public float maxY = 50f; // Максимальная высота камеры

    void Update()
    {
        Vector3 pos = transform.position;

        // Векторы локальных осей камеры
        Vector3 forward = transform.forward; 
        Vector3 right = transform.right;

        // Проекция векторов на плоскость XZ (чтобы игнорировать наклон камеры)
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Перемещение камеры с помощью курсора у краёв экрана
        if (Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos += forward * panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.y <= panBorderThickness)
        {
            pos -= forward * panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos += right * panSpeed * Time.deltaTime;
        }
        if (Input.mousePosition.x <= panBorderThickness)
        {
            pos -= right * panSpeed * Time.deltaTime;
        }

        // Зум камеры
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}
