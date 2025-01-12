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

    void Start()
    {
        UnitSelectionManager.Instance.allUnitsList.Add(gameObject);

        unitHealth = unitMaxHealth;
        UpdateHealthUI();

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        unitMovement = GetComponent<UnitMovement>();
        boxCollider = GetComponent<BoxCollider>();
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
            animator.SetBool("isDead",true);
            Destroy(boxCollider);
            Destroy(sphereCollider);
            //attackController.enabled = false;
            isDead = true;
            unitMovement.enabled = false;
            agent.enabled = false;
            gameObject.layer = 0;
            gameObject.tag = "Untagged";
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
