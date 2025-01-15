using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;

    [SerializeField] private ObjectsDatabseSO database;

    [SerializeField] private GridData floorData, furnitureData;

    [SerializeField] private PreviewSystem previewSystem;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    [SerializeField] private LayerMask obstacleLayer;  // Слой препятствий для проверки

    int selectedID;

    IBuildingState buildingState;

    private void Start()
    {
        floorData = new();
        furnitureData = new();
    }

    public void StartPlacement(int ID)
    {
        selectedID = ID;

        StopPlacement();

        buildingState = new PlacementState(ID, grid, previewSystem, database, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void RemovePlacementData(Vector3 position)
    {
        floorData.RemoveObjectAt(grid.WorldToCell(position));
    }

    public void StartRemoving()
    {
        StopPlacement();

        buildingState = new RemovingState(grid, previewSystem, floorData, furnitureData, objectPlacer);

        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        Vector3 worldPosition = grid.CellToWorld(gridPosition);

        ObjectData selectedObject = database.GetObjectByID(selectedID);

        // Проверка, является ли объект юнитом
        if (selectedObject.Prefab.TryGetComponent(out Unit unit))
        {
            // Проверка на препятствия
            if (IsPositionBlocked(worldPosition))
            {
                Debug.Log("Нельзя разместить юнита: позиция занята препятствием.");
                return;
            }

            // Проверка, входит ли позиция в зону размещения (например, у казармы)
            if (!IsWithinBarracksPlacementZone(worldPosition))
            {
                Debug.Log("Нельзя разместить юнита за пределами зоны размещения.");
                return;
            }
        }

        buildingState.OnAction(gridPosition);

        StopPlacement();
    }

    private bool IsPositionBlocked(Vector3 position)
    {
        float checkRadius = 0.5f; // Радиус проверки
        return Physics.CheckSphere(position, checkRadius, obstacleLayer);
    }

    private bool IsWithinBarracksPlacementZone(Vector3 position)
    {
        BarracksGrid[] barracksZones = FindObjectsOfType<BarracksGrid>();

        foreach (var zone in barracksZones)
        {
            if (zone.IsWithinPlacementZone(position))
                return true;
        }
        return false;
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;

        buildingState.EndState();

        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;

        lastDetectedPosition = Vector3Int.zero;

        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
            return;

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }
}
