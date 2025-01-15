using System.Collections.Generic;
using UnityEngine;

public class BarracksGrid : MonoBehaviour
{
    [Header("Grid settings")]
    [SerializeField] private Vector2Int gridSize = new Vector2Int(5, 5); 
    [SerializeField] private float cellSize = 1f;                      
    [SerializeField] private Vector3 gridOffset = Vector3.zero; 

    public static BarracksGrid Instance { get; set; }   

    private List<Vector3> availablePositions = new();
    
    // Новая переменная для хранения состояния размещения юнита в сетке
    public bool isUnitInGrid = false;

    private void Start()
    {
        GeneratePlacementGrid();
    }

    private void GeneratePlacementGrid()
    {
        Vector3 origin = transform.position + gridOffset - new Vector3(gridSize.x / 2f * cellSize, 0, gridSize.y / 2f * cellSize);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                Vector3 pos = origin + new Vector3(x * cellSize, 0, z * cellSize);
                availablePositions.Add(pos);
            }
        }
    }

    public bool IsWithinPlacementZone(Vector3 position)
    {
        foreach (Vector3 pos in availablePositions)
        {
            if (Vector3.Distance(pos, position) <= cellSize / 2)
                return true;
        }
        return false;
    }

    // Метод, который проверяет, находится ли юнит в сетке
    public void CheckUnitPlacement(Vector3 unitPosition)
    {
        if (IsWithinPlacementZone(unitPosition))
        {
            isUnitInGrid = true;
        }
        else
        {
            isUnitInGrid = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 origin = transform.position + gridOffset - new Vector3(gridSize.x / 2f * cellSize, 0, gridSize.y / 2f * cellSize);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int z = 0; z < gridSize.y; z++)
            {
                Vector3 pos = origin + new Vector3(x * cellSize, 0, z * cellSize);
                Gizmos.DrawWireCube(pos + Vector3.up * 0.1f, new Vector3(cellSize, 0.1f, cellSize));
            }
        }
    }
}
