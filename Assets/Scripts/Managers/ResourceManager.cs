using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private void Start()
    {
        UpdateUI();
    }

    public int gold = 150;

    public event Action OnResourceChanged;

    public TextMeshProUGUI goldUI;

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
                goldUI.text = gold.ToString();
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
                goldUI.text = gold.ToString();
                break;
                
            default:
                break;
        }

        OnResourceChanged?.Invoke();
    }

    private void OnEnable()
    {
        OnResourceChanged += UpdateUI;
    }

    private void OnDisable()
    {
        OnResourceChanged -= UpdateUI;
    }

    public void UpdateUI()
    {
        goldUI.text = gold.ToString();
    }

    public int GetGold()
    {
        return gold;
    }

    internal int GetResourceAmount(ResourcesType resource)
    {
        switch(resource)
        {
            case ResourcesType.Gold:
                return gold;
                
            default:
                break;
        }

        return 0;
    }

    internal void DecreaseResourcesBasedOnRequirement(ObjectData objectData)
    {
        foreach(BuildRequirement req in objectData.requirements)
        {
            DecreaseResource(req.resource, req.amount);
        }
    }
}
