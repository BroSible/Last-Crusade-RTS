using System;
using UnityEngine;
using UnityEngine.UI;

public class BuySlot : MonoBehaviour
{
    public bool isAvailable;

    public BuyingSystem buyingSystem;

    public int databaseItemId;

    private Button button;
    private Image buttonImage;
    private Color originalColor;
    private Color unavailableColor = new Color(0.5f, 0.5f, 0.5f, 1f); // Серый цвет

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (buttonImage != null)
        {
            originalColor = buttonImage.color; // Сохраняем исходный цвет кнопки
        }

        HandleResourcesChanged();
    }

    public void ClickedOnSlot()
    {
        if (isAvailable)
        {
            buyingSystem.placementSystem.StartPlacement(databaseItemId);
        }
    }

    private void UpdateAvailabilityUI()
    {
        if (isAvailable)
        {
            if (button != null)
            {
                button.interactable = true; // Разрешаем взаимодействие
            }

            if (buttonImage != null)
            {
                buttonImage.color = originalColor; // Восстанавливаем оригинальный цвет
            }
        }
        else
        {
            if (button != null)
            {
                button.interactable = false; // Запрещаем взаимодействие
            }

            if (buttonImage != null)
            {
                buttonImage.color = unavailableColor; // Устанавливаем серый цвет
            }
        }
    }

    private void OnEnable()
    {
        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnResourceChanged += HandleResourcesChanged;
            Debug.Log($"Кнопка с ID {databaseItemId} подписалась на событие");
        }
        else
        {
            Debug.LogError($"ResourceManager.Instance = null для кнопки с ID {databaseItemId}. Проверь порядок инициализации.");
        }
    }



    private void OnDisable()
    {
        ResourceManager.Instance.OnResourceChanged -= HandleResourcesChanged;
        
    }


    private void HandleResourcesChanged()
    {
        ObjectData objectData = DatabaseManager.Instance.databaseSO.objectsData[databaseItemId];

        bool requirementMet = true;

        foreach(BuildRequirement req in objectData.requirements)
        {
            if(ResourceManager.Instance.GetResourceAmount(req.resource) < req.amount)
            {
                requirementMet = false;
                break;
            }
        }

        isAvailable = requirementMet;

        UpdateAvailabilityUI();
    }
}
