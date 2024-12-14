using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    public float unitHealth;
    public float unitMaxHealth;
    public HealthTracker healthTracker;

    Animator animator;
    NavMeshAgent agent;
    UnitMovement unitMovement;

    void Start()
    {
        UnitSelectionManager.Instance.allUnitsList.Add(gameObject);

        unitHealth = unitMaxHealth;
        UpdateHealthUI();

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        unitMovement = GetComponent<UnitMovement>();
    }

    private void Update()
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

    void OnDestroy()
    {
        UnitSelectionManager.Instance.allUnitsList.Remove(gameObject);
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(unitHealth, unitMaxHealth);

        if(unitHealth <= 0)
        {
            // unit died
            Destroy(gameObject);
        }
    }

    internal void TakeDamage(int damageToInflict)
    {
        unitHealth -= damageToInflict;
        UpdateHealthUI();
    }
}
