using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour, IDamageable
{
    public float unitHealth;
    public float unitMaxHealth;
    public bool isDead = false;
    public HealthTracker healthTracker;
    BoxCollider boxCollider;
    SphereCollider sphereCollider;
    Animator animator;
    NavMeshAgent agent;
    UnitMovement unitMovement;
    AttackController attackController;
    public int id;

    // Добавьте ссылку на базу данных юнитов
    public UnitDatabaseSO unitDatabase;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        unitMovement = GetComponent<UnitMovement>();
        boxCollider = GetComponent<BoxCollider>();
        
        // Проверяем, есть ли база данных и если в ней есть юниты
        if (unitDatabase != null && unitDatabase.units.Count > 0)
        {
            // Получаем первый юнит из списка и присваиваем его максимальное здоровье
            UnitData Unit = unitDatabase.units[id];
            unitMaxHealth = Unit.maxHealth;
            agent.speed = Unit.speedUnit;
        }

        else
        {
            Debug.LogWarning("UnitDatabase is not assigned or empty.");
        }

        unitHealth = unitMaxHealth;
        UpdateHealthUI();


    }

    private void Update()
    {
        if(!isDead)
        {
            // Agent reached destination
            if(agent.remainingDistance > agent.stoppingDistance)
            {
                animator.SetBool("isFollow", true);
            }
            else
            {
                unitMovement.isCommandedToMove = false;
                animator.SetBool("isFollow", false);
            }
        }
    }

    void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);

        if(unitHealth <= 0)
        {
            animator.SetBool("isDead", true);
            Destroy(boxCollider);
            Destroy(sphereCollider);
            isDead = true;
            unitMovement.enabled = false;
            agent.enabled = false;
            StartCoroutine(deathCoroutine());
        }
    }

    public void TakeDamage(int damageToInflict)
    {
        unitHealth -= damageToInflict;
        UpdateHealthUI();
    }

    private IEnumerator deathCoroutine()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
}
