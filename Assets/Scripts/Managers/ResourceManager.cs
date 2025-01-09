using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
        }
    }

    private int gold;

    public event Action OnResourceChanged;

    public enum ResourcesType
    {
        Gold,
    }

    public void IncreaseResource(ResourcesType resource, int amountToIncrease)
    {
        switch (resource)
        {
            case ResourcesType.Gold:
                gold += amountToIncrease;
                break;

            default:
                break;
        }

        OnResourceChanged?.Invoke();
    }

    public void DecreaseResource(ResourcesType resource, int amountToDecrease)
    {
        switch (resource)
        {
            case ResourcesType.Gold:
                gold -= amountToDecrease;
                break;
                
            default:
                break;
        }
        
        OnResourceChanged?.Invoke();
    }

    public int GetGold()
    {
        return gold;
    }
}
