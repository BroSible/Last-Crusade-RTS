using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ObjectPlacer : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> placedGameObjects = new();

    public int PlaceObject(GameObject prefab, Vector3 position)
    {
        // Создание объекта на сцене
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = position;

        // Проверка, является ли объект зданием
        Constructable constructable = newObject.GetComponent<Constructable>();
        if (constructable != null)
        {
            constructable.ConstructableWasPlaced();
            constructable.buildPosition = position;
        }

        // Проверка, является ли объект юнитом
        Unit unit = newObject.GetComponent<Unit>();
        AttackController attackController = newObject.GetComponent<AttackController>();
        Animator animator = newObject.GetComponent<Animator>();
        NavMeshAgent navMeshAgent = newObject.GetComponent<NavMeshAgent>();
        if (unit != null)
        {
            unit.enabled = true;  // Активируем скрипт юнита
            attackController.enabled = true;
            animator.enabled = true;
            gameObject.tag = "Unit";
            gameObject.layer = 6;
            navMeshAgent.enabled = true;
        }

        // Сохранение объекта в списке размещённых объектов
        placedGameObjects.Add(newObject);

        return placedGameObjects.Count - 1;
    }

    internal void RemoveObjectAt(int gameObjectIndex)
    {
        if (placedGameObjects.Count <= gameObjectIndex || placedGameObjects[gameObjectIndex] == null)
            return;

        Destroy(placedGameObjects[gameObjectIndex]);
        placedGameObjects[gameObjectIndex] = null;
    }
}
