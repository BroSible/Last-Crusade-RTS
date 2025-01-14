using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracksGrid : MonoBehaviour
{
    public float gridSize = 1f;          // Размер ячейки сетки
    public int gridWidth = 10;           // Ширина сетки (в ячейках)
    public int gridHeight = 10;          // Высота сетки (в ячейках)
    public float gridRadius = 5f;        // Радиус зоны вокруг объекта, где можно ставить юнитов

    private void OnDrawGizmos()
    {
        // Позиция объекта, на котором висит этот скрипт
        Vector3 objectPosition = transform.position;

        Gizmos.color = Color.green;

        // Рисуем сетку вокруг объекта
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Вычисляем координаты центра ячейки
                Vector3 position = new Vector3(x * gridSize, 0f, z * gridSize) + objectPosition;

                // Проверяем, находится ли ячейка внутри радиуса
                if (Vector3.Distance(position, objectPosition) <= gridRadius)
                {
                    Gizmos.DrawWireCube(position, new Vector3(gridSize, 0.1f, gridSize));
                }
            }
        }
    }

    // Проверка, можно ли разместить юнита на указанной позиции
    public bool CanPlaceUnit(Vector3 position)
    {
        Vector3 objectPosition = transform.position;
        Vector3 offsetPosition = position - objectPosition;
        float distanceToCenter = offsetPosition.magnitude;

        // Проверяем, что позиция внутри радиуса и не выходит за пределы сетки
        if (distanceToCenter <= gridRadius)
        {
            return true;
        }

        return false;
    }
}
