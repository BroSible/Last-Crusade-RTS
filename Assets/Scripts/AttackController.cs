using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Transform targetToAttack;
    public int unitDamage;

    // Ссылка на UnitDatabaseSO для загрузки данных юнитов
    [SerializeField] private UnitDatabaseSO unitDatabase;  
    public int id;

    void Start()
    {

        if (unitDatabase != null && unitDatabase.units.Count > 0)
        {
            // Получаем первый юнит из списка и присваиваем его максимальное здоровье
            UnitData Unit = unitDatabase.units[id];
            unitDamage = Unit.unitDamage;
        }

        else
        {
            Debug.LogWarning("UnitDatabase is not assigned or empty.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack == null)
        {
            targetToAttack = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy") && targetToAttack != null)
        {
            targetToAttack = null;
        }
    }
}
