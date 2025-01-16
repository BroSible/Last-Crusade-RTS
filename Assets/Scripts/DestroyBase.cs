using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBase : MonoBehaviour
{
    Constructable constructable;

    void Start()
    {
        constructable = GetComponent<Constructable>();
    }

    public void TakeDamage(int damage)
    {
        CheckHealth();
    }

    private void CheckHealth()
    {
        if (constructable.constHealth <= 0)
        {
            DestroyBuilding();
        }
    }

    private void DestroyBuilding()
    {
        Destroy(gameObject);
        Time.timeScale = 0f; // Пауза игры
        Debug.Log("Специальное здание уничтожено. Игра на паузе.");
    }
}
