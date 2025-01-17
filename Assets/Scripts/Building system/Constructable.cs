using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Constructable : MonoBehaviour, IDamageable
{
    public float constHealth;
    public float constMaxHealth;
    public HealthTracker healthTracker;
    public bool isEnemy = false;
    NavMeshObstacle obstacle;
    public Vector3 buildPosition;


    void Start()
    {
        constHealth = constMaxHealth;
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        healthTracker.UpdateSliderValue(constHealth, constMaxHealth);

        if(constHealth <= 0)
        {
            // Other destruction logic

            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        constHealth -= damage;
        UpdateHealthUI();
    }

    public void ConstructableWasPlaced()
    {
        ActivateObstacle();
    }

    private void ActivateObstacle()
    {
        if(isEnemy)
        {
            gameObject.tag = "Enemy";
        }

        obstacle = GetComponentInChildren<NavMeshObstacle>();
        obstacle.enabled = true;
    }

}
